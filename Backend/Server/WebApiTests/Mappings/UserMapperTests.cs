using Core.Entities;
using Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Mappings;
using static WebApi.Dtos.UserDto;

namespace WebApiTests.Mappings
{
    public class UserMapperTests
    {
        [Fact]
        public void User_ToEntity_MapsCorrectly()
        {
            var dto = new UserPostDto("user", "pw", "email", "fn", "ln", DateTime.UtcNow, 0, "Addr", "", 1);
            var entity = dto.ToEntity();

            Assert.Equal(dto.UserName, entity.UserName);
            Assert.True(SecurityHelper.VerifyPassword(dto.Password, entity.PasswordHash));
            Assert.Equal(dto.Email, entity.Email);
            Assert.Equal(dto.Role, entity.Role);
            Assert.Equal(dto.GeoPostalId, entity.GeoPostalId);
        }

        [Fact]
        public void User_UpdateEntity_MapsCorrectly()
        {
            var entity = new User();
            var dto = new UserPutDto(1, [1, 2], "user", "email", "fn", "ln", DateTime.UtcNow, 0, "Addr", "", 1);
            dto.UpdateEntity(entity);

            Assert.Equal(dto.UserName, entity.UserName);
            Assert.Equal(dto.Email, entity.Email);
            Assert.Equal(dto.FirstName, entity.FirstName);
            Assert.Equal(dto.LastName, entity.LastName);
            Assert.Equal(dto.BirthDate, entity.BirthDate);
            Assert.Equal(dto.PhoneNumber, entity.PhoneNumber);
            Assert.Equal(dto.Address, entity.Address);
            Assert.Equal(dto.Role, entity.Role);
            Assert.Equal(dto.RowVersion, entity.RowVersion);
            Assert.Equal(dto.GeoPostalId, entity.GeoPostalId);
        }

        [Fact]
        public void User_ChangePassword_UpdatesPassword()
        {
            var entity = new User();
            var newPassword = "newPassword123";
            var dto = new UserChangePasswordDto(1, [1, 2], newPassword);

            dto.ChangePassword(entity);

            Assert.True(SecurityHelper.VerifyPassword(newPassword, entity.PasswordHash));
            Assert.Equal(dto.RowVersion, entity.RowVersion);
        }
    }
}
