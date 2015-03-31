using StructureMap;

namespace MediatorLib
{
    public class ValidateMediator : IValidateMediator
    {
        private readonly IContainer _container;

        public ValidateMediator(IContainer container)
        {
            _container = container;
        }

        public void Validate<T>(T item)
        {
            ValidateItem(item, true);
        }

        public void TryValidate<T>(T item)
        {
            ValidateItem(item, false);
        }

        private void ValidateItem<T>(T item, bool required)
        {
            var validator = required
                ? _container.GetInstance<IValidateHandler<T>>()
                : _container.TryGetInstance<IValidateHandler<T>>();

            if (validator != null)
            {
                validator.Validate(item);
            }
        }
    }
}