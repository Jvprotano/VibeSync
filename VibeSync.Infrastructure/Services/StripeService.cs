using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using VibeSync.Application.Contracts.Services;
using VibeSync.Application.Helpers;

namespace VibeSync.Infrastructure.Services;

public class StripeService : IStripeService
{
    private readonly StripeSettings _settings;

    public StripeService(IOptions<StripeSettings> settings)
    {
        _settings = settings.Value;
        StripeConfiguration.ApiKey = _settings.SecretKey;
    }

    public async Task<string> CreateCheckoutSessionAsync(string stripePriceId, Guid planId, Guid userId)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = stripePriceId,
                    Quantity = 1,
                },
            },
            Mode = "subscription",
            SuccessUrl = $"http://localhost:4200/success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"http://localhost:4200/pricing",
            Metadata = new Dictionary<string, string>
            {
                { "userId", userId.ToString() },
                { "planId", $"{planId}"}
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return session.Url;
    }
}
