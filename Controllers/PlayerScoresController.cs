using Microsoft.AspNetCore.Mvc;
using AnastasiiaPortfolio.Models;
using AnastasiiaPortfolio.Services;

namespace AnastasiiaPortfolio.Controllers
{
    public class PlayerScoresController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public PlayerScoresController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // GET: PlayerScores
        public async Task<IActionResult> Index()
        {
            var scores = await _mongoDBService.GetPlayerScoresAsync();
            return View(scores);
        }

        // GET: PlayerScores/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playerScore = await _mongoDBService.GetPlayerScoreAsync(id);
            if (playerScore == null)
            {
                return NotFound();
            }

            return View(playerScore);
        }

        // GET: PlayerScores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PlayerScores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlayerName,Score")] PlayerScore playerScoreModel)
        {
            if (ModelState.IsValid)
            {
                await _mongoDBService.CreatePlayerScoreAsync(playerScoreModel);
                return RedirectToAction(nameof(Index));
            }
            return View(playerScoreModel);
        }

        // GET: PlayerScores/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playerScore = await _mongoDBService.GetPlayerScoreAsync(id);
            if (playerScore == null)
            {
                return NotFound();
            }
            return View(playerScore);
        }

        // POST: PlayerScores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,PlayerName,Score")] PlayerScore playerScoreModel)
        {
            if (id != playerScoreModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _mongoDBService.UpdatePlayerScoreAsync(id, playerScoreModel);
                return RedirectToAction(nameof(Index));
            }
            return View(playerScoreModel);
        }

        // GET: PlayerScores/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playerScore = await _mongoDBService.GetPlayerScoreAsync(id);
            if (playerScore == null)
            {
                return NotFound();
            }

            return View(playerScore);
        }

        // POST: PlayerScores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _mongoDBService.DeletePlayerScoreAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
} 