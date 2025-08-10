using Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController<T>(IUnitOfWork uow, ILogger<T> logger) : ControllerBase
    {
        protected readonly IUnitOfWork _uow = uow;
        protected readonly ILogger<T> _logger = logger;

        protected int? GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out int userId) ? userId : null;
        }
    }
}
