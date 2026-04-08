namespace MechanicShop.Domain.Common.Results
{
    public readonly record struct Error
    {
        public string Code { get; }
        public string Message { get; }
        public ErrorKind Kind { get; }

        private Error(string code, string message, ErrorKind kind)
        {
            Code = code;
            Message = message;
            Kind = kind;
        }

        public static Error Failure(string code, string message) =>
            new (code, message, ErrorKind.Failure);

        public static Error Unexpected(string code, string message) =>
            new(code, message, ErrorKind.Unexpected);

        public static Error Validation(string code, string message) =>
            new(code, message, ErrorKind.Validation);

        public static Error NotFound(string code, string message) =>
            new(code, message, ErrorKind.NotFound);

        public static Error Conflict(string code, string message) =>
            new(code, message, ErrorKind.Conflict);

        public static Error Forbidden(string code, string message) =>
            new(code, message, ErrorKind.Forbidden);

        public static Error Unauthorized(string code, string message) =>
            new(code, message, ErrorKind.Unauthorized);

    }
}