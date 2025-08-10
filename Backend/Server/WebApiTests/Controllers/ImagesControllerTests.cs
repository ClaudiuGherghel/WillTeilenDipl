using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using WebApi.Controllers;
using Xunit;
using static WebApi.Dtos.ImageDto;

namespace WebApiTests.Controllers
{
    public class ImagesControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IImageRepository> _mockImageRepo;
        private readonly Mock<IItemRepository> _mockItemRepo;
        private readonly Mock<ILogger<ImagesController>> _mockLogger;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly ImagesController _controller;

        public ImagesControllerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockImageRepo = new Mock<IImageRepository>();
            _mockItemRepo = new Mock<IItemRepository>();
            _mockLogger = new Mock<ILogger<ImagesController>>();
            _mockEnv = new Mock<IWebHostEnvironment>();

            _mockUow.Setup(u => u.ImageRepository).Returns(_mockImageRepo.Object);
            _mockUow.Setup(u => u.ItemRepository).Returns(_mockItemRepo.Object);

            _mockEnv.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            _controller = new ImagesController(_mockUow.Object, _mockLogger.Object, _mockEnv.Object);
        }

        [Fact]
        public async Task Get_ReturnsOk_WithListOfImages()
        {
            var images = new List<Image> { new() { Id = 1 } };
            _mockImageRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(images);

            var result = await _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Image>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetById_NotFound_Returns404()
        {
            _mockImageRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Image?)null);

            var result = await _controller.Get(42);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            var image = new Image { Id = 1 };
            _mockImageRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(image);

            var result = await _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(image, okResult.Value);
        }

        [Fact]
        public async Task PostByUser_NoFile_ReturnsBadRequest()
        {
            var dto = new ImagePostDto("https://test.com/img.jpg", "alt", "image/jpeg", 1, true, 1);

            var result = await _controller.PostByUser(null!, dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PostByUser_InvalidFileType_ReturnsBadRequest()
        {
            var dto = new ImagePostDto("https://test.com/img.jpg", "alt", "image/jpeg", 1, true, 1);
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.ContentType).Returns("text/plain");

            var result = await _controller.PostByUser(fileMock.Object, dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PostByUser_Unauthorized_Returns401()
        {
            var dto = new ImagePostDto("https://test.com/img.jpg", "alt", "image/jpeg", 1, true, 1);
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            var result = await _controller.PostByUser(fileMock.Object, dto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task PutByUser_IdMismatch_ReturnsBadRequest()
        {
            var dto = new ImagePutDto(2, null, "https://test.com/img.jpg", "alt", "image/jpeg", 1, true, 1);

            var result = await _controller.PutByUser(1, dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutByUser_ImageNotFound_Returns404()
        {
            var dto = new ImagePutDto(1, null, "https://test.com/img.jpg", "alt", "image/jpeg", 1, true, 1);
            _mockImageRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Image?)null);

            var result = await _controller.PutByUser(1, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteByUser_ImageNotFound_Returns404()
        {
            _mockImageRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Image?)null);

            var result = await _controller.DeleteByUser(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteByUser_ItemNotFound_Returns404()
        {
            var img = new Image { Id = 1, ItemId = 5 };
            _mockImageRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(img);
            _mockItemRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Item?)null);

            var result = await _controller.DeleteByUser(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteByUser_Unauthorized_Returns401()
        {
            var img = new Image { Id = 1, ItemId = 5 };
            var item = new Item { Id = 5 };

            _mockImageRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(img);
            _mockItemRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(item);

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
    }
}
