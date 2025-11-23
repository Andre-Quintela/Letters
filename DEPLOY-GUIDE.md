# Guia de Deploy do Letters no Azure App Service

## Pré-requisitos

1. **Azure CLI** instalado ([Download](https://docs.microsoft.com/cli/azure/install-azure-cli))
2. **Node.js** e **npm** instalados
3. **.NET 8.0 SDK** instalado
4. Conta do Azure ativa

## Opção 1: Deploy Manual via Portal Azure

### Passo 1: Build e Publicação Local

1. Execute o script de build:
   ```powershell
   .\build-and-publish.ps1
   ```

2. Isso criará uma pasta `publish` com todos os arquivos necessários.

### Passo 2: Criar Web App no Portal Azure

1. Acesse o [Portal Azure](https://portal.azure.com)
2. Clique em "Criar um recurso" > "Web App"
3. Configure:
   - **Nome**: `letters-app-[seu-nome-único]`
   - **Sistema Operacional**: Windows ou Linux
   - **Stack de runtime**: .NET 8 (LTS)
   - **Região**: Brazil South (ou sua preferência)
   - **Plano de Preços**: B1 ou superior

4. Clique em "Revisar + criar" > "Criar"

### Passo 3: Deploy dos Arquivos

**Opção A - Via Portal:**
1. Vá para o recurso criado
2. No menu lateral, vá em "Centro de Implantação"
3. Escolha "Local Git" ou "FTP"
4. Siga as instruções para fazer upload da pasta `publish`

**Opção B - Via VS Code:**
1. Instale a extensão "Azure App Service"
2. Clique com botão direito na pasta `publish`
3. Selecione "Deploy to Web App..."
4. Escolha seu App Service

**Opção C - Via Azure CLI:**
```powershell
# Fazer login
az login

# Fazer deploy
az webapp deployment source config-zip `
  --resource-group seu-resource-group `
  --name letters-app-seu-nome `
  --src publish.zip
```

### Passo 4: Configurar Variáveis de Ambiente

No Portal Azure, vá em "Configuração" > "Configurações do aplicativo":

```
ASPNETCORE_ENVIRONMENT = Production
ConnectionStrings__DefaultConnection = [sua-connection-string-azure-sql]
OpenAI__ApiKey = [sua-chave-openai]
OpenAI__Endpoint = [seu-endpoint-openai]
OpenAI__DeploymentName = [seu-deployment-name]
```

## Opção 2: Deploy Automatizado via Azure CLI

### Deploy Completo com um Comando

```powershell
# 1. Login no Azure
az login

# 2. Criar Resource Group
az group create --name rg-letters --location brazilsouth

# 3. Criar App Service Plan
az appservice plan create `
  --name letters-plan `
  --resource-group rg-letters `
  --sku B1 `
  --is-linux

# 4. Criar Web App
az webapp create `
  --name letters-app-seu-nome `
  --resource-group rg-letters `
  --plan letters-plan `
  --runtime "DOTNETCORE:8.0"

# 5. Build e publicar
.\build-and-publish.ps1

# 6. Compactar arquivos
Compress-Archive -Path .\publish\* -DestinationPath publish.zip -Force

# 7. Deploy
az webapp deployment source config-zip `
  --resource-group rg-letters `
  --name letters-app-seu-nome `
  --src publish.zip

# 8. Configurar variáveis de ambiente
az webapp config appsettings set `
  --resource-group rg-letters `
  --name letters-app-seu-nome `
  --settings `
    ASPNETCORE_ENVIRONMENT=Production `
    "ConnectionStrings__DefaultConnection=sua-connection-string"
```

## Opção 3: Deploy com GitHub Actions (CI/CD)

### Criar workflow automático:

1. No repositório GitHub, vá em "Settings" > "Secrets and variables" > "Actions"
2. Adicione os secrets:
   - `AZURE_WEBAPP_PUBLISH_PROFILE` (baixe do Portal Azure)
   - `AZURE_SQL_CONNECTION_STRING`
   - `OPENAI_API_KEY`

3. Crie o arquivo `.github/workflows/azure-deploy.yml`:

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '20'
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Build Angular
      run: |
        cd Letters.Angular
        npm ci
        npm run build -- --configuration production
    
    - name: Copy Angular to API
      run: |
        mkdir -p Letters.API/wwwroot
        cp -r Letters.Angular/dist/letters.angular/browser/* Letters.API/wwwroot/
    
    - name: Build and Publish .NET
      run: |
        dotnet publish Letters.API/Letters.API.csproj -c Release -o publish
    
    - name: Deploy to Azure
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'letters-app-seu-nome'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

## Configurações Importantes

### 1. CORS no Ambiente de Produção

Se você tiver problemas de CORS, atualize o `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("https://seu-app.azurewebsites.net")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

### 2. Connection String do Banco de Dados

No `appsettings.json`, use variável de ambiente:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "#{ConnectionString}#"
  }
}
```

### 3. Health Check (Opcional)

Adicione no `Program.cs`:

```csharp
builder.Services.AddHealthChecks();

// ...

app.MapHealthChecks("/health");
```

## Verificação Pós-Deploy

1. Acesse: `https://seu-app.azurewebsites.net`
2. Teste o Swagger: `https://seu-app.azurewebsites.net/swagger`
3. Verifique os logs: Portal Azure > "Stream de log"

## Custos Estimados

- **F1 (Free)**: Grátis - 60 min/dia, 1GB RAM
- **B1 (Basic)**: ~R$ 70/mês - 100 horas totais de ACU, 1.75GB RAM
- **S1 (Standard)**: ~R$ 350/mês - Suporta autoscale, slots de deployment

## Solução de Problemas

### Erro 500 ao acessar

- Verifique os logs no Portal Azure
- Confirme as variáveis de ambiente
- Teste a connection string do banco

### Angular não carrega

- Confirme que `wwwroot` tem os arquivos do Angular
- Verifique se `UseStaticFiles()` e `MapFallbackToFile()` estão no Program.cs

### API funciona mas Angular não

- Verifique o `outputPath` no `angular.json`
- Confirme que copiou da pasta correta (`dist/letters.angular/browser`)

## Recursos Adicionais

- [Documentação Azure App Service](https://docs.microsoft.com/azure/app-service/)
- [Deploy ASP.NET Core no Azure](https://docs.microsoft.com/aspnet/core/host-and-deploy/azure-apps/)
- [Azure Pricing Calculator](https://azure.microsoft.com/pricing/calculator/)
