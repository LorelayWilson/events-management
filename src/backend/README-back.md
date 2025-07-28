# Events Management System - Backend

A .NET 6 Web API backend for an events management system with user authentication, event management, and secure API endpoints.

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 6.0
- **Database**: SQLite with Entity Framework Core
- **Authentication**: JWT Bearer tokens with ASP.NET Core Identity
- **Documentation**: Swagger/OpenAPI
- **Language**: C#

## 📋 Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022 or VS Code
- SQLite (included with EF Core)

## 🔧 Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd src/backend
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure the database**
   - The application uses SQLite by default
   - Database will be created automatically on first run
   - Seed data will be populated automatically

4. **Configure JWT settings** (optional)
   - Update `appsettings.json` with your JWT configuration:
   ```json
   {
     "Jwt": {
       "Key": "your-secret-key-here",
       "Issuer": "your-issuer",
       "Audience": "your-audience"
     }
   }
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the API**
   - API Base URL: `https://localhost:5001` or `http://localhost:5000`
   - Swagger Documentation: `https://localhost:5001/swagger`

## 📁 Project Structure

```
backend/
├── Controllers/          # API Controllers
│   ├── AuthController.cs # Authentication endpoints
│   ├── EventsController.cs # Event management
│   └── UserController.cs # User management
├── Data/                # Database context and initialization
│   ├── ApplicationDbContext.cs
│   └── DbInitializer.cs
├── DTOs/               # Data Transfer Objects
│   ├── EventDto.cs
│   └── PaginatedResult.cs
├── Models/             # Entity models
│   ├── ApplicationUser.cs
│   ├── Category.cs
│   ├── Event.cs
│   ├── EventCategory.cs
│   └── Registration.cs
├── Services/           # Business logic services
│   └── EventService.cs
└── Program.cs         # Application entry point
```

## 🔒 Security Features

- **Input Validation**: Comprehensive model validation
- **Secure Logging**: Sensitive information is not logged

## 🔍 Problems Found and Solved

### 🚨 Critical
- [CRITICAL] **Password exposure in logs**: User passwords were being logged in application logs, representing a serious security risk. Fixed by completely removing password logging in authentication endpoints.
- [CRITICAL] **Event privacy bug**: Private events were shown to all users, violating event privacy. Implemented a filtering system that only shows private events to their creators or registered users.
- [CRITICAL] **Duplicate event registrations**: Users could register multiple times for the same event, causing data integrity and capacity issues. Implemented validation to prevent duplicate registrations.

### ⚠️ Important
- [IMPORTANT] **Incorrect development URLs**: Development URLs were configured with incorrect ports (50713/50714). Fixed to standard ports (5000/5001) for better compatibility.
- [IMPORTANT] **Inconsistent authentication**: The authentication system did not properly handle unauthenticated users in some endpoints. Improved authentication validation.
- [IMPORTANT] **Lack of pagination**: Event endpoints returned all events without pagination, which could cause performance issues with large data volumes. Implemented pagination system.
- [IMPORTANT] **Inefficient queries**: Entity Framework queries were not optimized, causing unnecessary entity tracking and excessive memory consumption. Implemented AsNoTracking() for performance optimization.

### 💡 Implemented Improvements

- **Advanced Event Management and API**:
  - Event privacy system: intelligent filtering to show private events only to creators or registered users.
  - User events API: endpoint to get events created by a specific user, with pagination and privacy control.
  - Duplicate registration prevention and capacity control: validation to prevent multiple registrations from the same user and respect maximum capacity.
  - Complete event CRUD: creation, updating, deletion and querying of events with category and address support.
  - Sorting and pagination: endpoints with pagination and sorting by event date.

- **Security and Authentication**:
  - Elimination of sensitive information logging (passwords).
  - Robust authentication state validation in endpoints.
  - Enhanced security in credential and role handling.

- **Optimization and Performance**:
  - Optimized queries with AsNoTracking() to reduce memory usage and improve performance.
  - Updated DTOs and models to support new fields (address, icons, etc.).

- **Export and Data**:
  - Enriched test data: sample categories with icons and events with realistic addresses.
  - DTO structure for paginated results and efficient export.

- **Configuration and Development**:
  - Fixed development URLs and project configuration for better compatibility.
  - Reorganization and improvement of services to facilitate maintenance and scalability.

## ⏱️ Time Invested

- **Analysis and Design**:  
  Security analysis, privacy, requirements, user experience and architecture: **2 hours**

- **Implementation and Development**:  
  Feature implementation (authentication, events, pagination, filtering, export, endpoints, validations, etc.): **3 hours**
  Refactoring, query optimization, DTOs/models and configuration adjustments: **1 hours**

- **Testing and Validation**:  
  Endpoint testing, security validation, visual feedback and final adjustments: **1 hours**

- **Total approximate**: **~7 hours**


## 📄 Author

**Lorelay Pricop Florescu**  
Graduate in Interactive Technologies and Project Manager with experience in .NET, Python, Angular, Azure DevOps, AI, and Agile methodologies.

🔗 [LinkedIn](https://www.linkedin.com/in/lorelaypricop)  
📧 Contact: lorelaypricop@gmail.com

> Some ideas were reviewed with the support of artificial intelligence (AI) tools



