# Azure Deployment Rehberi - LightNap

Bu rehber LightNap uygulamasını Azure'a deploy etmek için gerekli adımları içerir.

## 📋 Ön Gereksinimler

### 1. Azure CLI Kurulumu
```bash
# macOS için
brew install azure-cli

# Windows için
winget install Microsoft.AzureCLI

# Linux için
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

### 2. Azure CLI Login
```bash
az login
```

### 3. Docker Kurulumu
```bash
# macOS için
brew install --cask docker

# Windows için
# Docker Desktop'ı indirin ve kurun

# Linux için
sudo apt-get update
sudo apt-get install docker.io
```

## 🚀 Hızlı Deployment

### Otomatik Deployment Script
```bash
# Script'i çalıştırılabilir yapın
chmod +x deploy-to-azure.sh

# Deployment'ı başlatın
./deploy-to-azure.sh
```

## 📦 Manuel Deployment Adımları

### 1. Resource Group Oluşturma
```bash
az group create --name lightnap-rg --location "West Europe"
```

### 2. Container Registry Oluşturma
```bash
az acr create --resource-group lightnap-rg --name lightnapregistry --sku Basic --admin-enabled true
```

### 3. SQL Server ve Database Oluşturma
```bash
# SQL Server
az sql server create \
    --resource-group lightnap-rg \
    --name lightnap-sqlserver \
    --location "West Europe" \
    --admin-user lightnapadmin \
    --admin-password "YourStrongPassword123!"

# Database
az sql db create \
    --resource-group lightnap-rg \
    --server lightnap-sqlserver \
    --name LightNap \
    --edition Basic
```

### 4. App Service Plan Oluşturma
```bash
az appservice plan create \
    --resource-group lightnap-rg \
    --name lightnap-appservice-plan \
    --sku P1v3 \
    --is-linux
```

### 5. App Services Oluşturma
```bash
# Web API App Service
az webapp create \
    --resource-group lightnap-rg \
    --plan lightnap-appservice-plan \
    --name lightnap-webapi \
    --deployment-local-git

# Maintenance App Service
az webapp create \
    --resource-group lightnap-rg \
    --plan lightnap-appservice-plan \
    --name lightnap-maintenance \
    --deployment-local-git
```

### 6. Docker Image'ları Build ve Push
```bash
# Container Registry'ye login
az acr login --name lightnapregistry

# Web API image build ve push
docker build -t lightnapregistry.azurecr.io/lightnap-webapi:latest -f LightNap.WebApi/Dockerfile .
docker push lightnapregistry.azurecr.io/lightnap-webapi:latest

# Maintenance Service image build ve push
docker build -t lightnapregistry.azurecr.io/lightnap-maintenance:latest -f LightNap.MaintenanceService/Dockerfile .
docker push lightnapregistry.azurecr.io/lightnap-maintenance:latest
```

### 7. App Service Container Ayarları
```bash
# Web API container ayarları
az webapp config container set \
    --resource-group lightnap-rg \
    --name lightnap-webapi \
    --docker-custom-image-name lightnapregistry.azurecr.io/lightnap-webapi:latest

# Maintenance Service container ayarları
az webapp config container set \
    --resource-group lightnap-rg \
    --name lightnap-maintenance \
    --docker-custom-image-name lightnapregistry.azurecr.io/lightnap-maintenance:latest
```

### 8. Environment Variables Ayarlama
```bash
# Web API App Settings
az webapp config appsettings set \
    --resource-group lightnap-rg \
    --name lightnap-webapi \
    --settings \
    "DatabaseProvider=SqlServer" \
    "ConnectionStrings__DefaultConnection=Server=tcp:lightnap-sqlserver.database.windows.net,1433;Database=LightNap;User ID=lightnapadmin;Password=YourStrongPassword123!;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;" \
    "Jwt__Key=YourSuperSecretJwtKeyForProductionEnvironment2024!" \
    "Jwt__Issuer=https://lightnap-webapi.azurewebsites.net" \
    "Jwt__Audience=https://lightnap-webapi.azurewebsites.net" \
    "Email__Provider=LogToConsole" \
    "Email__FromEmail=noreply@yourdomain.com" \
    "Email__FromDisplayName=LightNap System" \
    "ASPNETCORE_ENVIRONMENT=Production"

# Maintenance App Settings
az webapp config appsettings set \
    --resource-group lightnap-rg \
    --name lightnap-maintenance \
    --settings \
    "ConnectionStrings__DefaultConnection=Server=tcp:lightnap-sqlserver.database.windows.net,1433;Database=LightNap;User ID=lightnapadmin;Password=YourStrongPassword123!;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;" \
    "ASPNETCORE_ENVIRONMENT=Production"
```

### 9. Angular Uygulamasını Build Etme
```bash
cd lightnap-ng
npm install
npm run build --prod
cd ..

# Angular build'ini Web API'ye kopyalama
cp -r lightnap-ng/dist/* LightNap.WebApi/wwwroot/
```

## 🔐 Güvenlik Konfigürasyonları

### 1. Key Vault Kullanımı
```bash
# Key Vault oluşturma
az keyvault create \
    --resource-group lightnap-rg \
    --name lightnap-keyvault \
    --location "West Europe"

# Hassas bilgileri Key Vault'a ekleme
az keyvault secret set --vault-name lightnap-keyvault --name "JwtKey" --value "YourSuperSecretJwtKeyForProductionEnvironment2024!"
az keyvault secret set --vault-name lightnap-keyvault --name "SqlPassword" --value "YourStrongPassword123!"
```

### 2. Managed Identity Kullanımı
```bash
# App Service için Managed Identity oluşturma
az webapp identity assign \
    --resource-group lightnap-rg \
    --name lightnap-webapi

# Key Vault erişim politikası
az keyvault set-policy \
    --name lightnap-keyvault \
    --object-id $(az webapp identity show --resource-group lightnap-rg --name lightnap-webapi --query principalId -o tsv) \
    --secret-permissions get list
```

## 📊 Monitoring ve Logging

### 1. Application Insights
```bash
# Application Insights oluşturma
az monitor app-insights component create \
    --resource-group lightnap-rg \
    --app lightnap-insights \
    --location "West Europe" \
    --kind web
```

### 2. Log Analytics
```bash
# Log Analytics Workspace oluşturma
az monitor log-analytics workspace create \
    --resource-group lightnap-rg \
    --workspace-name lightnap-logs
```

## 🔄 CI/CD Pipeline

### Azure DevOps Pipeline
```yaml
# azure-deploy.yml dosyasını kullanın
# Bu dosya otomatik build ve deployment sağlar
```

### GitHub Actions
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
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - name: Build and Deploy
      run: |
        dotnet build --configuration Release
        dotnet publish -c Release -o ./publish
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'lightnap-webapi'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

## 🛠️ Troubleshooting

### Yaygın Sorunlar ve Çözümleri

1. **Docker Image Pull Hatası**
   ```bash
   # Container Registry credentials'ları kontrol edin
   az acr credential show --name lightnapregistry
   ```

2. **SQL Server Bağlantı Hatası**
   ```bash
   # Firewall kurallarını kontrol edin
   az sql server firewall-rule create \
       --resource-group lightnap-rg \
       --server lightnap-sqlserver \
       --name AllowAzureServices \
       --start-ip-address 0.0.0.0 \
       --end-ip-address 0.0.0.0
   ```

3. **App Service Logları**
   ```bash
   # Logları görüntüleme
   az webapp log tail --resource-group lightnap-rg --name lightnap-webapi
   ```

## 📈 Cost Optimization

### 1. App Service Plan Optimizasyonu
- Development için: B1 (Basic)
- Production için: P1v3 (Premium V3)

### 2. SQL Database Optimizasyonu
- Development için: Basic (5 DTU)
- Production için: Standard S1 (20 DTU)

### 3. Container Registry Optimizasyonu
- Basic SKU: Aylık 10GB storage
- Standard SKU: Aylık 100GB storage

## 🔍 Monitoring Dashboard

### Azure Portal'da İzleme
1. Resource Group'a gidin
2. App Services'i seçin
3. Monitoring sekmesini kontrol edin
4. Application Insights'ı yapılandırın

### Custom Metrics
```csharp
// Application Insights için custom metrics
using Microsoft.ApplicationInsights;

var telemetry = new TelemetryClient();
telemetry.TrackMetric("ActiveUsers", activeUserCount);
```

## 📞 Destek

Herhangi bir sorun yaşarsanız:
1. Azure Portal'da Resource Group'u kontrol edin
2. App Service loglarını inceleyin
3. Application Insights'ta hataları araştırın
4. Azure CLI ile resource durumlarını kontrol edin

---

**Not:** Bu rehber production ortamı için temel konfigürasyonları içerir. Güvenlik, performans ve maliyet optimizasyonu için ek konfigürasyonlar gerekebilir. 