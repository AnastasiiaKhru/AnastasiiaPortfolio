using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AnastasiiaPortfolio.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using AnastasiiaPortfolio.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AnastasiiaPortfolio.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly MongoDBService _mongoDBService;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment, MongoDBService mongoDBService, IConfiguration configuration)
    {
        _logger = logger;
        _environment = environment;
        _mongoDBService = mongoDBService;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        // Create a view model for the home page
        var viewModel = new HomeViewModel
        {
            FeaturedProjects = new List<Project>
            {
                new Project
                {
                    Id = "1",
                    Title = "E-Commerce Platform",
                    Description = "A full-stack e-commerce solution built with ASP.NET Core MVC",
                    ImageUrl = "/images/projects/ecommerce.jpg",
                    Technologies = "ASP.NET Core MVC, SQL Server, Entity Framework, Bootstrap",
                    ProjectUrl = "https://example.com/ecommerce",
                    GitHubUrl = "https://github.com/pewbertable/CrmTechTitans",
                    DateCompleted = DateTime.Now.AddMonths(-3),
                    IsFeatured = true,
                    Category = "Web Application"
                },
                // Add more featured projects as needed
            }
        };
        return View(viewModel);
    }

    public IActionResult Projects() => View();
    public IActionResult Skills() => View();
    public IActionResult Experience() => View();
    public IActionResult Education() => View();
    
    // Demo websites
    public IActionResult ArtStudioDemo() => View();
    public IActionResult CakeShopDemo() => View();
    public IActionResult GymCrmDemo() => View();
    public IActionResult LiquorStoreDemo() => View();
    public IActionResult EventWebinarDemo() => View();
    public IActionResult BrandRefreshDemo() => View();
    public IActionResult AdCreativeDemo() => View();
    public IActionResult WPBusinessDemo() => View();
    public IActionResult WPLifestyleDemo() => View();
    public IActionResult WPWellnessDemo() => View();
    public IActionResult WPConstructionDemo() => View();
    public IActionResult ValentineDemo() => View();
    public IActionResult SkyBridgeITDemo() => View();

    private record DemoFrameMeta(string Title, string UrlLabel);

    private static readonly Dictionary<string, DemoFrameMeta> DemoFramePages = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ArtStudioDemo"] = new("By Alisa Khru — Gallery Catalogue", "alisakhru.gallery — catalogue demo"),
        ["CakeShopDemo"] = new("Sweet Delights Bakery", "sweetdelights.demo - Sweet Delights Bakery"),
        ["GymCrmDemo"] = new("FitHub CRM — Gym Management System", "fithub.crm - Gym Management System"),
        ["LiquorStoreDemo"] = new("LUXE SPIRITS — Premium Alcohol Delivery", "luxespirits.demo - Premium Alcohol Delivery"),
        ["EventWebinarDemo"] = new("Event / Webinar Landing Page", "event.demo - Event / Webinar Landing Page"),
        ["BrandRefreshDemo"] = new("Brand Refresh Case Study", "brand.demo - Brand Refresh Case Study"),
        ["AdCreativeDemo"] = new("Ad Creative / Banner Set", "ads.demo - Ad Creative / Banner Set"),
        ["WPBusinessDemo"] = new("ConsultPro — WordPress Demo", "consultpro.demo - Strategy & Growth"),
        ["WPLifestyleDemo"] = new("WanderLens — WordPress Demo", "wanderlens.demo - Lifestyle Blog"),
        ["WPWellnessDemo"] = new("NurtureWell — WordPress Demo", "nurturewell.demo - Wellness Studio"),
        ["WPConstructionDemo"] = new("Heritage Build — WordPress Demo", "heritagebuild.demo - Construction"),
        ["ValentineDemo"] = new("Valentine Demo", "valentine.demo - Interactive Card"),
        ["SkyBridgeITDemo"] = new("SkyBridge IT Solutions", "skybridgeit.demo - IT Consulting"),
    };

    [HttpGet("/Home/DemoFrame/{name}")]
    public IActionResult DemoFrame(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || !DemoFramePages.TryGetValue(name, out var meta))
            return NotFound();

        ViewBag.ForceActiveAction = "Projects";
        ViewBag.DemoSrc = Url.Action(name)!;
        ViewBag.DemoTitle = meta.Title;
        ViewBag.DemoUrlLabel = meta.UrlLabel;
        ViewData["Title"] = meta.Title;
        return View("DemoFrame");
    }

    public IActionResult Portfolio()
    {
        // TODO: Replace with actual data from database
        var projects = new List<Project>
        {
            new Project
            {
                Id = "1",
                Title = "E-Commerce Platform",
                Description = "A full-stack e-commerce solution built with ASP.NET Core MVC",
                ImageUrl = "/images/projects/ecommerce.jpg",
                Technologies = "ASP.NET Core MVC, SQL Server, Entity Framework, Bootstrap",
                ProjectUrl = "https://example.com/ecommerce",
                GitHubUrl = "https://github.com/pewbertable/CrmTechTitans",
                DateCompleted = DateTime.Now.AddMonths(-3),
                IsFeatured = true,
                Category = "Web Application"
            },
            // Add more sample projects here
        };

        return View(projects);
    }

    public IActionResult Resume()
    {
        var resumePath = Path.Combine(_environment.WebRootPath, "files", "resume.pdf");
        if (!System.IO.File.Exists(resumePath))
        {
            _logger.LogWarning("Resume file not found at: {Path}", resumePath);
        }
        return View();
    }

    public IActionResult DownloadResume()
    {
        var resumePath = Path.Combine(_environment.WebRootPath, "files", "AnastasiiaResume.pdf");
        if (!System.IO.File.Exists(resumePath))
        {
            _logger.LogWarning("Resume file not found at: {Path}", resumePath);
            return NotFound("Resume file not found.");
        }

        // Return file inline for browser preview (no download prompt)
        return PhysicalFile(resumePath, "application/pdf");
    }

    public IActionResult DownloadReferences()
    {
        var referencesPath = Path.Combine(_environment.WebRootPath, "files", "AnastasiiaReference.pdf");
        if (!System.IO.File.Exists(referencesPath))
        {
            _logger.LogWarning("References file not found at: {Path}", referencesPath);
            return NotFound("References file not found.");
        }

        // Return file inline for browser preview (no download prompt)
        return PhysicalFile(referencesPath, "application/pdf");
    }

    [HttpGet]
    public IActionResult Contact()
    {
        return View(new HomeViewModel());
    }

    public async Task<IActionResult> TestMongoDB()
    {
        try
        {
            // Test Projects Collection
            var project = new Project
            {
                Title = "Test Project",
                Description = "Test Description",
                ImageUrl = "/images/test.jpg",
                Category = "Test",
                Technologies = "Test Tech"
            };
            await _mongoDBService.CreateProjectAsync(project);
            var projects = await _mongoDBService.GetProjectsAsync();
            await _mongoDBService.DeleteProjectAsync(project.Id);

            // Test Reviews Collection
            var review = new Review
            {
                Name = "Test User",
                Email = "test@test.com",
                Rating = 5,
                Comment = "Test Comment"
            };
            await _mongoDBService.CreateReviewAsync(review);
            var reviews = await _mongoDBService.GetReviewsAsync();
            await _mongoDBService.DeleteReviewAsync(review.Id);

            return Json(new { success = true, message = "MongoDB connection and operations successful!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    public async Task<IActionResult> TestConnection()
    {
        try
        {
            // Test the connection by listing database names
            var client = _mongoDBService.GetMongoClient();
            var dbList = await client.ListDatabaseNames().ToListAsync();
            return Json(new { success = true, message = "Connection successful", databases = dbList });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MongoDB connection test failed");
            return Json(new { success = false, error = ex.Message });
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
