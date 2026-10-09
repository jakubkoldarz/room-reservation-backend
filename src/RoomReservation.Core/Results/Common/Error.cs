using RoomReservation.Core.Enums;

namespace RoomReservation.Core.Results.Common
{
    public record Error(string ErrorMessage, ErrorType ErrorType)
    {
        public sealed override string ToString()
            => ErrorMessage;
    }
}
