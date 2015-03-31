namespace MediatorLib
{
    public interface ICommandMediator
    {
        void Execute<T>(T command);
        void TryExecute<T>(T command);
    }
}