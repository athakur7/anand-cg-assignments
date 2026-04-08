# anand-cg-assignments

## Setup Instructions

### After Cloning the Repository

Client-side libraries (Bootstrap, jQuery, etc.) are **not** stored in the repository to keep it size-efficient. They are managed using **LibMan** (Library Manager) built into ASP.NET Core.

#### Option 1: Automatic Restore (Recommended)

Run the restore script from the repository root:

```powershell
.\restore-libraries.ps1
```

This script will:
- Automatically find all ASP.NET Core projects
- Restore libraries from `libman.json` using the dotnet CLI
- Display progress and any issues encountered

#### Option 2: Manual Restore (Per Project)

For each project folder, run:

```powershell
cd path/to/project
dotnet libman restore
```

#### Option 3: Using Visual Studio

If you have Visual Studio open:
1. Right-click the project in Solution Explorer → **Restore Client-Side Libraries**
2. Or use Tools → **Restore Client-Side Libraries**

### What Gets Restored

The following libraries are managed by LibMan:
- **Bootstrap** - CSS framework
- **jQuery** - JavaScript library
- **jQuery Validation** - Form validation
- **jQuery Validation Unobtrusive** - ASP.NET integration

These are restored to `wwwroot/lib/` in each project.

### Prerequisites

- **.NET SDK** - Required for `dotnet libman restore`
  - [Download .NET](https://dotnet.microsoft.com/download)
  - Check installation: `dotnet --version`

- **LibMan** (included with .NET SDK 5.0+)
  - If needed: `dotnet tool install -g Microsoft.Web.LibraryManager.Cli`

### Repository Size Optimization

This repository uses `.gitignore` to exclude:
- `wwwroot/lib/` - Client-side libraries
- `bin/` and `obj/` - Build artifacts
- `node_modules/` - NPM packages

This keeps the repository lean while ensuring everything works across different machines.