using Microsoft.AspNetCore.Mvc;
using Spot_The_Difference.Contracts.Requests;
using Spot_The_Difference.Domain.Services;

namespace Spot_The_Difference.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameService _gameService;
        private readonly LevelService _levelService;

        public GameController(GameService gameService, LevelService levelService)
        {
            _gameService = gameService;
            _levelService = levelService;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] StartGameRequest request)
        {
            var response = await _gameService.StartNewGameAsync(request);

            if (response == null)
            {
                return NotFound(new { message = "geen meer levels" });
            }

            return Ok(response);
        }

        [HttpPost("guess")]
        public async Task<IActionResult> MakeGuess([FromBody] GuessRequest request)
        {
            var result = await _gameService.ProcessGuessAsync(request);

            if (result == null) return NotFound("Spelsessie niet gevonden");

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLevel(int id, [FromQuery] string language = "en")
        {
            var level = await _levelService.GetLevel(id, language);
            if (level == null) return NotFound();
            return Ok(level);
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var results = await _gameService.GetLeaderboardAsync();
            return Ok(results);
        }
    }
}