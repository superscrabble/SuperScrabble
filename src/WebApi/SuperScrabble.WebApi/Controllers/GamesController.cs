namespace SuperScrabble.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using SuperScrabble.Services.Data;

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
            catch (System.Exception)
            {
                return this.NotFound();
            }
        }
    }
}
