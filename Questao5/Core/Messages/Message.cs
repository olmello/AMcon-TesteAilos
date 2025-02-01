using System.Text.Json.Serialization;

namespace Questao5.Core.Messages
{
    public abstract class Message
    {
        [JsonIgnore]
        public string MessageType { get; protected set; }

        [JsonIgnore]
        public Guid AggregateId { get; protected set; }

        public Message()
        {
            MessageType = GetType().Name;
        }
    }
}
