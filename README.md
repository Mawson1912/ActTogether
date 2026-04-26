# ActTogether

A social networking platform for actors and performers, built with ASP.NET Core MVC and SQL Server. Users can share self-tape auditions with friends, exchange private feedback, and communicate through direct messaging.

## Technologies Used

- .NET 8.0 (ASP.NET Core MVC)
- Entity Framework Core 8.0
- SQL Server
- Bootstrap 5 (Sketchy theme from Bootswatch)
- JavaScript
- BCrypt.Net for password hashing
- Swagger for API documentation

## Prerequisites

- .NET 8.0 SDK
- SQL Server (local instance or network server)
- SQL Server Management Studio (recommended)

## Setup Instructions

### 1. Database Setup

1. Open SQL Server Management Studio and connect to your SQL Server instance
2. Open the setup script located at `docs/database-setup.sql`
3. Run the script to create the database, tables, and dummy data

### 2. Configure Connection String

Update the connection string in `appsettings.json` to match your SQL Server setup:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ActTogether;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Replace `YOUR_SERVER` with your SQL Server instance name (e.g., `localhost`, `.\SQLEXPRESS`, or `STU-LAB-SQL,1433`).

### 3. Run the Application

Using the .NET CLI:

```
dotnet run
```

Or open `ActTogether.sln` in Visual Studio and press F5.

## Demo Accounts

All dummy accounts use the password **Password123!**

| Username | Role  |
|----------|-------|
| alice    | User  |
| bob      | User  |
| charlie  | User  |
| dave     | User  |
| admin    | Admin |

## API Documentation

Swagger UI is available at `/swagger/index.html` when the application is running.

### API Endpoints

- `GET/POST/PUT/DELETE /api/UsersApi` — User management
- `GET/POST/PUT/DELETE /api/PostsApi` — Post management
- `GET/POST/PUT/DELETE /api/FriendshipsApi` — Friendship management
- `GET/POST/PUT/DELETE /api/MessagesApi` — Message management

## Project Structure

```
ActTogether/
├── Controllers/        # MVC and API controllers
├── Data/               # Database context (AppDbContext)
├── Helpers/            # Utility classes (e.g., PasswordHelper)
├── Models/             # Entity models and ViewModels
├── Views/              # Razor views organised by controller
├── wwwroot/            # Static files (CSS, JS, images, uploads)
├── docs/               # Database setup script and architecture diagram
├── Program.cs          # Application entry point and service configuration
└── appsettings.json    # Configuration (connection string, etc.)
```

## Features

- User registration and login with cookie-based authentication
- Profile pages with bio and profile picture
- Friend requests (send, accept, decline, block)
- Video post sharing via Google Drive links
- Private feedback on friends' audition tapes
- Direct messaging with unread message counts
- User search
- Contact form for admin support
- Admin dashboard for managing reports and contact messages
- User reporting system

## Making an Existing User an Admin

Run this SQL command against your database:

```sql
UPDATE Users SET IsAdmin = 1 WHERE Username = 'your_username';
```
