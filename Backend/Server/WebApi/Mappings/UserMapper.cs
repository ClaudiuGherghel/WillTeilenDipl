using Core.Entities;
using WebApi.Controllers;
using Core.Helper;
using WebApi.Dtos;
using static WebApi.Dtos.UserDto;

namespace WebApi.Mappings
{
    public static class UserMapper
    {


        public static User ToEntity(this UserPostDto userDto)
        {
            return new User
            {
                CreatedAt = DateTime.UtcNow,
                Username = userDto.Username ?? string.Empty,
                PasswordHash = SecurityHelper.HashPassword(userDto.Password ?? string.Empty),
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
            userToPut.UpdatedAt = DateTime.UtcNow;
            userToPut.RowVersion = userDto.RowVersion;
            userToPut.Username = userDto.Username ?? string.Empty;
            userToPut.Email = userDto.Email ?? string.Empty;
            userToPut.FirstName = userDto.FirstName ?? string.Empty;
            userToPut.LastName = userDto.LastName ?? string.Empty;
            userToPut.BirthDate = userDto.BirthDate;
            userToPut.PhoneNumber = userDto.PhoneNumber ?? string.Empty;
            userToPut.Country = userDto.Country ?? string.Empty;
            userToPut.PostalCode = userDto.PostalCode ?? string.Empty;
            userToPut.Place = userDto.Place ?? string.Empty;
            userToPut.Address = userDto.Address ?? string.Empty;
            userToPut.Role = userDto.Role;
        }

        public static void ChangePassword(this UserChangePasswordDto userDto, User userToPut)
        {
            userToPut.PasswordHash = SecurityHelper.HashPassword(userDto.NewPassword);
            userToPut.UpdatedAt = DateTime.UtcNow;
            userToPut.RowVersion = userDto.RowVersion;
        }

    }
}
