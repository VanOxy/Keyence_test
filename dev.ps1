# Starts SalesPlatform in development mode: backend (ASP.NET Core, :5213) and
# frontend (Vite, :5173), each in its own window so you can watch logs / stop them
# independently with Ctrl+C. DB migration + seeding happen automatically on backend
# startup (see Program.cs) — nothing else to run by hand.

$root = Split-Path -Parent $MyInvocation.MyCommand.Path

function Test-PortInUse($port) {
    return [bool](Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue)
}

if (Test-PortInUse 5213) {
    Write-Host "Backend already running on :5213 - skipping." -ForegroundColor Yellow
} else {
    Write-Host "Starting backend on http://localhost:5213 ..." -ForegroundColor Cyan
    Start-Process powershell -ArgumentList @(
        '-NoExit', '-Command',
        "Set-Location `"$root\src\backend\SalesPlatform.Api`"; dotnet run --urls http://localhost:5213"
    )
}

if (Test-PortInUse 5173) {
    Write-Host "Frontend already running on :5173 - skipping." -ForegroundColor Yellow
} else {
    Write-Host "Starting frontend on http://localhost:5173 ..." -ForegroundColor Cyan
    Start-Process powershell -ArgumentList @(
        '-NoExit', '-Command',
        "Set-Location `"$root\src\frontend`"; npm run dev"
    )
}

Write-Host "`nBackend:  http://localhost:5213/swagger" -ForegroundColor Green
Write-Host "Frontend: http://localhost:5173" -ForegroundColor Green
