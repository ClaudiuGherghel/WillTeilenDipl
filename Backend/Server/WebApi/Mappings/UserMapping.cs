using Core.Entities;
using WebApi.Controllers;

namespace WebApi.Mappings
{
    public static class UserMapping
    {
        public static User ToEntity(this UserPostDto userDto)
        {
            return new User
            {
                Username = userDto.Username ?? string.Empty,
                PasswordHash = userDto.PasswordHash ?? string.Empty,
                Email = userDto.Email ?? string.Empty,
                FirstName = userDto.FirstName ?? string.Empty,
                LastName = userDto.LastName ?? string.Empty,
                BirthDate = userDto.BirthDate,
                PhoneNumber = userDto.PhoneNumber ?? string.Empty,
                Country = userDto.Country ?? string.Empty,
                PostalCode = userDto.PostalCode ?? string.Empty,
                Place = userDto.Place ?? string.Empty,
                Address = userDto.Address ?? string.Empty,
                Role = userDto.Role
            };
        }


        public static void UpdateEntity(this UserPutDto userDto, User userToPut)
        {
            userToPut.Username = userDto.Username;
            userToPut.PasswordHash = userDto.PasswordHash;
            userToPut.Email = userDto.Email;
            userToPut.FirstName = userDto.FirstName;
            userToPut.LastName = userDto.LastName;
            userToPut.BirthDate = userDto.BirthDate;
            userToPut.PhoneNumber = userDto.PhoneNumber ?? string.Empty;
            userToPut.Country = userDto.Country ?? string.Empty;
            userToPut.PostalCode = userDto.PostalCode ?? string.Empty;
            userToPut.Place = userDto.Place ?? string.Empty;
            userToPut.Address = userDto.Address ?? string.Empty;
            userToPut.Role = userDto.Role;
            userToPut.RowVersion = userDto.RowVersion;
        }

    }
}
