# Script para build e publicação do projeto completo (Angular + .NET API)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Building Letters Application" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# 1. Build do Angular
Write-Host "`n[1/4] Building Angular application..." -ForegroundColor Yellow
Set-Location "Letters.Angular"
npm install
npm run build -- --configuration=production

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao fazer build do Angular!" -ForegroundColor Red
    exit 1
}

# 2. Copiar arquivos do Angular para o projeto da API
Write-Host "`n[2/4] Copying Angular files to API project..." -ForegroundColor Yellow
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

# 3. Build da API .NET
Write-Host "`n[3/4] Building .NET API..." -ForegroundColor Yellow
dotnet build Letters.API\Letters.API.csproj --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao fazer build da API!" -ForegroundColor Red
    exit 1
}

# 4. Publicar a API
Write-Host "`n[4/4] Publishing .NET API..." -ForegroundColor Yellow
dotnet publish Letters.API\Letters.API.csproj --configuration Release --output publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao publicar a API!" -ForegroundColor Red
    exit 1
}

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "`nPublished files are in: .\publish" -ForegroundColor Cyan
Write-Host "You can now deploy the 'publish' folder to Azure App Service" -ForegroundColor Cyan
