using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domain.Models
{
    public class Provider : AuditableEntity
    {
        // استخدام private set لتحقيق الـ Encapsulation
        public int Id { get; private set; }
        public string UserId { get; private set; } = null!;
        public string BusinessName { get; private set; } = null!;
        public string? Address { get; private set; }
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public TimeSpan? OpenTime { get; private set; }
        public TimeSpan? CloseTime { get; private set; }

        public bool IsActive { get; private set; } = true;

        private readonly List<ProviderService> _providerServices = new();
        public virtual IReadOnlyCollection<ProviderService> ProviderServices => _providerServices.AsReadOnly();

        private readonly List<Review> _reviews = new();
        public virtual IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

        // EF Core Constructor
        private Provider() { }

        public Provider(string userId, string businessName, string? address = null)
        {
            UserId = string.IsNullOrWhiteSpace(userId) ? throw new ArgumentNullException(nameof(userId)) : userId;
            BusinessName = string.IsNullOrWhiteSpace(businessName) ? throw new ArgumentNullException(nameof(businessName)) : businessName;
            Address = address;
        }

        public void UpdateBusinessInfo(string businessName, string? address, TimeSpan? openTime, TimeSpan? closeTime)
        {
            if (string.IsNullOrWhiteSpace(businessName))
                throw new ArgumentException("Business name cannot be empty.");

            if (openTime.HasValue && closeTime.HasValue)
            {
                if (closeTime.Value <= openTime.Value)
                {
                    throw new ArgumentException("Close time must be after open time.");
                }
            }

            BusinessName = businessName;
            Address = address;
            OpenTime = openTime;
            CloseTime = closeTime;

      
        }
        public void ToggleStatus()
        {
            IsActive = !IsActive;
        }
        public void UpdateLocation(string? address, double? lat, double? lon)
        {
            Address = address;
            Latitude = lat;
            Longitude = lon;
        }

        public void SetBusinessHours(TimeSpan open, TimeSpan close)
        {
            if (close <= open) throw new ArgumentException("Close time must be after open time.");
            OpenTime = open;
            CloseTime = close;
        }


        public void AddService(int serviceId, decimal price, decimal? discountedPrice = null)
        {
            if (_providerServices.Any(ps => ps.ServiceId == serviceId))
                throw new InvalidOperationException("Service is already assigned to this provider.");

            var providerService = new ProviderService(serviceId, price, discountedPrice);
            _providerServices.Add(providerService);
        }

        public void RemoveService(int serviceId)
        {
            var service = _providerServices.FirstOrDefault(ps => ps.ServiceId == serviceId);
            if (service != null)
            {
                _providerServices.Remove(service);
            }
        }

        public void UpdateServicePricing(int serviceId, decimal newPrice, decimal? newDiscountedPrice = null)
        {
            var service = _providerServices.FirstOrDefault(ps => ps.ServiceId == serviceId);
            if (service == null)
                throw new KeyNotFoundException("Service not found for this provider.");

            service.UpdatePricing(newPrice, newDiscountedPrice);
        }
    }
}