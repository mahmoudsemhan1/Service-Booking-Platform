using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceBooking.UnitTests.IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // اول حاجه بعملها بدور ع dbcontext الحقيقي 
                var descriptor = services.SingleOrDefault(
                     d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                // هنشتغل ب dbcontext وهمي
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting")
                    // يطنش اي تحذير يخص ال transactions
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                });
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();
                }
                //علشان يفهم ال schema الوهميه اللي عملتها 
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestScheme";
                    options.DefaultChallengeScheme = "TestScheme";
                })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });

            });
        }
    }
}
