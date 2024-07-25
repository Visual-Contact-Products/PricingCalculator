
using Newtonsoft.Json;
using PricingCalculator.Clients.Interfaces;
using PricingCalculator.Models;
using PricingCalculator.Models.Errors;
using PricingCalculator.Models.Results;
using System.Text;

namespace PricingCalculator.Clients
{
    public class EmailClient(HttpClient httpClient, ILogger<EmailClient> logger) : IEmailClient
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger _logger = logger;

        public async Task<Result<string, Error>> SendEmailAsync(EmailMessage message)
        {
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    // Agregar partes al contenido
                    content.Add(new StringContent(message.From), "From");
                    content.Add(new StringContent(message.To), "To");
                    content.Add(new StringContent(message.Subject), "Subject");
                    content.Add(new StringContent(message.Content), "Content");

                    // Enviar la solicitud
                    var response = await _httpClient.PostAsync("email", content);
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Email sent successfully");
                        return Result<string, Error>.Success();
                    }
                    else
                    {
                        _logger.LogError("Email failed to send");
                        return Result<string, Error>.Failure(Errors.EmailCanNotBeSended);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}