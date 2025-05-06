using Microsoft.AspNetCore.Mvc;
using AnastasiiaPortfolio.Models;
using AnastasiiaPortfolio.Services;

namespace AnastasiiaPortfolio.Controllers
{
    public class RateController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public RateController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        public async Task<IActionResult> Index()
        {
            var reviews = await _mongoDBService.GetReviewsAsync();
            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Review review)
        {
            if (ModelState.IsValid)
            {
                await _mongoDBService.CreateReviewAsync(review);
                return RedirectToAction(nameof(Index));
            }
            return View(review);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var review = await _mongoDBService.GetReviewAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _mongoDBService.UpdateReviewAsync(id, review);
                return RedirectToAction(nameof(Index));
            }
            return View(review);
        }

        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteReviewAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
} 