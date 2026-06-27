using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using HotelManagement.Data;
using HotelManagement.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("Auth"));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "HotelManagement.Auth";
    });
builder.Services.AddAuthorization();

// Add Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Register EF Core with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Apply migrations automatically on startup (optional but handy)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var authSettings = scope.ServiceProvider.GetRequiredService<IOptions<AuthSettings>>().Value;
    var hasher = new PasswordHasher<UserAccount>();

    var seeded = false;
    seeded |= EnsureUser(db, hasher, authSettings.Admin.Username, authSettings.Admin.Password, "Admin");
    seeded |= EnsureUser(db, hasher, authSettings.User.Username, authSettings.User.Password, "User");
    
    seeded |= EnsureRooms(db);

    if (seeded)
    {
        db.SaveChanges();
    }
}

static bool EnsureRooms(ApplicationDbContext db)
{
    if (db.Rooms.Any()) return false;

    db.Rooms.AddRange(
        new Room { RoomNumber = "101", Type = "Standard", Price = 15000, IsAvailable = true },
        new Room { RoomNumber = "102", Type = "Standard", Price = 15000, IsAvailable = true },
        new Room { RoomNumber = "201", Type = "Deluxe", Price = 25000, IsAvailable = true },
        new Room { RoomNumber = "202", Type = "Deluxe", Price = 25000, IsAvailable = true },
        new Room { RoomNumber = "301", Type = "Suite", Price = 45000, IsAvailable = true },
        new Room { RoomNumber = "401", Type = "Presidential Suite", Price = 85000, IsAvailable = true },
        new Room { RoomNumber = "105", Type = "Family Room", Price = 30000, IsAvailable = true }
    );
    return true;
}

static bool EnsureUser(ApplicationDbContext db, PasswordHasher<UserAccount> hasher, string username, string password, string role)
{
    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
        return false;
    }

    var normalized = username.Trim().ToLower();
    var exists = db.UserAccounts.Any(u => u.Username.ToLower() == normalized);
    if (exists)
    {
        return false;
    }

    var user = new UserAccount
    {
        Username = username.Trim(),
        Role = role
    };

    user.PasswordHash = hasher.HashPassword(user, password);
    db.UserAccounts.Add(user);
    return true;
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/NotFoundPage");

// Enable response compression for production performance
app.UseResponseCompression();

app.UseHttpsRedirection();

// Add Security Headers Middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    await next();
});

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
