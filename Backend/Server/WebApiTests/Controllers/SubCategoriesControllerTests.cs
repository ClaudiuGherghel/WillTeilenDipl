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
using static WebApi.Dtos.SubCategoryDto;

namespace WebApiTests.Controllers
{
    public class SubCategoriesControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ISubCategoryRepository> _mockSubCategoryRepo;
        private readonly Mock<ICategoryRepository> _mockCategoryRepo;
        private readonly Mock<ILogger<SubCategoriesController>> _mockLogger;
        private readonly SubCategoriesController _controller;

        public SubCategoriesControllerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockSubCategoryRepo = new Mock<ISubCategoryRepository>();
            _mockCategoryRepo = new Mock<ICategoryRepository>();
            _mockLogger = new Mock<ILogger<SubCategoriesController>>();

            _mockUow.Setup(u => u.SubCategoryRepository).Returns(_mockSubCategoryRepo.Object);
            _mockUow.Setup(u => u.CategoryRepository).Returns(_mockCategoryRepo.Object);

            _controller = new SubCategoriesController(_mockUow.Object, _mockLogger.Object);
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
        public async Task Get_ReturnsOk_WithListOfSubCategories()
        {
            var subCategories = new List<SubCategory> { new() { Id = 1, Name = "SubTest" } };
            _mockSubCategoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(subCategories);

            var result = await _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<SubCategory>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetById_NotFound_Returns404()
        {
            _mockSubCategoryRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((SubCategory?)null);

            var result = await _controller.Get(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            var subCategory = new SubCategory { Id = 1, Name = "SubTest" };
            _mockSubCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(subCategory);

            var result = await _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<SubCategory>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task PostByAdmin_CategoryNotFound_Returns404()
        {
            SetAdminUser();
            var dto = new SubCategoryPostDto("SubTest", 1);
            _mockCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Category?)null);

            var result = await _controller.PostByAdmin(dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PostByAdmin_ValidData_ReturnsCreated()
        {
            SetAdminUser();
            var dto = new SubCategoryPostDto("SubTest", 1);
            _mockCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Category { Id = 1 });

            _mockSubCategoryRepo.Setup(r => r.Insert(It.IsAny<SubCategory>()))
                .Callback<SubCategory>(sc => sc.Id = 42);

            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            var result = await _controller.PostByAdmin(dto);

            _mockSubCategoryRepo.Verify(r => r.Insert(It.IsAny<SubCategory>()), Times.Once);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task PutByAdmin_IdMismatch_ReturnsBadRequest()
        {
            SetAdminUser();
            var dto = new SubCategoryPutDto(2, null, "SubTest", 1);

            var result = await _controller.PutByAdmin(1, dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutByAdmin_SubCategoryNotFound_ReturnsNotFound()
        {
            SetAdminUser();
            var dto = new SubCategoryPutDto(1, null, "SubTest", 1);
            _mockSubCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((SubCategory?)null);

            var result = await _controller.PutByAdmin(1, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PutByAdmin_CategoryNotFound_ReturnsNotFound()
        {
            SetAdminUser();
            var dto = new SubCategoryPutDto(1, null, "SubTest", 1);
            _mockSubCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new SubCategory { Id = 1 });
            _mockCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Category?)null);

            var result = await _controller.PutByAdmin(1, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PutByAdmin_ValidData_ReturnsOk()
        {
            SetAdminUser();
            var dto = new SubCategoryPutDto(1, null, "Updated", 1);
            var subCategory = new SubCategory { Id = 1, Name = "Old" };

            _mockSubCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(subCategory);
            _mockCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Category { Id = 1 });

            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            var result = await _controller.PutByAdmin(1, dto);

            _mockSubCategoryRepo.Verify(r => r.Update(It.IsAny<SubCategory>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteByAdmin_NotFound_ReturnsNotFound()
        {
            SetAdminUser();
            _mockSubCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((SubCategory?)null);

            var result = await _controller.DeleteByAdmin(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteByAdmin_Found_ReturnsNoContent()
        {
            SetAdminUser();
            _mockSubCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new SubCategory { Id = 1 });
            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

            var result = await _controller.DeleteByAdmin(1);

            _mockSubCategoryRepo.Verify(r => r.SoftDelete(1), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
