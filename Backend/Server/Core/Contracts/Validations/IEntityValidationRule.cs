namespace Core.Contracts.Validations
{
    public interface IEntityValidationRule
    {
        Task ValidateAsync(object entity, bool checkMemory = false);
    }
}
