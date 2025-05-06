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
        public async Task<IActionResult> SaveScore([FromBody] PlayerScore score)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false });
            }

            score.PlayedAt = DateTime.UtcNow;
            await _mongoDBService.CreatePlayerScoreAsync(score);

            var topScores = await _mongoDBService.GetTopPlayerScoresAsync(3);
            return Json(new { success = true, topScores });
        }
    }
} 