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
                UserName = userDto.UserName ?? string.Empty,
                PasswordHash = SecurityHelper.HashPassword(userDto.Password ?? string.Empty),
                Email = userDto.Email ?? string.Empty,
                FirstName = userDto.FirstName ?? string.Empty,
                LastName = userDto.LastName ?? string.Empty,
                BirthDate = userDto.BirthDate,
                PhoneNumber = userDto.PhoneNumber ?? string.Empty,
                Address = userDto.Address ?? string.Empty,
                Role = userDto.Role,
                GeoPostalId = userDto.GeoPostaldId,
            };
        }


        public static void UpdateEntity(this UserPutDto userDto, User userToPut)
        {
            userToPut.UpdatedAt = DateTime.UtcNow;
            userToPut.RowVersion = userDto.RowVersion;
            userToPut.UserName = userDto.UserName ?? string.Empty;
            userToPut.Email = userDto.Email ?? string.Empty;
            userToPut.FirstName = userDto.FirstName ?? string.Empty;
            userToPut.LastName = userDto.LastName ?? string.Empty;
            userToPut.BirthDate = userDto.BirthDate;
            userToPut.PhoneNumber = userDto.PhoneNumber ?? string.Empty;
            userToPut.Address = userDto.Address ?? string.Empty;
            userToPut.Role = userDto.Role;
            userToPut.GeoPostalId = userDto.GeoPostaldId;
        }

        public static void ChangePassword(this UserChangePasswordDto userDto, User userToPut)
        {
            userToPut.PasswordHash = SecurityHelper.HashPassword(userDto.NewPassword);
            userToPut.UpdatedAt = DateTime.UtcNow;
            userToPut.RowVersion = userDto.RowVersion;
        }

    }
}
