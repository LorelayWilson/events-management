# 🎯 Events Management System - Technical Challenge

## 📋 Project Overview

This is an Events Management System created for technical assessment. The system allows users to create events, register for them, and view events by categories. Your task is to identify and fix issues while implementing new features.

## 🚀 Quick Start

### Prerequisites
- .NET 6 SDK
- Node.js 16+ and npm
- Git

### 1. Clone the Repository
```bash
git clone <repository-url>
cd events-management-system
```

### 2. Setup Backend (.NET API)
```bash
cd src/backend
dotnet restore
dotnet run
```
The API will be available at: `http://localhost:5000`

### 3. Setup Frontend (Angular)
```bash
cd src/frontend
npm install
npm start
```
The frontend will be available at: `http://localhost:4200`


## 👥 Test Users

The system comes with pre-seeded test data:

| Email | Password | Role |
|-------|----------|------|
| john@test.com | Password123! | User |
| jane@test.com | Password123! | User |
| bob@test.com | Password123! | User |

## 🎯 Your Mission

### 1. 🔍 **Identify and Document Issues**
Look for problems in these areas:
- **🔒 Security Issues** (unauthorized access, data exposure)
- **⚡ Performance Problems** (slow queries, no pagination)
- **🧠 Logic Errors** (validation failures, business rules)
- **📱 UI/UX Issues** (responsive design, user experience)

### 2. 🔧 **Fix Critical Issues**
Implement solutions for the most critical problems found.

### 3. ✨ **Add Missing Features**
- **User events page** (`/users/{id}/events`)
- **Event address field** (optional with map display)
- **Unit tests** for backend
- **Any other improvements you consider important**

## 📁 Project Structure

```
src/
├── backend/                 # .NET 6 Web API
│   ├── Controllers/         # API Controllers
│   ├── Services/           # Business Logic
│   ├── Models/             # Entity Models
│   ├── Data/               # Database Context
│   └── DTOs/               # Data Transfer Objects
└── frontend/               # Angular 15 Application
    ├── src/app/
    │   ├── components/     # Angular Components
    │   ├── services/       # HTTP Services
    │   └── ...
    └── ...
```

## 🔧 Technologies Used

- **Backend**: .NET 6, Entity Framework Core, SQLite, ASP.NET Core Identity
- **Frontend**: Angular 15, Bootstrap 5, TypeScript
- **Database**: SQLite (local file)

## 📝 Expected Deliverables

1. **Fixed codebase** with documented changes
2. **README.md** with:
   - List of errors found and how you fixed them
   - Technical decisions made
   - Time spent on each task
3. **Tests** covering your new implementations
4. **Clean commit history** with descriptive messages

## 🛠️ Development Tips

### Backend Development
```bash
# Run with hot reload
dotnet watch run

# Run tests
dotnet test

# Reset database
rm events.db
dotnet run
```

### Frontend Development
```bash
# Run with hot reload
ng serve

# Run tests
ng test

# Build for production
ng build
```

## ⚠️ Common Issues

If you encounter issues:

1. **Database locked**: Stop the backend and restart
2. **Port already in use**: Change ports in `launchSettings.json` or `angular.json`
3. **CORS errors**: Backend should be configured for local development
4. **npm install fails**: Try `npm ci` or delete `node_modules` and reinstall

## 🎯 Evaluation Focus

We're looking for:
- **Problem detection skills**: Can you find the issues?
- **Solution quality**: How well do you implement fixes?
- **Code quality**: Clean, maintainable code
- **Documentation**: Clear explanations of your changes

---

**Good luck! 🚀**

*Remember: Focus on identifying and fixing the most critical issues. Document your process and decisions clearly.*