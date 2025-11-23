# Script para testar o build localmente antes do deploy

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing Local Build" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# 1. Build do Angular
Write-Host "`n[1/3] Building Angular application..." -ForegroundColor Yellow
Set-Location "Letters.Angular"
npm run build -- --configuration production

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao fazer build do Angular!" -ForegroundColor Red
    Set-Location ..
    exit 1
}

# 2. Copiar arquivos do Angular para o projeto da API
Write-Host "`n[2/3] Copying Angular files to API project..." -ForegroundColor Yellow
Set-Location ..
$angularDist = "Letters.Angular\dist\letters.angular\browser"
$apiWwwroot = "Letters.API\wwwroot"

# Remover pasta wwwroot antiga se existir
if (Test-Path $apiWwwroot) {
    Remove-Item -Path $apiWwwroot -Recurse -Force
}

# Criar pasta wwwroot e copiar arquivos
New-Item -ItemType Directory -Path $apiWwwroot -Force
Copy-Item -Path "$angularDist\*" -Destination $apiWwwroot -Recurse -Force

Write-Host "Angular files copied successfully!" -ForegroundColor Green

# 3. Executar a API
Write-Host "`n[3/3] Starting API server..." -ForegroundColor Yellow
Write-Host "API will serve both Angular and API endpoints" -ForegroundColor Cyan
Write-Host "Access: http://localhost:5000" -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow
Write-Host "" 

Set-Location Letters.API
dotnet run
