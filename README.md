# 🏨 Grand Horizon — Hotel Management System
### ASP.NET Core MVC 8 + Entity Framework Core + SQL Server

---

## 📋 TABLE OF CONTENTS
1. [What This Project Does](#what-this-project-does)
2. [Prerequisites — What to Install](#prerequisites)
3. [Project Structure](#project-structure)
4. [Step-by-Step Setup Guide](#step-by-step-setup)
5. [Running the Project](#running-the-project)
6. [Using the Application](#using-the-application)
7. [Troubleshooting](#troubleshooting)

---

## What This Project Does

A full hotel room management web application with:

| Module | Features |
|--------|----------|
| **User** | Browse available rooms, Book a room, View bookings by name, Cancel bookings |
| **Admin** | View all rooms + bookings, Add/Edit/Delete rooms, Manage availability |

**Tech Stack:** ASP.NET Core MVC 8 · Razor Views · Entity Framework Core · SQL Server LocalDB · C# 12

---

## Prerequisites

You need to install **three things** before running this project.

---

### ✅ 1. Install Visual Studio 2022 (Community — Free)

**Download:** https://visualstudio.microsoft.com/vs/community/

During installation, on the **Workloads** screen, check:
> ☑ **ASP.NET and web development**

This will automatically install:
- .NET 8 SDK
- SQL Server LocalDB (the lightweight database we use)
- All required tools

**How to check if .NET is installed after:**
Open Command Prompt (Win+R → type `cmd`) and run:
```
dotnet --version
```
You should see `8.x.x` or higher.

---

### ✅ 2. Install SQL Server LocalDB (if not installed automatically)

LocalDB usually comes with Visual Studio. To check, open **Command Prompt** and run:
```
sqllocaldb info
```
If you see `MSSQLLocalDB` listed → you're good.

If NOT installed, download from:
https://www.microsoft.com/en-us/sql-server/sql-server-downloads
→ Click **Download now** under "Express" edition → During install choose **LocalDB** feature.

---

### ✅ 3. Install SQL Server Management Studio — SSMS (Optional but Recommended)

Used to visually inspect your database tables and data.

**Download:** https://aka.ms/ssmsfullsetup

---

## Project Structure

```
HotelManagement/
│
├── Controllers/
│   ├── HomeController.cs        ← Dashboard stats
│   ├── RoomsController.cs       ← Room CRUD (User browse + Admin manage)
│   └── BookingsController.cs    ← Book, Cancel, View bookings
│
├── Models/
│   ├── Room.cs                  ← Room entity (Id, RoomNumber, Type, Price, IsAvailable)
│   └── Booking.cs               ← Booking entity (Id, RoomId, UserName, BookingDate)
│
├── Data/
│   └── ApplicationDbContext.cs  ← EF Core database context + seed data
│
├── Views/
│   ├── Shared/_Layout.cshtml    ← Main HTML layout (navbar, footer, styles)
│   ├── Home/Index.cshtml        ← Landing page
│   ├── Rooms/
│   │   ├── Index.cshtml         ← Browse available rooms (User)
│   │   ├── AdminIndex.cshtml    ← All rooms table (Admin)
│   │   ├── Create.cshtml        ← Add new room form
│   │   ├── Edit.cshtml          ← Edit room form
│   │   └── Delete.cshtml        ← Delete confirmation
│   └── Bookings/
│       ├── Book.cshtml          ← Booking form
│       ├── Confirmation.cshtml  ← Booking success page
│       ├── MyBookings.cshtml    ← User views/cancels their bookings
│       └── Index.cshtml         ← Admin views all bookings
│
├── Migrations/                  ← EF Core database migration files
├── wwwroot/js/site.js           ← Client-side scripts
├── appsettings.json             ← Database connection string
├── Program.cs                   ← App startup & DI configuration
└── HotelManagement.csproj       ← Project file with NuGet packages
```

---

## Step-by-Step Setup

### STEP 1 — Open the Project in Visual Studio

1. Open **Visual Studio 2022**
2. Click **"Open a project or solution"**
3. Navigate to the `HotelManagement` folder you downloaded
4. Select **`HotelManagement.csproj`** and click **Open**
5. Wait for Visual Studio to load (it will restore NuGet packages automatically — watch the bottom status bar)

---

### STEP 2 — Check the Connection String

Open **`appsettings.json`**. You'll see:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

**This is already correct for most machines.** The database `HotelManagementDB` will be created automatically.

> ⚠️ If you have a full SQL Server installed (not LocalDB), change `(localdb)\\mssqllocaldb` to your server name, e.g., `.\SQLEXPRESS` or `localhost`.

---

### STEP 3 — Restore NuGet Packages

In Visual Studio:
- Go to **Tools → NuGet Package Manager → Manage NuGet Packages for Solution**
- Click **"Restore"** if prompted
- Or right-click the solution in Solution Explorer → **"Restore NuGet Packages"**

Alternatively, open **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console) and run:
```
dotnet restore
```

---

### STEP 4 — Apply Database Migrations

The migration files are already included in the project. You just need to apply them.

**In Visual Studio:**
Go to **Tools → NuGet Package Manager → Package Manager Console**

In the console that opens at the bottom, type:
```powershell
Update-Database
```
Press Enter and wait. You should see:
```
Build started...
Build succeeded.
Applying migration '20240101000000_InitialCreate'.
Done.
```

This creates the `HotelManagementDB` database with `Rooms` and `Bookings` tables, plus 5 sample rooms.

> 💡 **Alternative:** The app is also configured to auto-apply migrations on startup (`db.Database.Migrate()` in Program.cs), so even if you skip this step, it runs when you first launch.

---

## Running the Project

### Method 1 — Visual Studio (Recommended)

1. Press **F5** (or click the green ▶ **Play button** in the toolbar)
2. Select **"IIS Express"** or **"HotelManagement"** profile
3. Your browser opens automatically at `https://localhost:5001` or `http://localhost:5000`

### Method 2 — Command Line

Open a terminal in the project folder and run:
```bash
dotnet run
```
Then open your browser and go to:
```
https://localhost:5001
```
(or whatever port the terminal shows)

---

## Using the Application

### 👤 As a Guest/User

| What to do | Where to go |
|-----------|-------------|
| See available rooms | Click **"Browse Rooms"** in the navbar |
| Book a room | Click **"Book This Room"** on any room card |
| Fill in your name + check-in date | On the booking form |
| View your booking confirmation | Shown automatically after booking |
| Find your bookings | Click **"My Bookings"** → type your name → Search |
| Cancel a booking | In My Bookings → click **Cancel** |

### ⚙️ As Admin

| What to do | Where to go |
|-----------|-------------|
| See all rooms (including occupied) | Click **"Admin Panel"** in navbar |
| Add a new room | Admin Panel → **"+ Add New Room"** |
| Edit a room | Admin Panel → **"Edit"** next to any room |
| Delete a room | Admin Panel → **"Delete"** next to any room |
| View all guest bookings | Admin Panel → **"View All Bookings"** |
| Cancel any booking | All Bookings → **Cancel** |

---

## Verify the Database (Optional — Using SSMS)

1. Open **SQL Server Management Studio (SSMS)**
2. In the **Server name** field type: `(localdb)\mssqllocaldb`
3. Authentication: **Windows Authentication**
4. Click **Connect**
5. Expand: **Databases → HotelManagementDB → Tables**
6. You'll see `dbo.Rooms` and `dbo.Bookings`
7. Right-click a table → **"Select Top 1000 Rows"** to view data

---

## Troubleshooting

### ❌ "A network-related or instance-specific error..."
- SQL Server LocalDB is not running. Open Command Prompt and run:
  ```
  sqllocaldb start MSSQLLocalDB
  ```

### ❌ "No executable found matching command 'dotnet-ef'"
- Run in Package Manager Console:
  ```
  Install-Package Microsoft.EntityFrameworkCore.Tools
  ```
  Then retry `Update-Database`.

### ❌ NuGet packages not restoring
- Check your internet connection
- Go to Tools → Options → NuGet Package Manager → Package Sources
- Make sure `nuget.org` is listed and enabled

### ❌ Port already in use
- Change the port in `launchSettings.json` (Properties folder)
- Or kill the process using that port:
  ```
  netstat -ano | findstr :5001
  taskkill /PID <PID> /F
  ```

### ❌ "Build failed" errors
- Make sure you selected the **ASP.NET and web development** workload in Visual Studio
- Try: Build → Clean Solution, then Build → Rebuild Solution

---

## Quick Reference — URL Routes

| URL | What it does |
|-----|-------------|
| `/` | Home / Dashboard |
| `/Rooms` | Browse available rooms |
| `/Rooms/AdminIndex` | Admin — all rooms |
| `/Rooms/Create` | Add new room |
| `/Rooms/Edit/1` | Edit room with ID 1 |
| `/Rooms/Delete/1` | Delete room with ID 1 |
| `/Bookings/Book/2` | Book room with ID 2 |
| `/Bookings/MyBookings` | Search guest bookings |
| `/Bookings/Index` | Admin — all bookings |

---

*Built with ASP.NET Core 8 MVC · Entity Framework Core · SQL Server*
