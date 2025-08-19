using Core.Contracts;
using Core.Dtos;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GeoPostalsController(IUnitOfWork uow, ILogger<ItemsController> logger) : BaseController<ItemsController>(uow, logger)
    {

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            GeoPostal? geoPostal = await _uow.GeoPostalRepository.GetByIdAsync(id);

            if (geoPostal is null)
                return NotFound(new { error = $"Kein GeoPostal mit der ID {id} gefunden." });

            return Ok(geoPostal);
        }

        [HttpGet]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries()
        {
            ICollection<string> countries = await _uow.GeoPostalRepository.GetCountriesAsync();

            return Ok(countries);
        }

        [HttpGet]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStates([FromQuery] string country)
        {
            ICollection<string> states = await _uow.GeoPostalRepository.GetStatesAsync(country);

            return Ok(states);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PostalCodeAndPlaceDto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPostalCodesAndPlaces([FromQuery] string state)
        {
            ICollection<PostalCodeAndPlaceDto> postalCodesAndPlaces = await _uow.GeoPostalRepository.GetPostalCodesAndPlacesAsync(state);

            return Ok(postalCodesAndPlaces);
        }


    }
}
