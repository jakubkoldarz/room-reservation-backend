using RoomReservation.Core.Models;

namespace RoomReservation.Core.Providers
{
    public static class AvailabilityProvider
    {
        public static bool EnsureValidAvailabilities(IReadOnlyList<AvailabilitySlot> availabilities)
        {
            if (availabilities.Any(a => a.StartTime >= a.EndTime))
            {
                return false;
            }

            bool hasDuplicateDays = availabilities
                .GroupBy(a => a.DayOfWeek)
                .Any(group => group.Count() > 1);

            if (hasDuplicateDays)
            {
                return false;
            }

            return true;
        }
    }
}
