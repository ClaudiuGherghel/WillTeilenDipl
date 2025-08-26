using Core.Dtos;
using Core.Entities;

namespace Core.Contracts
{
    public interface IGeoPostalRepository
    {
        Task<int> CountAsync();
        Task<GeoPostal?> GetByIdAsync(int id);
        Task<GeoPostal?> GetByQueryAsync(string country, string state, string postalCode, string place);
        Task<ICollection<string>> GetCountriesAsync();
        Task<ICollection<PostalCodeAndPlaceDto>> GetPostalCodesAndPlacesAsync(string state);
        Task<ICollection<string>> GetStatesAsync(string country);
    }
}