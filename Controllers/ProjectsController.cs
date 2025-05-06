using Microsoft.AspNetCore.Mvc;
using AnastasiiaPortfolio.Models;
using AnastasiiaPortfolio.Services;

namespace AnastasiiaPortfolio.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public ProjectsController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var projects = await _mongoDBService.GetProjectsAsync();
            return View(projects);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _mongoDBService.GetProjectAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,ImageUrl,ProjectUrl,GitHubUrl,Category,Technologies,IsFeatured,DateCompleted")] Project project)
        {
            if (ModelState.IsValid)
            {
                await _mongoDBService.CreateProjectAsync(project);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _mongoDBService.GetProjectAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Description,ImageUrl,ProjectUrl,GitHubUrl,Category,Technologies,IsFeatured,DateCompleted")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _mongoDBService.UpdateProjectAsync(id, project);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _mongoDBService.GetProjectAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _mongoDBService.DeleteProjectAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
} 