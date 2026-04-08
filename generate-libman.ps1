# Generate libman.json files for all ASP.NET Core projects with wwwroot/lib
# This script creates libman.json based on existing library directories

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Generating libman.json files for projects" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan

$count = 0

Get-ChildItem -Recurse -Filter "*.csproj" -ErrorAction SilentlyContinue | ForEach-Object {
    $projDir = Split-Path $_.FullName
    $libDir = Join-Path $projDir "wwwroot\lib"
    $libmanPath = Join-Path $projDir "libman.json"
    
    # Skip if already has libman.json
    if (Test-Path $libmanPath) {
        return
    }
    
    # Skip if no wwwroot/lib directory
    if (-not (Test-Path $libDir)) {
        return
    }
    
    # Get library names from existing directories
    $libs = @(Get-ChildItem -Path $libDir -Directory -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Name)
    
    if ($libs.Count -eq 0) {
        return
    }
    
    # Library version mapping - adjust as needed
    $libVersions = @{
        'bootstrap' = '5.3.0'
        'jquery' = '3.6.0'
        'jquery-validation' = '1.19.5'
        'jquery-validation-unobtrusive' = '4.0.0'
        'popper.js' = '2.11.6'
        'select2' = '4.1.0-rc.0'
    }
    
    # Build libraries array
    $libraries = @()
    foreach ($lib in $libs) {
        $version = if ($libVersions.ContainsKey($lib)) { $libVersions[$lib] } else { "1.0.0" }
        $libraries += @{
            provider = "cdnjs"
            library = "$lib@$version"
            destination = "wwwroot/lib/$lib"
        }
    }
    
    # Create libman.json
    if ($libraries.Count -gt 0) {
        $libmanJson = @{
            version = "1.0"
            defaultProvider = "cdnjs"
            libraries = $libraries
        } | ConvertTo-Json -Depth 10
        
        Write-Host "`nCreating libman.json in: $projDir" -ForegroundColor Yellow
        Write-Host "  Libraries: $($libs -join ', ')" -ForegroundColor Green
        
        $libmanJson | Out-File -FilePath $libmanPath -Encoding UTF8
        $count++
    }
}

Write-Host "`n============================================" -ForegroundColor Cyan
Write-Host "Generated $count libman.json file(s)" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "1. Review generated libman.json files" -ForegroundColor Cyan
Write-Host "2. Run './restore-libraries.ps1' to verify restoration works" -ForegroundColor Cyan
Write-Host "3. Commit libman.json files to git" -ForegroundColor Cyan
Write-Host "4. Delete or ignore wwwroot/lib directories" -ForegroundColor Cyan
