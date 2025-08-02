using Core.Entities;

namespace Core.Contracts
{
    public interface IRentalRepository
    {
        Task<int> CountAsync();
        void Delete(Rental rentalToRemove);
        Task<ICollection<Rental>> GetAllAsync();
        Task<Rental?> GetByIdAsync(int id);
        void Insert(Rental rentalToPost);
        void SoftDelete(int id);
        void Update(Rental rentalToPut);
    }
}