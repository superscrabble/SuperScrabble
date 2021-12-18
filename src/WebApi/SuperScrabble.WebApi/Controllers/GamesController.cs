using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperScrabble.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperScrabble.WebApi.Controllers
{
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
            var viewModel = this.gamesService.GetSummaryById(id, this.User.Identity.Name);
            
            if(viewModel == null)
            {
                return NotFound();
            }

            return Ok(viewModel);
        }
    }
}
