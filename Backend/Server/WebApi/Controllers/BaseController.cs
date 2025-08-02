using Core.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    //Kein if (ModelState.IsValid) mehr nötig, bei ApiController passiert das automatisch, (Fehlerausgabe ProblemDetails) 
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController(IUnitOfWork uow, ILogger<ItemsController> logger) : ControllerBase
    {
        // Bei Fehlern wird Middleware einspringen

        internal readonly IUnitOfWork _uow = uow;
        internal readonly ILogger<ItemsController> _logger = logger;

        protected int? GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out int userId) ? userId : null;
        }
    }
}
