namespace MediatorLib
{
    public interface IValidateHandler<in T>
    {
        void Validate(T item);
    }
}