using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using WebApi.Controllers;
using static WebApi.Dtos.CategoryDto;

namespace WebApiTests.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ICategoryRepository> _mockCategoryRepo;
        private readonly Mock<ILogger<CategoriesController>> _mockLogger;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockCategoryRepo = new Mock<ICategoryRepository>();
            _mockLogger = new Mock<ILogger<CategoriesController>>();

            _mockUow.Setup(u => u.CategoryRepository).Returns(_mockCategoryRepo.Object);

            _controller = new CategoriesController(_mockUow.Object, _mockLogger.Object);
        }

        private void SetAdminUser()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Role, Roles.Admin.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }


        [Fact]
        public async Task Get_ReturnsOkResult_WithListOfCategories()
        {
            var categories = new List<Category> { new() { Id = 1, Name = "Test" } };
            _mockCategoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            var result = await _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Category>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetById_UnknownId_ReturnsNotFound()
        {
            _mockCategoryRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Category?)null);

            var result = await _controller.Get(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsCategory()
        {
            var category = new Category { Id = 1, Name = "Test" };
            _mockCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

            var result = await _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Category>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task PostByAdmin_AsAdmin_ReturnsCreatedAtAction()
        {
            SetAdminUser();
            var categoryDto = new CategoryPostDto("TestCategory");

            _mockCategoryRepo.Setup(r => r.Insert(It.IsAny<Category>()))
                .Callback<Category>(c => c.Id = 42);
            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            var result = await _controller.PostByAdmin(categoryDto);

            _mockCategoryRepo.Verify(r => r.Insert(It.IsAny<Category>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(false), Times.Once);
            Assert.IsType<CreatedAtActionResult>(result);
        }



        [Fact]
        public async Task PutByAdmin_IdMismatch_ReturnsBadRequest()
        {
            SetAdminUser();
            var dto = new CategoryPutDto(2, null, "Test");

            var result = await _controller.PutByAdmin(1, dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutByAdmin_NotFound_ReturnsNotFound()
        {
            SetAdminUser();
            _mockCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Category?)null);
            var dto = new CategoryPutDto(1, null, "Test");

            var result = await _controller.PutByAdmin(1, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteByAdmin_NotFound_ReturnsNotFound()
        {
            SetAdminUser();
            _mockCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Category?)null);

            var result = await _controller.DeleteByAdmin(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteByAdmin_Found_ReturnsNoContent()
        {
            SetAdminUser();
            _mockCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Category { Id = 1 });

            var result = await _controller.DeleteByAdmin(1);

            _mockUow.Verify(u => u.SaveChangesAsync(false), Times.Once);
            _mockCategoryRepo.Verify(r => r.SoftDelete(1), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
