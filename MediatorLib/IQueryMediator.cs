namespace MediatorLib
{
    public interface IQueryMediator
    {
        T Query<T>(IQuery<T> request);
    }
}