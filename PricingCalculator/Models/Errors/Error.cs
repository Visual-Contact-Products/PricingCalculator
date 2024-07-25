using System.Text.Json.Serialization;

namespace PricingCalculator.Models.Errors
{
    /// <summary>
    /// Represents an error with a code and a description.
    /// </summary>
    /// <param name="Code">The error code.</param>
    /// <param name="Description">The error description.</param>
    public sealed record Error(string Code, string Description)
    {
        /// <summary>
        /// Represents a non-error, with empty code and description.
        /// </summary>
        public static readonly Error None = new(string.Empty, string.Empty);

        // New field to save the http code
        [JsonIgnore]
        public int HttpStatusCode { get; } = 400;

        public Error(string code, string description, int httpStatusCode = 400) : this(code, description)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}
