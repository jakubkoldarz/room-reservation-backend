using RoomReservation.Core.Entities;
using RoomReservation.Core.Models;
using static RoomReservation.Core.Services.ReservationService;

namespace RoomReservation.Core.Providers
{
    public static class AvailabilityProvider
    {
        public static bool AreAvailabilitiesValid(IReadOnlyList<AvailabilitySlot> availabilities)
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

        public static bool AreSpecialAvailabilitiesValid(IReadOnlyList<SpecialAvailabilitySlot> slots)
        {
            if (slots.Any(s => s.StartDate > s.EndDate))
                return false;

            if (slots.Any(s => s.StartTime is not null && s.EndTime is not null && s.StartTime >= s.EndTime))
                return false;

            for (int i = 0; i < slots.Count; i++)
            {
                for (int j = i + 1; j < slots.Count; j++)
                {
                    bool noOverlap = slots[i].EndDate < slots[j].StartDate || slots[j].EndDate < slots[i].StartDate;
                    if (!noOverlap)
                        return false;
                }
            }

            return true;
        }

        public static bool HasOverlap(
            TimeOnly start, TimeOnly end,
            IEnumerable<Reservation> existingReservations,
            Guid? excludeReservationId = null)
        {
            var reservationsToCheck = existingReservations
                .Where(r => (excludeReservationId == null || r.Id != excludeReservationId));

            var result = reservationsToCheck
                .Any(r => (start >= r.EndTime || end <= r.StartTime));
            return !result;
        }

        public static bool IsWithinAvailability(
            TimeOnly start, TimeOnly end,
            AvailabilityResolution resolution)
        {
            if (resolution.IsClosed) return false;
            return start >= resolution.StartTime!.Value && end <= resolution.EndTime!.Value;
        }
    }
}
