using Domain.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Provider> Providers => Set<Provider>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<ProviderService> ProviderServices => Set<ProviderService>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =======================
            // Identity Relationships
            // =======================

            // UserProfile 1:1 ApplicationUser
            builder.Entity<UserProfile>()
                .HasOne<ApplicationUser>()
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Provider 1:1 ApplicationUser
            builder.Entity<Provider>()
                .HasOne<ApplicationUser>()
                .WithOne(u => u.Provider)
                .HasForeignKey<Provider>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking -> ApplicationUser (Many:1)
            builder.Entity<Booking>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment -> ApplicationUser (Many:1)
            builder.Entity<Payment>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review -> ApplicationUser (Many:1)
            builder.Entity<Review>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =======================
            // Domain Relationships
            // =======================


            // Owned Types (Images)
            // إعداد صور الخدمات
            builder.Entity<Service>(entity =>
            {
                entity.OwnsMany(s => s.Images, imageBuilder =>
                {
                    imageBuilder.ToTable("ServiceImages");
                    imageBuilder.HasKey(p => p.Id);
                    imageBuilder.WithOwner().HasForeignKey("ServiceId");

                    // السطر ده هو اللي هيحل مشكلة الـ NULL
                    // بنقوله إن خاصية ImagePath في الكلاس مرتبطة بعمود اسمه ImagePath في الجدول وهي إجبارية
                    imageBuilder.Property(p => p.ImagePath)
                                .HasColumnName("ImagePath")
                                .IsRequired();

                    imageBuilder.Property(p => p.IsPrimary)
                                .HasDefaultValue(false);
                });
            });


            // Configuration for ProviderService (The Marketplace Logic)
            builder.Entity<ProviderService>(entity =>
            {
                entity.HasIndex(ps => new { ps.ProviderId, ps.ServiceId })
                      .IsUnique();

                entity.Property(ps => ps.Price)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(ps => ps.DiscountedPrice)
                      .HasColumnType("decimal(18,2)");
            });

            // ProviderService relationships
            builder.Entity<ProviderService>()
                .HasOne(ps => ps.Provider)
                .WithMany(p => p.ProviderServices)
                .HasForeignKey(ps => ps.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProviderService>()
                .HasOne(ps => ps.Service)
                .WithMany(s => s.ProviderServices)
                .HasForeignKey(ps => ps.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking relationships
            builder.Entity<Booking>()
                .HasOne(b => b.Service)
                .WithMany()
                .HasForeignKey(b => b.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Provider)
                .WithMany()
                .HasForeignKey(b => b.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment 1:1 Booking
            builder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review -> Provider
            builder.Entity<Review>()
                .HasOne(r => r.Provider)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            // =======================
            // Soft Delete Filters
            // =======================

            builder.Entity<UserProfile>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<Provider>().HasQueryFilter(x => !x.IsDeleted && x.IsActive);
            builder.Entity<Service>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<ProviderService>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<Booking>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<Payment>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<Review>().HasQueryFilter(x => !x.IsDeleted);

            // =======================
            // Precision Configurations
            // =======================

            builder.Entity<ProviderService>()
                .Property(ps => ps.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<ProviderService>()
                .Property(ps => ps.DiscountedPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Service>()
                .Property(s => s.BasePrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");
        }
    }
}
