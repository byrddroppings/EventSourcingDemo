namespace EventSourcingDemo.Models
{
    public class ViewModel
    {
        public Person[] People { get; set; }

        public Person Person { get; set; }
        public CommandLog[] Commands { get; set; }
        public CommandLog[] PendingCommands { get; set; }
    }
}