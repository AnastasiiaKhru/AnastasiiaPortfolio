using AnastasiiaPortfolio.Models;
using AnastasiiaPortfolio.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Listen on 0.0.0.0 and PORT (Railway injects PORT at runtime).
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add environment variables configuration
builder.Configuration.AddEnvironmentVariables(prefix: "AnastasiiaPortfolio_");
// User Secrets (optional): dotnet user-secrets set "EmailSettings:SmtpPassword" "your-gmail-app-password"
builder.Configuration.AddUserSecrets<Program>(optional: true);

// Reduce DataProtection warnings in Production (ephemeral container filesystem).
// Keys live in /root/.aspnet/DataProtection-Keys; they are lost on redeploy. Fine for this app.
if (!builder.Environment.IsDevelopment())
	builder.Logging.AddFilter("Microsoft.AspNetCore.DataProtection", LogLevel.Error);

// Ensure MongoDB config in Production (Railway) — avoid cryptic crashes.
if (builder.Environment.IsProduction())
{
	var conn = builder.Configuration["MongoDB:ConnectionString"]?.Trim();
	if (string.IsNullOrEmpty(conn))
		throw new InvalidOperationException(
			"MongoDB:ConnectionString is missing. Set MongoDB__ConnectionString (and MongoDB__DatabaseName, etc.) in Railway → Variables. See RAILWAY_DEPLOY.md.");
}

// Add services to the container.
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDBService>();

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireUppercase = true;
	options.Password.RequireNonAlphanumeric = true;
	options.Password.RequiredLength = 8;
})
.AddDefaultTokenProviders();

// Add custom stores for MongoDB
builder.Services.AddScoped<IUserStore<ApplicationUser>, MongoUserStore>();
builder.Services.AddScoped<IRoleStore<IdentityRole>, MongoRoleStore>();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
	// Trust X-Forwarded-* from Railway’s reverse proxy (scheme, host, etc.).
	var fwd = new ForwardedHeadersOptions
	{
		ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
	};
	fwd.KnownNetworks.Clear();
	fwd.KnownProxies.Clear();
	app.UseForwardedHeaders(fwd);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.Use(async (context, next) =>
{
	if (context.Request.Path == "/" || context.Request.Path.Value == "/")
	{
		var ua = context.Request.Headers.UserAgent.ToString();
		if (ua.Contains("LinkedInBot", StringComparison.OrdinalIgnoreCase) ||
		    ua.Contains("facebookexternalhit", StringComparison.OrdinalIgnoreCase) ||
		    ua.Contains("Twitterbot", StringComparison.OrdinalIgnoreCase) ||
		    ua.Contains("Slurp", StringComparison.OrdinalIgnoreCase) ||
		    ua.Contains("WhatsApp", StringComparison.OrdinalIgnoreCase))
		{
			context.Response.Redirect("/Home/Index", permanent: false);
			return;
		}
	}
	await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok("ok"));

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
