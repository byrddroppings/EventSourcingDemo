namespace MediatorLib
{
    public interface IValidateMediator
    {
        void Validate<T>(T item);
        void TryValidate<T>(T item);
    }
}