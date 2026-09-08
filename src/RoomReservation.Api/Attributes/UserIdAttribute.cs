using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RoomReservation.Api.Binders;

namespace RoomReservation.Api.Attributes
{
    public class UserIdAttribute : ModelBinderAttribute, IBindingSourceMetadata
    {
        public UserIdAttribute() : base(typeof(UserIdBinder)) {}

        public new BindingSource BindingSource = BindingSource.Special;
    }
}
