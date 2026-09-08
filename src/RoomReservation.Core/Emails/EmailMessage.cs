using System.Text.Json;

namespace RoomReservation.Core.Emails
{
    public abstract class EmailMessage
    {
        public required string To { get; init; }

        internal abstract string Subject { get; }
        internal virtual string TemplateName => GetType().Name;
        internal abstract Dictionary<string, string> GetReplacements();

        internal string ToJsonPayload()
        {
            var data = JsonSerializer.Serialize(this, GetType());
            var wrapper = new EmailJobPayload(TemplateName, data);
            return JsonSerializer.Serialize(wrapper);
        }
    }
}
