namespace Core.Contracts.Validations
{
    public interface IEntityValidator
    {
        Task ValidateAsync(object entity, bool checkMemory = false);
    }
}
