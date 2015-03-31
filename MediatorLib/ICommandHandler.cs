namespace MediatorLib
{
    public interface ICommandHandler<in T>
    {
        void Execute(T command);
    }
}