# UltimatePOS

## Overview
UltimatePOS is a Windows desktop Point of Sale (POS) application built with .NET 8 and WinUI 3. This is a Windows-only application that requires Windows 10 or later to run the full UI.

**Note:** The WinUI desktop application cannot run on Replit's Linux environment. Only the cross-platform library components (Core, Data, Services) and tests can be built and tested here.

## Project Structure
```
├── src/
│   ├── UltimatePOS.Core/        # Core domain models and business logic
│   ├── UltimatePOS.Data/        # Data access layer with Entity Framework Core (SQLite)
│   ├── UltimatePOS.Services/    # Business services layer
│   └── UltimatePOS.WinUI/       # Windows UI layer (WinUI 3) - Windows only
├── tests/
│   └── UltimatePOS.Tests/       # Unit tests (xUnit)
└── UltimatePOS.slnx             # Solution file
```

## Technology Stack
- **.NET 8**: Cross-platform framework
- **WinUI 3**: Windows desktop UI framework (Windows-only)
- **Entity Framework Core**: ORM with SQLite provider
- **xUnit**: Unit testing framework
- **BCrypt**: Password hashing
- **Serilog**: Logging

## What Works on Replit
- Building Core, Data, and Services libraries
- Running unit tests
- Code editing and development

## What Doesn't Work on Replit
- Running the WinUI desktop application (requires Windows)
- Full solution build (WinUI project targets Windows)

## Commands
- Build libraries: `dotnet build src/UltimatePOS.Core && dotnet build src/UltimatePOS.Data && dotnet build src/UltimatePOS.Services`
- Run tests: `dotnet test tests/UltimatePOS.Tests`

## Configuration
The application uses `appsettings.json` for configuration including:
- Database settings (SQLite by default)
- Theme settings
- Localization (en-US, fr-FR, ar-AR)
- POS settings
- Invoice settings
- Backup settings

## Recent Changes
- 2026-02-05: Initial import and Replit environment setup
  - Installed .NET 8.0
  - Configured build and test workflow
  - Added .gitignore for .NET projects
