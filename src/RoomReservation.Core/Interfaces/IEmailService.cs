using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IEmailService
    {
        Task<Result> SendEmailAsync(EmailMessage message);
        Task<ResultT<string>> GetMessageAsync(string template, Dictionary<string, string> replacements);
    }
}
