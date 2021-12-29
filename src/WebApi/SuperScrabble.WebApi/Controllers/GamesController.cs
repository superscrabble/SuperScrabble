namespace SuperScrabble.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using SuperScrabble.Services.Data;
    using SuperScrabble.CustomExceptions.Game;
    using Microsoft.AspNetCore.SignalR;
    using SuperScrabble.WebApi.Hubs;

    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGamesService gamesService;

        public GamesController(IGamesService gamesService)
        {
            this.gamesService = gamesService;
        }

        [Authorize]
        [HttpGet("summary/{id}")]
        public IActionResult GetSummary(string id)
        {
            try
            {
                var viewModel = this.gamesService.GetSummaryById(id, this.User.Identity.Name);
                return this.Ok(viewModel);
            }
            catch (GameNotFoundException)
            {
                return this.NotFound();
            }
        }
    }
}
