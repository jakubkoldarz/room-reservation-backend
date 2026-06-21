using RoomReservation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Tests.TestHelpers
{
    public static class UserFaker
    {
        public static User Create(
            Guid? id = null,
            string email = "jan@test.com",
            string password = "correct_password",
            string firstname = "Jan",
            string lastname = "Kowalski")
        {
            return new User
            {
                Id = id ?? Guid.NewGuid(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Firstname = firstname,
                Lastname = lastname
            };
        }
    }
}
