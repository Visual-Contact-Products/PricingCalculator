using PricingCalculator.Models;
using PricingCalculator.Models.Errors;
using PricingCalculator.Models.Results;

namespace PricingCalculator.Clients.Interfaces
{
    public interface IEmailClient
    {
        Task<Result<string, Error>> SendEmailAsync(EmailMessage message);
    }
}