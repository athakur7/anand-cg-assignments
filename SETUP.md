# Repository Setup Guide

This guide explains how to set up the repository after cloning and restore client-side libraries.

## Quick Start

After cloning the repository, run this command from the root:

### Windows (PowerShell)
```powershell
.\restore-libraries.ps1
```

### macOS / Linux (Bash)
```bash
find . -name libman.json | while read f; do
  cd "$(dirname "$f")"
  dotnet libman restore
  cd - > /dev/null
done
```

## What Is This?

Client-side libraries (Bootstrap, jQuery, etc.) are managed using **LibMan** (Library Manager), which is built into ASP.NET Core. This keeps the repository:
- **Smaller** - Less bandwidth needed to clone
- **Cleaner** - No large dependency files cluttering the repo
- **Synchronized** - Library versions are defined in `libman.json` files

## Installation Methods

### Method 1: Automatic Script (Recommended)

Run the provided PowerShell script from the root:
```powershell
.\restore-libraries.ps1
```

This script automatically:
- Finds all projects with `libman.json`
- Runs `dotnet libman restore` in each
- Reports success/failure

### Method 2: Visual Studio (Easiest)

If you open the solution in **Visual Studio**:
1. Right-click each project in Solution Explorer
2. Select **Restore Client-Side Libraries**
3. Or use Tools → **Restore Client-Side Libraries**

Visual Studio will automatically restore all libraries.

### Method 3: Manual (Per Project)

Navigate to each project folder and run:
```bash
cd path/to/project
dotnet libman restore
```

### Method 4: Manual Installation (If .NET SDK Not Available)

If you don't have .NET SDK installed but need the files:
1. Download libraries manually or use a CDN
2. Extract to `wwwroot/lib/` in each project
3. (See [library sources](#library-sources) below)

## Prerequisites

### Required
- **.NET SDK 5.0 or higher** (for `dotnet libman` command)
  - Download: https://dotnet.microsoft.com/download
  - Check: `dotnet --version`

### Optional
- **Visual Studio 2019+** or **Visual Studio Code**
- **LibMan CLI** (installed automatically with .NET SDK 5.0+)

### Installing .NET SDK

#### Windows
Download from https://dotnet.microsoft.com/download and run the installer.

#### macOS
```bash
brew install dotnet
```

#### Linux
Follow instructions at https://learn.microsoft.com/en-us/dotnet/core/install/linux

## Library Sources

The repositories are configured to use **cdnjs** for retrieving libraries. Managed libraries include:

| Library | Version | Purpose |
|---------|---------|---------|
| bootstrap | 5.3.0 | CSS framework |
| jquery | 3.6.0 | JavaScript library |
| jquery-validation | 1.19.5 | Form validation |
| jquery-validation-unobtrusive | 4.0.0 | ASP.NET integration |

These are defined in `libman.json` files in each project directory.

## Verify Installation

After restoration, check that libraries are restored:

```bash
ls anand-week1-assignment/11-march-assignment/"MVC Demo Project"/wwwroot/lib/
```

You should see directories for: `bootstrap/`, `jquery/`, `jquery-validation/`, `jquery-validation-unobtrusive/`

## Troubleshooting

### Error: "dotnet: command not found"
- **Solution**: Install .NET SDK from https://dotnet.microsoft.com/download

### Error: "Could not execute because the specified command or file was not found"
- **Solution**: LibMan isn't installed. Try:
  ```bash
  dotnet tool install -g Microsoft.Web.LibraryManager.Cli
  ```
  Then retry `dotnet libman restore`

### Libraries still missing after running script
- Check that `.NET SDK` version is 5.0 or higher: `dotnet --version`
- Verify `libman.json` exists in the project directory
- Try restoring a single project manually for detailed error messages

### Script execution blocked on Windows
Run PowerShell as Administrator and execute:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

## Repository Size

Before and after library exclusion:

- **With libraries** (~1.3 GB): Complete libraries checked into Git
- **Without libraries** (~150 MB): Only library definitions (`libman.json`)

This ~90% reduction in size means:
- Faster cloning
- Less bandwidth usage
- Faster checkouts/pulls

## For Developers

### Adding a New Library

1. Open `libman.json` in your project
2. Add a new entry:
   ```json
   {
       "provider": "cdnjs",
       "library": "library-name@version",
       "destination": "wwwroot/lib/library-name"
   }
   ```
3. Run `dotnet libman restore` in that project
4. Commit the updated `libman.json`

### Updating Library Versions

1. Edit the version in `libman.json`
2. Run `dotnet libman restore`
3. Commit the change

## See Also

- [LibMan Documentation](https://learn.microsoft.com/en-us/aspnet/core/client-side/libman)
- [ASP.NET Core - Managing Client-Side Libraries](https://learn.microsoft.com/en-us/aspnet/core/client-side)
