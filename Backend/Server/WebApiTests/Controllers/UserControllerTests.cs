using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Core.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using WebApi.Controllers;
using WebApi.Dtos;
using Xunit;
using static WebApi.Dtos.UserDto;

namespace WebApiTests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ILogger<UsersController>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<UsersController>>();
            _mockConfig = new Mock<IConfiguration>();
            _mockEnv = new Mock<IWebHostEnvironment>();

            _mockUow.Setup(u => u.UserRepository).Returns(_mockUserRepo.Object);

            var jwtSecret = SecurityHelper.GenerateJwtSecretBase64();

            // JWT Settings für Token-Erzeugung
            _mockConfig.Setup(c => c["JwtSettings:Secret"]).Returns(jwtSecret);
            _mockConfig.Setup(c => c["JwtSettings:Issuer"]).Returns("TestIssuer");
            _mockConfig.Setup(c => c["JwtSettings:Audience"]).Returns("TestAudience");
            _mockConfig.Setup(c => c["JwtSettings:ExpirationMinutes"]).Returns("60");

            _controller = new UsersController(_mockUow.Object, _mockLogger.Object, _mockConfig.Object, _mockEnv.Object);
        }

        [Fact]
        public async Task GetByAdmin_ReturnsOk_WithUsers()
        {
            var users = new List<User> { new() { Id = 1, UserName = "User1" } };
            _mockUserRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            var result = await _controller.GetByAdmin();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetByAdmin_IdNotFound_Returns404()
        {
            _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

            var result = await _controller.GetByAdmin();

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetByAdmin_IdFound_ReturnsOk()
        {
            var user = new User { Id = 1, UserName = "User1" };
            _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            var result = await _controller.GetByAdmin();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<User>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task PostByAdmin_ReturnsCreated()
        {
            var dto = new UserPostDto("User1", "pass", "mail@mail.com", "First", "Last", DateTime.Now.AddYears(-20), Roles.User, "Street", "123456", 1);
            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            var result = await _controller.PostByAdmin(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<User>(createdResult.Value);
            Assert.Equal(dto.UserName, returnValue.UserName);
        }

        [Fact]
        public async Task Register_ReturnsToken()
        {
            var dto = new UserPostDto("User1", "pass", "mail@mail.com", "First", "Last", DateTime.Now.AddYears(-20), Roles.User, "Street", "123456", 1);

            // Insert + SaveChanges werden vom Controller aufgerufen -> Insert setzt Id für besseren Token-Inhalt
            _mockUserRepo.Setup(r => r.Insert(It.IsAny<User>()))
                .Callback<User>(u => u.Id = 7);

            var result = await _controller.Register(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var tokenObj = Assert.IsType<AuthResponse>(okResult.Value);
            Assert.False(string.IsNullOrWhiteSpace(tokenObj.Token));
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            _mockUserRepo.Setup(r => r.AuthenticateAsync("user", "pass")).ReturnsAsync((User?)null);

            var result = await _controller.Login(new LoginRequest("user", "pass"));

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Erzeuge echten PBKDF2-Hash wie in Produktion
            var hashedPassword = SecurityHelper.HashPassword("pass");
            var user = new User { Id = 1, UserName = "user", Role = Roles.User, PasswordHash = hashedPassword };

            // Mock der AuthenticateAsync implementiert die Passwortprüfung (wie echte Repo)
            _mockUserRepo.Setup(r => r.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string username, string password) =>
                    Task.FromResult(username == "user" && SecurityHelper.VerifyPassword(password, hashedPassword) ? user : null)
                );

            var result = await _controller.Login(new LoginRequest("user", "pass"));

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task UpdateByUser_IdMismatch_ReturnsBadRequest()
        {
            var dto = new UserPutDto(2, null, "User", "mail@mail.com", "First", "Last", DateTime.Now.AddYears(-20), Roles.User, "Street", "123456", 1);

            var result = await _controller.UpdateByUser(1, dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateByUser_UserNotFound_Returns404()
        {
            var dto = new UserPutDto(1, null, "User", "mail@mail.com", "First", "Last", DateTime.Now.AddYears(-20), Roles.User, "Street", "123456", 1);
            _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

            var result = await _controller.UpdateByUser(1, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateByUser_Unauthorized_Returns401()
        {
            var dto = new UserPutDto(1, null, "User", "mail@mail.com", "First", "Last", DateTime.Now.AddYears(-20), Roles.User, "Street", "123456", 1);
            _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1 });

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            var result = await _controller.UpdateByUser(1, dto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task UpdateByUser_Forbidden_Returns403()
        {
            var dto = new UserPutDto(1, null, "User", "mail@mail.com", "First", "Last", DateTime.Now.AddYears(-20), Roles.User, "Street", "123456", 1);
            _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1 });

            // Eingeloggt als anderer User
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "2") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            var result = await _controller.UpdateByUser(1, dto);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status403Forbidden, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateByUser_OwnerCanUpdate_ReturnsOk()
        {
            var dto = new UserPutDto(1, null, "User", "mail@mail.com", "First", "Last", DateTime.Now.AddYears(-20), Roles.User, "Street", "123456", 1);
            var user = new User { Id = 1 };
            _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            var result = await _controller.UpdateByUser(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<User>(okResult.Value);
            Assert.Equal(dto.UserName, returnValue.UserName);
        }

        [Fact]
        public async Task DeleteByUser_UserNotFound_Returns404()
        {
            _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

            var result = await _controller.DeleteByUser(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteByUser_Unauthorized_Returns401()
        {
            _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1 });

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            var result = await _controller.DeleteByUser(1);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task DeleteByUser_Forbidden_Returns403()
        {
            _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1 });

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "2") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            var result = await _controller.DeleteByUser(1);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status403Forbidden, statusResult.StatusCode);
        }

        [Fact]
        public async Task DeleteByUser_OwnerCanDelete_ReturnsNoContent()
        {
            var user = new User { Id = 1 };
            _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            var result = await _controller.DeleteByUser(1);

            _mockUserRepo.Verify(r => r.SoftDelete(1), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
