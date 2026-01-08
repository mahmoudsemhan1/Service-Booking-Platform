using Domain.Models.Base;

namespace Domain.Models
{
    public class Service : RatableEntity
    {
        public int Id { get; private set; }
        public string Title { get; private set; } = null!;
        public string? Description { get; private set; }
        public decimal? BasePrice { get; private set; }
        public int? DurationMinutes { get; private set; }

        private readonly List<Image> _images = new();
        public IReadOnlyCollection<Image> Images => _images.AsReadOnly();
        public ICollection<ProviderService> ProviderServices { get; private set; } = new List<ProviderService>();

        public void AddImage(string path, bool isPrimary = false)
        {
            if (!_images.Any())
                isPrimary = true;
            if (isPrimary)
            {
                foreach (var existingImage in _images)
                {
                    existingImage.UnmarkPrimary();
                }
            }
            _images.Add(new Image(path, isPrimary));

        }
        public void RemoveImage(int ImageID)
        {
            var image = _images.FirstOrDefault(p => p.Id == ImageID);
            if (image != null) _images.Remove(image);

        }
        // Navigation properties

        private Service() { } // EF Core

        public Service(string title, decimal? price, string? description = null, int? durationMinutes = null)
        {
            Title = !string.IsNullOrWhiteSpace(title) ? title : throw new ArgumentNullException(nameof(title));
            BasePrice = price >= 0 ? price : throw new ArgumentException("Price cannot be negative.", nameof(price));
            Description = description;
            DurationMinutes = durationMinutes;
        }

        public void Update(string? title = null, decimal? price = null, int? durationMinutes = null, string? description = null)
        {
            if (!string.IsNullOrWhiteSpace(title))
                Title = title;

            if (price.HasValue)
            {
                if (price.Value < 0) throw new ArgumentException("Price cannot be negative.");
                BasePrice = price.Value;
            }

            if (durationMinutes.HasValue)
                DurationMinutes = durationMinutes;

            if (description != null)
                Description = description;


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
