using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperScrabble.Common.Exceptions.Game;
using SuperScrabble.Services.Data.Games;

namespace SuperScrabble.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly IGamesService _gamesService;

    public GamesController(IGamesService gamesService)
    {
        _gamesService = gamesService;
    }

    [Authorize]
    [HttpGet("summary/{id}")]
    public IActionResult GetSummary(string id)
    {
        try
        {
            var viewModel = _gamesService.GetSummaryById(id, User.Identity?.Name!);
            return Ok(viewModel);
        }
        catch (GameNotFoundException)
        {
            return NotFound();
        }
    }
}
