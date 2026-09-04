using RoomReservation.Core.Enums;

namespace RoomReservation.Core.Results.Common
{
    public class Error
    {
        public string ErrorMessage { get; init; } = string.Empty;
        public ErrorType ErrorType { get; init; } = ErrorType.BadRequest;
        public Error(string errorMessage, ErrorType errorType)
        {
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }
        public override string ToString()
            => ErrorMessage;
    }
}
