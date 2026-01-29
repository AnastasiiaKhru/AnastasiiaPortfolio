using Microsoft.AspNetCore.Mvc;
using AnastasiiaPortfolio.Models;
using AnastasiiaPortfolio.Services;
using System.Threading.Tasks;

namespace AnastasiiaPortfolio.Controllers
{
    public class PlayZoneController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public PlayZoneController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        public async Task<IActionResult> Index()
        {
            var topScores = await _mongoDBService.GetTopPlayerScoresAsync(3);
            ViewBag.TopScores = topScores;
            return View();
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> SaveScore([FromBody] PlayerScore score)
        {
            if (score == null)
                return Json(new { success = false, error = "Invalid payload" });

            var name = (score.PlayerName ?? "").Trim();
            if (name.Length == 0 || name.Length > 50)
                return Json(new { success = false, error = "Name required (max 50 chars)" });

            if (score.Score < 0 || score.Score > 999999)
                return Json(new { success = false, error = "Score out of range" });

            score.PlayerName = name;
            score.PlayedAt = DateTime.UtcNow;
            await _mongoDBService.CreatePlayerScoreAsync(score);

            var topScores = await _mongoDBService.GetTopPlayerScoresAsync(3);
            return Json(new { success = true, topScores });
        }
    }
} 