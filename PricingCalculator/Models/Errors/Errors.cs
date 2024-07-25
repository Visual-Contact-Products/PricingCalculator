namespace PricingCalculator.Models.Errors
{
    public static class Errors
    {
        //application errors
        public static readonly Error UnauthorizedAccess = new("UnauthorizedAccess", "Unauthorized access to the resource", 401);
        public static readonly Error ServerError = new("ServerError", "Internal server error occurred", 500);
        public static readonly Error ResourceNotFound = new("ResourceNotFound", "Requested resource not found", 404);
        public static readonly Error BadRequest = new("BadRequest", "Bad request - invalid or missing data", 400);
        public static readonly Error Timeout = new("Timeout", "Operation timed out", 408);
        public static readonly Error ConnectionError = new("ConnectionError", "Error connecting to external service", 500);
        public static readonly Error HttpRequestError = new("HttpRequestError", "Error sending or receiving an HTTP request.", 500);
        public static readonly Error SocketError = new("SocketError", "Error in a socket operation.", 500);
        public static readonly Error GenericError = new("GenericError", "An unspecified error has occurred.", 500);
        //Product errors
        public static readonly Error ProductNotFound = new("ProductNotFound", "Product not found", 404);
        //Category errors
        public static readonly Error CategoryNotFound = new("CategoryNotFound", "Category not found", 404);
        //user errors
        public static readonly Error UserNotFound = new("UserNotFound", "User not found", 404);
        public static readonly Error UserIsNotActive = new("UserIsNotActive", "User is not active", 401);
        public static readonly Error RolesNotSpecified = new("RolesNotSpecified", "Roles not specified", 400);
        public static readonly Error EmailCanNotBeEmpty = new("EmailCanNotBeEmpty", "Email Can Not Be Empty", 400);
        //auth errors
        public static readonly Error PasswordDoesNotMatch = new("PasswordDoesNotMatch", "Password doesn't match", 401);
        public static readonly Error InvalidRefreshToken = new("InvalidRefreshToken", "Invalid refresh token", 401);
        public static readonly Error RefreshTokenNotFound = new("RefreshTokenNotFound", "Refresh token not found", 401);
        //Role errors
        public static readonly Error RoleNotFound = new("RoleNotFound", "Role not found", 404);

        public static readonly Error EmailServiceNotAvailable = new("EmailServiceNotAvailable", "Email Service Not Available", 500);
        public static readonly Error EmailCanNotBeSended = new("EmailCanNotBeSended", "Email Can Not Be Sended", 500);
    }
}
