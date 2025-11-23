# Deploy Letters - Resumo Rápido

## ✅ Configurações Realizadas

1. **Program.cs** - Configurado para servir arquivos estáticos do Angular
2. **Environment Files** - Criados para usar URLs relativas em produção
3. **Services** - Atualizados para usar variáveis de ambiente
4. **Scripts** - Criados para build e deploy

## 🚀 Deploy Rápido (Opção Recomendada)

### Via Azure Portal (Mais Simples)

```powershell
# 1. Build local
.\build-and-publish.ps1

# 2. Compactar
Compress-Archive -Path .\publish\* -DestinationPath publish.zip -Force

# 3. No Portal Azure:
#    - Criar Web App (.NET 8)
#    - Centro de Implantação > ZIP Deploy
#    - Upload do publish.zip
```

### Via Azure CLI (Mais Rápido)

```powershell
# Login
az login

# Criar recursos
az group create --name rg-letters --location brazilsouth

az appservice plan create --name letters-plan --resource-group rg-letters --sku B1 --is-linux

az webapp create --name letters-app-[SEU-NOME] --resource-group rg-letters --plan letters-plan --runtime "DOTNETCORE:8.0"

# Build e Deploy
.\build-and-publish.ps1
Compress-Archive -Path .\publish\* -DestinationPath publish.zip -Force

az webapp deployment source config-zip --resource-group rg-letters --name letters-app-[SEU-NOME] --src publish.zip

# Configurar variáveis
az webapp config appsettings set --resource-group rg-letters --name letters-app-[SEU-NOME] --settings ASPNETCORE_ENVIRONMENT=Production "ConnectionStrings__DefaultConnection=[SUA-CONNECTION-STRING]" "OpenAI__ApiKey=[SUA-KEY]" "OpenAI__Endpoint=[SEU-ENDPOINT]" "OpenAI__DeploymentName=[SEU-DEPLOYMENT]"
```

## 🧪 Testar Localmente Primeiro

```powershell
.\test-local-build.ps1
# Acesse: http://localhost:5000
```

## 📋 Configurações Necessárias no Azure

Após o deploy, configure no Portal Azure > Configuração:

- `ASPNETCORE_ENVIRONMENT` = Production
- `ConnectionStrings__DefaultConnection` = [Azure SQL Connection String]
- `OpenAI__ApiKey` = [Sua chave OpenAI]
- `OpenAI__Endpoint` = [Seu endpoint OpenAI]
- `OpenAI__DeploymentName` = [Nome do deployment]

## 📦 Custos Estimados

- **F1 (Free)**: Grátis - 60 min/dia
- **B1 (Basic)**: ~R$ 70/mês ⭐ Recomendado
- **S1 (Standard)**: ~R$ 350/mês

## 📚 Arquivos Importantes

- `DEPLOY-GUIDE.md` - Guia completo de deploy
- `build-and-publish.ps1` - Script de build e publicação
- `test-local-build.ps1` - Testar localmente
- `azure-deploy-template.json` - Template ARM para Azure

## ⚠️ Problemas Comuns

1. **Erro 500**: Verificar logs no Portal Azure
2. **Angular não carrega**: Confirmar que wwwroot existe
3. **CORS error**: Atualizar origens permitidas no Program.cs
4. **Database error**: Verificar connection string

## 🔗 Links Úteis

- [Portal Azure](https://portal.azure.com)
- [Azure Pricing Calculator](https://azure.microsoft.com/pricing/calculator/)
- Documentação completa: Ver `DEPLOY-GUIDE.md`
