using AnastasiiaPortfolio.Models;
using AnastasiiaPortfolio.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add environment variables configuration
builder.Configuration.AddEnvironmentVariables(prefix: "AnastasiiaPortfolio_");
// User Secrets (optional): dotnet user-secrets set "EmailSettings:SmtpPassword" "your-gmail-app-password"
builder.Configuration.AddUserSecrets<Program>(optional: true);

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
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
