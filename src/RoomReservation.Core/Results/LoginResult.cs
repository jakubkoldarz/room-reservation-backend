using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RoomReservation.Core.Results
{
    public class LoginResult
    {
        [MemberNotNullWhen(true, nameof(VerificationId))]
        [MemberNotNullWhen(false, nameof(JwtToken), nameof(RefreshToken))]
        public bool Requires2FA { get; set; }

        public Guid? VerificationId { get; set; }
        public string? JwtToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
