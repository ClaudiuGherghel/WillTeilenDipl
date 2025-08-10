using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using WebApi.Controllers;
using WebApi.Dtos;
using Xunit;
using static WebApi.Dtos.RentalDto;

namespace WebApiTests.Controllers
{
    public class RentalsControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IRentalRepository> _mockRentalRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IItemRepository> _mockItemRepo;
        private readonly Mock<ILogger<RentalsController>> _mockLogger;
        private readonly RentalsController _controller;

        public RentalsControllerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockRentalRepo = new Mock<IRentalRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockItemRepo = new Mock<IItemRepository>();
            _mockLogger = new Mock<ILogger<RentalsController>>();

            _mockUow.Setup(u => u.RentalRepository).Returns(_mockRentalRepo.Object);
            _mockUow.Setup(u => u.UserRepository).Returns(_mockUserRepo.Object);
            _mockUow.Setup(u => u.ItemRepository).Returns(_mockItemRepo.Object);

            _controller = new RentalsController(_mockUow.Object, _mockLogger.Object);
        }

        private void SetUser(int userId, bool isAdmin = false)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString())
            };
            if (isAdmin)
                claims.Add(new Claim(ClaimTypes.Role, Roles.Admin.ToString()));

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        // ------------------- GET -------------------

        [Fact]
        public async Task Get_ReturnsOk_WithListOfRentals()
        {
            var rentals = new List<Rental> { new() { Id = 1 } };
            _mockRentalRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(rentals);

            var result = await _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Rental>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetById_NotFound_Returns404()
        {
            _mockRentalRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Rental?)null);

            var result = await _controller.Get(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            var rental = new Rental { Id = 1 };
            _mockRentalRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(rental);

            var result = await _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Rental>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        // ------------------- POST -------------------

        [Fact]
        public async Task PostByUser_UserNotFound_Returns404()
        {
            var dto = new RentalPostDto(DateTime.Now, DateTime.Now, "note", RentalStatus.Active, 1, 2, 3);
            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.RenterId)).ReturnsAsync((User?)null);

            var result = await _controller.PostByUser(dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PostByUser_ItemNotFound_Returns404()
        {
            var dto = new RentalPostDto(DateTime.Now, DateTime.Now, "note", RentalStatus.Active, 1, 2, 3);
            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.RenterId)).ReturnsAsync(new User());
            _mockItemRepo.Setup(r => r.GetByIdAsync(dto.ItemId)).ReturnsAsync((Item?)null);

            var result = await _controller.PostByUser(dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PostByUser_Unauthorized_Returns401()
        {
            var dto = new RentalPostDto(DateTime.Now, DateTime.Now, "note", RentalStatus.Active, 1, 2, 3);
            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.RenterId)).ReturnsAsync(new User());
            _mockItemRepo.Setup(r => r.GetByIdAsync(dto.ItemId)).ReturnsAsync(new Item());

            // HttpContext mit leerem User setzen, damit User != null ist, aber keine Claims hat
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            var result = await _controller.PostByUser(dto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task PostByUser_Forbidden_Returns403()
        {
            var dto = new RentalPostDto(DateTime.Now, DateTime.Now, "note", RentalStatus.Active, 1, 2, 3);
            var item = new Item { OwnerId = 99 };

            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.RenterId)).ReturnsAsync(new User());
            _mockItemRepo.Setup(r => r.GetByIdAsync(dto.ItemId)).ReturnsAsync(item);

            SetUser(1, false); // not owner, not admin

            var result = await _controller.PostByUser(dto);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status403Forbidden, status.StatusCode);
        }

        [Fact]
        public async Task PostByUser_Valid_ReturnsCreated()
        {
            var dto = new RentalPostDto(DateTime.Now, DateTime.Now, "note", RentalStatus.Active, 1, 2, 3);
            var item = new Item { OwnerId = 1 };
            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.RenterId)).ReturnsAsync(new User());
            _mockItemRepo.Setup(r => r.GetByIdAsync(dto.ItemId)).ReturnsAsync(item);

            _mockRentalRepo.Setup(r => r.Insert(It.IsAny<Rental>()))
                .Callback<Rental>(r => r.Id = 42);

            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            SetUser(1, false); // owner

            var result = await _controller.PostByUser(dto);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        // ------------------- PUT -------------------

        [Fact]
        public async Task PutByUser_IdMismatch_ReturnsBadRequest()
        {
            var dto = new RentalPutDto(2, null, DateTime.Now, DateTime.Now, "note", RentalStatus.Active, 1, 2, 3);

            var result = await _controller.PutByUser(1, dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutByUser_RentalNotFound_ReturnsNotFound()
        {
            var dto = new RentalPutDto(1, null, DateTime.Now, DateTime.Now, "note", RentalStatus.Active, 1, 2, 3);
            _mockRentalRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Rental?)null);

            var result = await _controller.PutByUser(1, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PutByUser_UserNotFound_ReturnsNotFound()
        {
            var dto = new RentalPutDto(1, null, DateTime.Now, DateTime.Now, "note", RentalStatus.Active, 1, 2, 3);
            _mockRentalRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Rental());
            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.RenterId)).ReturnsAsync((User?)null);

            var result = await _controller.PutByUser(1, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PutByUser_ItemNotFound_ReturnsNotFound()
        {
            var dto = new RentalPutDto(1, null, DateTime.Now, DateTime.Now, "note", RentalStatus.Active, 1, 2, 3);
            _mockRentalRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Rental());
            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.RenterId)).ReturnsAsync(new User());
            _mockItemRepo.Setup(r => r.GetByIdAsync(dto.ItemId)).ReturnsAsync((Item?)null);

            var result = await _controller.PutByUser(1, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PutByUser_Unauthorized_Returns401()
        {
            var dto = new RentalPutDto(1, null, DateTime.Now, DateTime.Now, "note", RentalStatus.Active, 1, 2, 3);
            _mockRentalRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Rental());
            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.RenterId)).ReturnsAsync(new User());
            _mockItemRepo.Setup(r => r.GetByIdAsync(dto.ItemId)).ReturnsAsync(new Item());

            // HttpContext mit leerem User setzen
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            var result = await _controller.PutByUser(1, dto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task PutByUser_Forbidden_Returns403()
        {
            var dto = new RentalPutDto(1, null, DateTime.Now, DateTime.Now, "note", RentalStatus.Active, 1, 2, 3);
            var item = new Item { OwnerId = 99 };

            _mockRentalRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Rental());
            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.RenterId)).ReturnsAsync(new User());
            _mockItemRepo.Setup(r => r.GetByIdAsync(dto.ItemId)).ReturnsAsync(item);

            SetUser(1, false);

            var result = await _controller.PutByUser(1, dto);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status403Forbidden, status.StatusCode);
        }

        [Fact]
        public async Task PutByUser_Valid_ReturnsOk()
        {
            var dto = new RentalPutDto(1, null, DateTime.Now, DateTime.Now, "note", RentalStatus.Active, 1, 2, 3);
            var rental = new Rental();
            var item = new Item { OwnerId = 1 };

            _mockRentalRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(rental);
            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.RenterId)).ReturnsAsync(new User());
            _mockItemRepo.Setup(r => r.GetByIdAsync(dto.ItemId)).ReturnsAsync(item);

            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            SetUser(1);

            var result = await _controller.PutByUser(1, dto);

            _mockRentalRepo.Verify(r => r.Update(It.IsAny<Rental>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }

        // ------------------- DELETE -------------------

        [Fact]
        public async Task DeleteByUser_NotFound_ReturnsNotFound()
        {
            _mockRentalRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Rental?)null);

            var result = await _controller.DeleteByUser(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteByUser_Unauthorized_Returns401()
        {
            _mockRentalRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Rental());

            // HttpContext mit leerem User setzen
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            var result = await _controller.DeleteByUser(1);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task DeleteByUser_Forbidden_Returns403()
        {
            var rental = new Rental { RenterId = 99 };
            _mockRentalRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(rental);

            SetUser(1, false);

            var result = await _controller.DeleteByUser(1);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status403Forbidden, status.StatusCode);
        }

        [Fact]
        public async Task DeleteByUser_Valid_ReturnsNoContent()
        {
            var rental = new Rental { RenterId = 1 };
            _mockRentalRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(rental);
            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            SetUser(1);

            var result = await _controller.DeleteByUser(1);

            _mockRentalRepo.Verify(r => r.SoftDelete(1), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
