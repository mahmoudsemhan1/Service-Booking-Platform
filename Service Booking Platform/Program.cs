using Application.Common.Models;
using Application.Interfaces.Services.BookingService;
using Application.Interfaces.Services.IfileService;
using Application.Interfaces.Services.Implement;
using Application.Interfaces.Services.IPaymentService;
using Application.Interfaces.Services.IProviderSerivce;
using Application.Interfaces.Services.IReviewServices;
using Application.Interfaces.Services.IServices;
using Application.Interfaces.Services.IUserIdentityServices;
using Application.Interfaces.Services.IUserProfileService;
using Application.Interfaces.Services.TokenService;
using Application.Mappings;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.UnitofWork;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using ServiceBooking.Api.Middlewares;
using Stripe;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// replace default logger with serilog 
// الربط مع appsettings.json
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Service Booking API", Version = "v1" });

    // 1. تعريف شكل الـ Security (JWT)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in this format: Bearer {your_token}"
    });

    // 2. تفعيل الـ Security في الـ Endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitofWork, UnitOfwork>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IProviderAppService, ProviderAppService>();
builder.Services.AddScoped<IFileService, Infrastructure.Services.FileService>();
builder.Services.AddScoped<ITokenService, Application.Interfaces.Services.Implement.TokenService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IUserIdentityService, UserIdentityService>();
builder.Services.AddScoped<Application.Interfaces.Services.IEmailService.IEmailService, EmailService>();
builder.Services.AddScoped<IReviewService, Application.Interfaces.Services.Implement.ReviewService>();
builder.Services.AddScoped<IUserProfileService, UserProfilesService>();

///
// تخصيص ردود أخطاء الـ Model Validation لتتناسب مع FluentValidation

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        // استخراج الأخطاء من الـ ModelState
        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        // تحويلها لـ ValidationException يلقطه الـ Middleware
        throw new FluentValidation.ValidationException(string.Join(" | ", errors));
    };
});

// Fluent Validation
// this line to enable automatic validation in ASP.NET Core
builder.Services.AddFluentValidationAutoValidation();
// this line to register all validators from the assembly where this code is located
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Stripe Configuration
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
// CORS Policy 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});
// Email Settings Configuration 
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
//  Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

//for httpcontextaccessor for getting user info from token and other things (url of images)
builder.Services.AddHttpContextAccessor();


builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = false // غالباً بنخليها false في المشاريع البسيطة
    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});


var app = builder.Build();
app.Use((context, next) =>
{
    context.Request.EnableBuffering();
    return next();
});

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
// CORS Policy 
app.UseCors("AllowAll");


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//  Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // نداء الميثود
        await ContextSeed.SeedAsync(userManager, roleManager);

        Console.WriteLine("Data Seeding Completed Successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred during seeding: {ex.Message}");
    }
}
// we guarantee (نضمن) that the logs are flushed and resources are released before the application exits
try
{
    Log.Information("Starting web host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush(); // بتمسح الـ memory وتتأكد إن كل سطر اتكتب في الفايل
}