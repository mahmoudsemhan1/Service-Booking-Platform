using Domain.Models.Base;
using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Service : AuditableEntity
    {
        public int Id { get; private set; }
        public string Title { get; private set; } = null!;
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public int? DurationMinutes { get; private set; }
        public string? ImagePath { get; private set; }

        // Navigation properties
        public ICollection<ProviderService> ProviderServices { get; private set; } = new List<ProviderService>();

        private Service() { } // EF Core

        public Service(string title, decimal price, string? description = null, int? durationMinutes = null, string? imagePath = null)
        {
            Title = !string.IsNullOrWhiteSpace(title) ? title : throw new ArgumentNullException(nameof(title));
            Price = price >= 0 ? price : throw new ArgumentException("Price cannot be negative.", nameof(price));
            Description = description;
            DurationMinutes = durationMinutes;
            ImagePath = imagePath;
        }

        public void Update(string? title = null, decimal? price = null, int? durationMinutes = null, string? description = null, string? imagePath = null)
        {
            if (!string.IsNullOrWhiteSpace(title))
                Title = title;

            if (price.HasValue)
            {
                if (price.Value < 0) throw new ArgumentException("Price cannot be negative.");
                Price = price.Value;
            }

            if (durationMinutes.HasValue)
                DurationMinutes = durationMinutes;

            if (description != null)
                Description = description;

            if (imagePath != null)
                ImagePath = imagePath;
        }

        public void AddProviderService(ProviderService providerService)
        {
            if (providerService == null) throw new ArgumentNullException(nameof(providerService));
            ProviderServices.Add(providerService);
        }

        public void RemoveProviderService(ProviderService providerService)
        {
            if (providerService == null) throw new ArgumentNullException(nameof(providerService));
            ProviderServices.Remove(providerService);
        }
    }
}
