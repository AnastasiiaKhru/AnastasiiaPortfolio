using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AnastasiiaPortfolio.Models;
using AnastasiiaPortfolio.Services;

namespace AnastasiiaPortfolio.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public ReviewsController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // GET: Reviews
        public async Task<IActionResult> Index(ReviewSortOption sortOption = ReviewSortOption.Newest)
        {
            var reviews = await _mongoDBService.GetReviewsAsync(sortOption);
            return View(reviews);
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _mongoDBService.GetReviewAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Rating,Comment")] Review review)
        {
            if (ModelState.IsValid)
            {
                await _mongoDBService.CreateReviewAsync(review);
                return RedirectToAction(nameof(Index));
            }
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _mongoDBService.GetReviewAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Email,Rating,Comment,IsHidden")] Review review)
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

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _mongoDBService.GetReviewAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _mongoDBService.DeleteReviewAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleVerification(string id)
        {
            await _mongoDBService.ToggleReviewVerificationAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFeatured(string id)
        {
            await _mongoDBService.ToggleReviewFeaturedAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateHelpfulCount(string id, bool isHelpful)
        {
            await _mongoDBService.UpdateReviewHelpfulCountAsync(id, isHelpful);
            return RedirectToAction(nameof(Index));
        }
    }
} 