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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please fill in all required fields correctly." });
            }

            try
            {
                await _mongoDBService.CreateReviewAsync(review);
                
                // Return the partial view for the new review
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while saving your review. Please try again." });
            }
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