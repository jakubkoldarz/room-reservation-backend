using Microsoft.AspNetCore.Mvc;
using RoomReservation.Api.Binders;

namespace RoomReservation.Api.Attributes
{
    public class UserIdAttribute : ModelBinderAttribute
    {
        public UserIdAttribute() : base(typeof(UserIdBinder)) {}
    }
}
