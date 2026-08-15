namespace FastKart.Models.Responses
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public required object Data { get; set; }
        public ApiError? Error { get; set; }
    }

    public class ApiError
    {
        public required string Code { get; set; }
        public required string Message { get; set; }
        public Dictionary<string, string[]?> Details { get; set; } = [];
    }

    public static class ApiErrorCodes
    {
        // General
        public const string UnknownError = "UNKNOWN_ERROR";
        public const string InternalServerError = "INTERNAL_SERVER_ERROR";
        public const string InvalidRequest = "INVALID_REQUEST";
        public const string ValidationFailed = "VALIDATION_FAILED";

        // Authentication
        public const string Unauthorized = "UNAUTHORIZED";
        public const string InvalidCredentials = "INVALID_CREDENTIALS";
        public const string InvalidToken = "INVALID_TOKEN";
        public const string TokenExpired = "TOKEN_EXPIRED";
        public const string AccountLocked = "ACCOUNT_LOCKED";
        public const string AccountDisabled = "ACCOUNT_DISABLED";

        // Authorization
        public const string Forbidden = "FORBIDDEN";
        public const string InsufficientPermissions = "INSUFFICIENT_PERMISSIONS";

        // Users
        public const string UserNotFound = "USER_NOT_FOUND";
        public const string UserAlreadyExists = "USER_ALREADY_EXISTS";
        public const string UsernameAlreadyExists = "USERNAME_ALREADY_EXISTS";
        public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";

        // Roles
        public const string RoleNotFound = "ROLE_NOT_FOUND";
        public const string OutOfSetPermission = "OUT_OF_SET_PERMISSION";
        public const string RoleAlreadyExists = "ROLE_ALREADY_EXISTS";

        // Registration
        public const string RegistrationFailed = "REGISTRATION_FAILED";
        public const string InvalidUsername = "INVALID_USERNAME";
        public const string InvalidEmail = "INVALID_EMAIL";
        public const string WeakPassword = "WEAK_PASSWORD";
        public const string PasswordMismatch = "PASSWORD_MISMATCH";

        // Resources
        public const string NotFound = "NOT_FOUND";
        public const string ResourceAlreadyExists = "RESOURCE_ALREADY_EXISTS";
        public const string ResourceConflict = "RESOURCE_CONFLICT";

        // Database
        public const string DatabaseError = "DATABASE_ERROR";
        public const string DatabaseConnectionFailed = "DATABASE_CONNECTION_FAILED";
        public const string DatabaseOperationFailed = "DATABASE_OPERATION_FAILED";

        // Request
        public const string MissingParameter = "MISSING_PARAMETER";
        public const string InvalidParameter = "INVALID_PARAMETER";
        public const string InvalidFormat = "INVALID_FORMAT";
        public const string InvalidId = "INVALID_ID";

        // Rate limiting
        public const string TooManyRequests = "TOO_MANY_REQUESTS";
    }
}
