using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using WebApi.Controllers;
using Xunit;
using static WebApi.Dtos.ItemDto;

namespace WebApiTests.Controllers
{
    public class ItemsControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IItemRepository> _mockItemRepo;
        private readonly Mock<ISubCategoryRepository> _mockSubCategoryRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ILogger<ItemsController>> _mockLogger;
        private readonly ItemsController _controller;

        public ItemsControllerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockItemRepo = new Mock<IItemRepository>();
            _mockSubCategoryRepo = new Mock<ISubCategoryRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<ItemsController>>();

            // UnitOfWork -> Mock-Repositories zuweisen
            _mockUow.Setup(u => u.ItemRepository).Returns(_mockItemRepo.Object);
            _mockUow.Setup(u => u.SubCategoryRepository).Returns(_mockSubCategoryRepo.Object);
            _mockUow.Setup(u => u.UserRepository).Returns(_mockUserRepo.Object);

            _controller = new ItemsController(_mockUow.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Get_ReturnsOk_WithListOfItems()
        {
            var items = new List<Item> { new() { Id = 1, Name = "Item1" } };
            _mockItemRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

            var result = await _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Item>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetById_NotFound_Returns404()
        {
            _mockItemRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Item?)null);

            var result = await _controller.Get(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            var item = new Item { Id = 1, Name = "Item1" };
            _mockItemRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

            var result = await _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Item>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task GetByFilter_ReturnsOk()
        {
            var items = new List<Item> { new() { Id = 1 } };
            _mockItemRepo.Setup(r => r.GetFilteredAsync("filter")).ReturnsAsync(items);

            var result = await _controller.GetByFilter("filter");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Item>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task PostByUser_SubCategoryNotFound_Returns404()
        {
            var dto = new ItemPostDto("Name", "Desc", true, "Street", 10, 5, 1, 0, 0, 1, 2, 1);
            _mockSubCategoryRepo.Setup(r => r.GetByIdAsync(dto.SubCategoryId)).ReturnsAsync((SubCategory?)null);

            var result = await _controller.PostByUser(dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PostByUser_UserNotFound_Returns404()
        {
            var dto = new ItemPostDto("Name", "Desc", true, "Street", 10, 5, 1, 0, 0, 1, 2, 1);
            _mockSubCategoryRepo.Setup(r => r.GetByIdAsync(dto.SubCategoryId)).ReturnsAsync(new SubCategory { Id = 1 });
            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.OwnerId)).ReturnsAsync((User?)null);

            var result = await _controller.PostByUser(dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PostByUser_Unauthorized_Returns401()
        {
            var dto = new ItemPostDto("Name", "Desc", true, "Street", 10, 5, 1, 0, 0, 1, 2, 1);


            _mockSubCategoryRepo.Setup(r => r.GetByIdAsync(dto.SubCategoryId))
                .ReturnsAsync(new SubCategory { Id = dto.SubCategoryId });

            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.OwnerId))
                .ReturnsAsync(new User { Id = dto.OwnerId });

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            var result = await _controller.PostByUser(dto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

            [Fact]
        public async Task PutByUser_IdMismatch_ReturnsBadRequest()
        {
            var dto = new ItemPutDto(2, null, "Name", "Desc", true, "Street", 10, 5, 1, 0, 0, 1, 2, 1);

            var result = await _controller.PutByUser(1, dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutByUser_ItemNotFound_Returns404()
        {
            var dto = new ItemPutDto(1, null, "Name", "Desc", true, "Street", 10, 5, 1, 0, 0, 1, 2, 1);
            _mockItemRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Item?)null);

            var result = await _controller.PutByUser(1, dto);

            Assert.IsType<NotFoundObjectResult>(result);

        }

        [Fact]
        public async Task PutByUser_Unauthorized_Returns401()
        {
            var dto = new ItemPutDto(2, null, "Name", "Desc", true, "Street", 10, 5, 1, 0, 0, 1, 2, 1);


            var existingItem = new Item { Id = dto.Id };

            _mockItemRepo.Setup(r => r.GetByIdAsync(dto.Id))
                .ReturnsAsync(existingItem);

            _mockSubCategoryRepo.Setup(r => r.GetByIdAsync(dto.SubCategoryId))
                .ReturnsAsync(new SubCategory { Id = dto.SubCategoryId });

            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.OwnerId))
                .ReturnsAsync(new User { Id = dto.OwnerId });

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            var result = await _controller.PutByUser(dto.Id, dto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task DeleteByUser_ItemNotFound_Returns404()
        {
            _mockItemRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Item?)null);

            var result = await _controller.DeleteByUser(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteByUser_Unauthorized_Returns401()
        {
            _mockItemRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Item { Id = 1 });

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    // Simuliert eingeloggten Benutzer ohne Claims → GetUserIdFromClaims() gibt null zurück
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            var result = await _controller.DeleteByUser(1);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }


        [Fact]
        public async Task DeleteByUser_Found_ReturnsNoContent()
        {
            var item = new Item { Id = 1, OwnerId = 1 };
            _mockItemRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            // Simuliere angemeldeten User mit OwnerId 1
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            var result = await _controller.DeleteByUser(1);

            _mockItemRepo.Verify(r => r.SoftDelete(1), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PostByUser_OwnerCanAdd_ReturnsCreated()
        {
            var dto = new ItemPostDto("Name", "Desc", true, "Street", 10, 5, 1, 0, 0, 1, 2, 1);


            _mockSubCategoryRepo.Setup(r => r.GetByIdAsync(dto.SubCategoryId))
                .ReturnsAsync(new SubCategory { Id = dto.SubCategoryId });
            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.OwnerId))
                .ReturnsAsync(new User { Id = dto.OwnerId });

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, dto.OwnerId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            var result = await _controller.PostByUser(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<Item>(createdResult.Value);
            Assert.Equal(dto.Name, returnValue.Name);
            _mockItemRepo.Verify(r => r.Insert(It.IsAny<Item>()), Times.Once);
        }

        [Fact]
        public async Task PutByUser_OwnerCanUpdate_ReturnsOk()
        {
            var dto = new ItemPutDto(2, null, "Name", "Desc", true, "Street", 10, 5, 1, 0, 0, 1, 2, 1);


            var existingItem = new Item { Id = 1, OwnerId = 2 };
            _mockItemRepo.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(existingItem);
            _mockSubCategoryRepo.Setup(r => r.GetByIdAsync(dto.SubCategoryId))
                .ReturnsAsync(new SubCategory { Id = dto.SubCategoryId });
            _mockUserRepo.Setup(r => r.GetByIdAsync(dto.OwnerId))
                .ReturnsAsync(new User { Id = dto.OwnerId });

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, dto.OwnerId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            var result = await _controller.PutByUser(dto.Id, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ItemPutDto>(okResult.Value);
            Assert.Equal(dto.Name, returnValue.Name);
            _mockItemRepo.Verify(r => r.Update(existingItem), Times.Once);
        }

    }
}
