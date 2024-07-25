namespace PricingCalculator.Models.Results
{
    public class Result<TValue, TError>
    {

        public bool IsSuccess { get; private set; }
        public List<TError> Errors { get; private set; } = [];
        public TValue? Value { get; private set; }
        public Result(TValue value)
        {
            IsSuccess = true;
            Value = value;
        }
        public Result(TError error)
        {
            IsSuccess = false;
            Errors.Add(error);
            Value = default;
        }
        public Result(List<TError> errors)
        {
            IsSuccess = false;
            Errors = errors;
            Value = default;
        }
        public Result()
        {
            IsSuccess = true;
            Value = default;
        }

        public static Result<TValue, TError> Success(TValue value) => new(value);
        public static Result<TValue, TError> Success() => new();
        public static Result<TValue, TError> Failure(TError error) => new(error);

        public static Result<TValue, TError> Failure(List<TError> errors) => new(errors);
        /// <summary>
        /// Implicitly converts a value to a successful result.
        /// </summary>
        /// <param name="value">The success value.</param>
        public static implicit operator Result<TValue, TError>(TValue value) => Success(value);

        /// <summary>
        /// Implicitly converts an error to a failed result.
        /// </summary>
        /// <param name="error">The error.</param>
        public static implicit operator Result<TValue, TError>(TError error) => Failure(error);

        /// <summary>
        /// Implicitly converts a Results to a boolean indicating success or failure.
        /// </summary>
        /// <param name="result">The result.</param>
        public static implicit operator bool(Result<TValue, TError> result) => result.IsSuccess;
    }
}
