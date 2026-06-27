# Hotel Management System

A full-stack **Hotel Management System** developed using **ASP.NET Core MVC** and **SQL Server**. The application streamlines hotel operations by enabling guests to browse and book rooms online while providing administrators with a complete dashboard to manage rooms and reservations.

---

## Overview

The Hotel Management System is a web-based application designed to simplify hotel room booking and management. It replaces manual reservation processes with an intuitive online platform where guests can reserve rooms and administrators can efficiently manage hotel inventory and bookings.

The project follows the **Model-View-Controller (MVC)** architecture, ensuring clean code organization, maintainability, and scalability.

---

## Features

### Guest Features

* Browse available rooms
* View room details and pricing
* Book available rooms
* Receive booking confirmation
* Search bookings using guest name
* Cancel existing bookings

### Admin Features

* Admin dashboard
* Add new rooms
* Update room information
* Delete rooms
* View all bookings
* Cancel bookings
* Manage room availability

---

## Tech Stack

| Technology              | Purpose             |
| ----------------------- | ------------------- |
| ASP.NET Core MVC 8      | Web Framework       |
| C# 12                   | Backend Programming |
| Entity Framework Core 8 | ORM                 |
| SQL Server LocalDB      | Database            |
| HTML5                   | Structure           |
| CSS3                    | Styling             |
| Razor Views             | Dynamic UI          |
| Kestrel                 | Web Server          |

---

## Architecture

The project follows the **MVC (Model-View-Controller)** architecture.

* **Model** – Defines application data and business entities.
* **View** – Displays the user interface.
* **Controller** – Handles requests, processes business logic, and coordinates between models and views.

---

## 🗄 Database Design

The system consists of two primary tables:

### Rooms

* Id
* RoomNumber
* Type
* Price
* IsAvailable

### Bookings

* Id
* RoomId (Foreign Key)
* UserName
* BookingDate

### Data Integrity

* A room can have multiple bookings over time.
* Active bookings automatically mark rooms as unavailable.
* Booking conflicts are prevented by ensuring a room cannot be double-booked.

---

## Getting Started

### Prerequisites

* Visual Studio 2022
* .NET 8 SDK
* SQL Server LocalDB

### Installation

1. Clone the repository

```bash
git clone https://github.com/azanshah459/Hotel-Management-System.git
```

2. Open the solution in Visual Studio.

3. Restore NuGet packages.

4. Update the database using Entity Framework migrations.

```powershell
Update-Database
```

5. Run the application.

---

## Project Structure

```
Hotel-Management-System
│
├── Controllers
├── Models
├── Views
├── Data
├── Migrations
├── wwwroot
├── Properties
├── Program.cs
├── appsettings.json
└── README.md
```

---

## Future Enhancements

* User Authentication & Authorization
* Online Payment Integration
* Email Booking Confirmation
* Room Image Gallery
* Check-In / Check-Out Module
* Booking History
* Customer Reviews
* Reports & Analytics
* Responsive Mobile Design

---

## Learning Outcomes

Through this project, we gained practical experience with:

* ASP.NET Core MVC
* C# Programming
* Entity Framework Core
* SQL Server Database Design
* CRUD Operations
* MVC Architecture
* Razor Views
* Web Application Development
* Database Relationships
* Business Logic Implementation

---

## Authors

* **Azan Shah**
* **Yasbah Ali**
* **Muhammad Salih**

---

## License

This project was developed as a Web Programming course project for educational purposes.

---

If you found this project interesting, consider giving it a star!
