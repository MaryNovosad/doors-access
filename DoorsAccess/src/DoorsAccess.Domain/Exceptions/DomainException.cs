namespace DoorsAccess.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainErrorType ErrorType { get; set; }
        public DomainException(DomainErrorType type, string message) : base(message)
        {
            ErrorType = type;
        }
    }
}
