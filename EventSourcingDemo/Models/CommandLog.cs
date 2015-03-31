using System;

namespace EventSourcingDemo.Models
{
    public class CommandLog
    {
        public int CommandLogId { get; set; }
        public Guid EntityId { get; set; }
        public string Command { get; set; }
        public string Status { get; set; }
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }
    }
}