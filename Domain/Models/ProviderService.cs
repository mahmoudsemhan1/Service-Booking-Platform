using Domain.Models;
using Domain.Models.Base;

public class ProviderService : AuditableEntity
{
    public int Id { get; private set; }
    public int ProviderId { get; private set; }
    public Provider? Provider { get; private set; }

    public int ServiceId { get; private set; }
    public Service? Service { get; private set; }

    public decimal Price { get; private set; }
    public decimal? DiscountedPrice { get; private set; }

    
    private ProviderService() { }

    public ProviderService(int serviceId, decimal price, decimal? discountedPrice = null)
    {
        if (price <= 0) throw new ArgumentException("Price must be greater than zero.");
        ValidateDiscount(price, discountedPrice);

        ServiceId = serviceId;
        Price = price;
        DiscountedPrice = discountedPrice;
    }

    public void UpdatePricing(decimal newPrice, decimal? newDiscountedPrice = null)
    {
        if (newPrice <= 0) throw new ArgumentException("Price must be greater than zero.");
        ValidateDiscount(newPrice, newDiscountedPrice);

        Price = newPrice;
        DiscountedPrice = newDiscountedPrice;
    }

    private void ValidateDiscount(decimal price, decimal? discount)
    {
        if (discount.HasValue && discount.Value > price)
            throw new ArgumentException("Discounted price cannot be higher than the original price.");
    }

    // Computed Properties (Read-Only) 
    public int DiscountPercentage =>
        (Price > 0 && DiscountedPrice.HasValue)
        ? (int)((1 - (DiscountedPrice.Value / Price)) * 100)
        : 0;
}