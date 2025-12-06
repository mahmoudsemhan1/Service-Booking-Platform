using Domain.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { }


        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Provider> Providers => Set<Provider>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<ProviderService> ProviderServices => Set<ProviderService>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Image> Images => Set<Image>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // UserProfile 1:1 with User
            builder.Entity<UserProfile>()
            .HasOne(up => up.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<UserProfile>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            // Provider 1:1 with User
            builder.Entity<Provider>()
                .HasOne(p=>p.User)
                .WithOne(u=>u.Provider)
                .HasForeignKey<Provider>(p=>p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // ProviderService unique index
            builder.Entity<ProviderService>()
                .HasIndex(ps => new { ps.ProviderId, ps.ServiceId })
                .IsUnique();

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

            builder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment relationships
            builder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review relationships
            builder.Entity<Review>()
                .HasOne(r => r.Provider)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Soft delete query filters
            builder.Entity<UserProfile>().HasQueryFilter(up => !up.IsDeleted);
            builder.Entity<Provider>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Service>().HasQueryFilter(s => !s.IsDeleted);
            builder.Entity<ProviderService>().HasQueryFilter(ps => !ps.IsDeleted);
            builder.Entity<Booking>().HasQueryFilter(b => !b.IsDeleted);
            builder.Entity<Payment>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Review>().HasQueryFilter(r => !r.IsDeleted);
            builder.Entity<Image>().HasQueryFilter(i => !i.IsDeleted);

            // Decimal precision
            builder.Entity<Service>().Property(s => s.Price).HasColumnType("decimal(18,2)");
            builder.Entity<Booking>().Property(b => b.TotalPrice).HasColumnType("decimal(18,2)");
            builder.Entity<Payment>().Property(p => p.Amount).HasColumnType("decimal(18,2)");


        }


    }
}
