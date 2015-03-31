namespace EventSourcingDemo.Features
{
    public class Rollback<T>
    {
        public Rollback()
        {            
        }

        public Rollback(T before, T after)
        {
            Before = before;
            After = after;
        }

        public T Before { get; set; }
        public T After { get; set; }
    }
}
