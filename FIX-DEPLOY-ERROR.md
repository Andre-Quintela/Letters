# Fix: Deploy Error - Windows vs Linux Path Issue

## ❌ Problema
Você criou um **App Service Linux**, mas o ZIP foi criado no Windows com caminhos usando `\` (barras invertidas), incompatíveis com Linux que usa `/`.

## ✅ Solução Recomendada: Usar Windows App Service

### Passo a Passo:

1. **Delete o App Service Linux atual** (ou deixe para depois)

2. **Criar novo App Service Windows:**

   ```powershell
   # Via Azure CLI
   az appservice plan create `
     --name letters-plan-win `
     --resource-group rg-letters `
     --sku B1 `
     --is-linux false
   
   az webapp create `
     --name letters-app-seu-nome `
     --resource-group rg-letters `
     --plan letters-plan-win `
     --runtime "DOTNET:8"
   ```

   **OU via Portal Azure:**
   - Criar recurso > Web App
   - **Sistema Operacional**: **Windows** ⚠️
   - **Publicar**: Código
   - **Stack de runtime**: .NET 8
   - **Região**: Brazil South
   - **Plano**: B1

3. **Deploy com o ZIP que você já tem:**

   ```powershell
   # Você já tem o publish.zip criado, basta fazer deploy:
   az webapp deployment source config-zip `
     --resource-group rg-letters `
     --name letters-app-seu-nome `
     --src publish.zip
   ```

   **OU via Portal:**
   - Vá no Web App criado
   - Centro de Implantação > Zip Deploy
   - Upload do `publish.zip`

## 🐧 Alternativa: Deploy para Linux (Mais Complexo)

Se você realmente precisa usar Linux:

```powershell
# Execute o script específico para Linux
.\build-and-publish-linux.ps1

# Deploy do ZIP gerado
az webapp deployment source config-zip `
  --resource-group rg-letters `
  --name letters-app-seu-nome `
  --src publish-linux.zip
```

## 💡 Recomendação

**Use Windows App Service** porque:
- ✅ Mais simples e direto
- ✅ Melhor compatibilidade com .NET
- ✅ Mesmo preço que Linux
- ✅ Menos problemas de compatibilidade
- ✅ Você já tem o ZIP pronto

## ⚙️ Configurações Pós-Deploy (Ambos)

Após deploy bem-sucedido, configure as variáveis de ambiente:

```powershell
az webapp config appsettings set `
  --resource-group rg-letters `
  --name letters-app-seu-nome `
  --settings `
    ASPNETCORE_ENVIRONMENT=Production `
    "ConnectionStrings__DefaultConnection=sua-connection-string" `
    "OpenAI__ApiKey=sua-key" `
    "OpenAI__Endpoint=seu-endpoint" `
    "OpenAI__DeploymentName=seu-deployment"
```
