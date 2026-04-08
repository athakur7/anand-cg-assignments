param()

Write-Host "Restoring client-side libraries with LibMan" -ForegroundColor Cyan
Write-Host ""

$projsWithLibMan = @()

Get-ChildItem -Recurse -Filter "*.csproj" -ErrorAction SilentlyContinue | ForEach-Object {
    $projDir = Split-Path $_.FullName
    $libmanPath = Join-Path $projDir "libman.json"
    if (Test-Path $libmanPath) {
        $projsWithLibMan += $projDir
    }
}

Write-Host "Found $($projsWithLibMan.Count) project(s) with libman.json" -ForegroundColor Green
Write-Host ""

$successCount = 0
$failureCount = 0

foreach ($projDir in $projsWithLibMan) {
    Write-Host "Restoring: $projDir" -ForegroundColor Yellow
    Push-Location $projDir
    
    dotnet libman restore 2>&1 | Out-Null
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  [OK]" -ForegroundColor Green
        $successCount++
    } else {
        Write-Host "  [FAILED]" -ForegroundColor Red
        $failureCount++
    }
    
    Pop-Location
}

Write-Host ""
Write-Host "Done! Restored: $successCount, Failed: $failureCount" -ForegroundColor Cyan
