#!/bin/bash

# Azure Deployment Script for LightNap
# Bu script Azure'da gerekli kaynakları oluşturur ve uygulamayı deploy eder

set -e

# Configuration
RESOURCE_GROUP_NAME="lightnap-rg"
LOCATION="West Europe"
ACR_NAME="lightnapregistry"
WEB_API_APP_NAME="lightnap-webapi"
MAINTENANCE_APP_NAME="lightnap-maintenance"
SQL_SERVER_NAME="lightnap-sqlserver"
SQL_DATABASE_NAME="LightNap"
KEY_VAULT_NAME="lightnap-keyvault"

echo "🚀 LightNap Azure Deployment başlatılıyor..."

# Azure CLI login kontrolü
echo "📋 Azure CLI login durumu kontrol ediliyor..."
az account show > /dev/null 2>&1 || {
    echo "❌ Azure CLI'da login yapılmamış. Lütfen 'az login' komutunu çalıştırın."
    exit 1
}

# Resource Group oluşturma
echo "📦 Resource Group oluşturuluyor: $RESOURCE_GROUP_NAME"
az group create --name $RESOURCE_GROUP_NAME --location $LOCATION

# Container Registry oluşturma
echo "🐳 Container Registry oluşturuluyor: $ACR_NAME"
az acr create --resource-group $RESOURCE_GROUP_NAME --name $ACR_NAME --sku Basic --admin-enabled true

# SQL Server oluşturma
echo "🗄️ SQL Server oluşturuluyor: $SQL_SERVER_NAME"
az sql server create \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $SQL_SERVER_NAME \
    --location $LOCATION \
    --admin-user lightnapadmin \
    --admin-password "YourStrongPassword123!"

# SQL Database oluşturma
echo "📊 SQL Database oluşturuluyor: $SQL_DATABASE_NAME"
az sql db create \
    --resource-group $RESOURCE_GROUP_NAME \
    --server $SQL_SERVER_NAME \
    --name $SQL_DATABASE_NAME \
    --edition Basic

# App Service Plan oluşturma
echo "🏗️ App Service Plan oluşturuluyor..."
az appservice plan create \
    --resource-group $RESOURCE_GROUP_NAME \
    --name "lightnap-appservice-plan" \
    --sku P1v3 \
    --is-linux

# Web API App Service oluşturma
echo "🌐 Web API App Service oluşturuluyor: $WEB_API_APP_NAME"
az webapp create \
    --resource-group $RESOURCE_GROUP_NAME \
    --plan "lightnap-appservice-plan" \
    --name $WEB_API_APP_NAME \
    --deployment-local-git

# Maintenance App Service oluşturma
echo "🔧 Maintenance App Service oluşturuluyor: $MAINTENANCE_APP_NAME"
az webapp create \
    --resource-group $RESOURCE_GROUP_NAME \
    --plan "lightnap-appservice-plan" \
    --name $MAINTENANCE_APP_NAME \
    --deployment-local-git

# Docker container ayarları
echo "🐳 Docker container ayarları yapılandırılıyor..."

# Web API için Docker ayarları
az webapp config container set \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $WEB_API_APP_NAME \
    --docker-custom-image-name "mcr.microsoft.com/dotnet/aspnet:9.0" \
    --docker-registry-server-url "https://$ACR_NAME.azurecr.io" \
    --docker-registry-server-user $(az acr credential show --name $ACR_NAME --query username -o tsv) \
    --docker-registry-server-password $(az acr credential show --name $ACR_NAME --query passwords[0].value -o tsv)

# Maintenance Service için Docker ayarları
az webapp config container set \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $MAINTENANCE_APP_NAME \
    --docker-custom-image-name "mcr.microsoft.com/dotnet/aspnet:9.0" \
    --docker-registry-server-url "https://$ACR_NAME.azurecr.io" \
    --docker-registry-server-user $(az acr credential show --name $ACR_NAME --query username -o tsv) \
    --docker-registry-server-password $(az acr credential show --name $ACR_NAME --query passwords[0].value -o tsv)

# Environment variables ayarlama
echo "⚙️ Environment variables ayarlanıyor..."

# Web API App Settings
az webapp config appsettings set \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $WEB_API_APP_NAME \
    --settings \
    "DatabaseProvider=SqlServer" \
    "ConnectionStrings__DefaultConnection=Server=tcp:$SQL_SERVER_NAME.database.windows.net,1433;Database=$SQL_DATABASE_NAME;User ID=lightnapadmin;Password=YourStrongPassword123!;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;" \
    "Jwt__Key=YourSuperSecretJwtKeyForProductionEnvironment2024!" \
    "Jwt__Issuer=https://$WEB_API_APP_NAME.azurewebsites.net" \
    "Jwt__Audience=https://$WEB_API_APP_NAME.azurewebsites.net" \
    "Email__Provider=LogToConsole" \
    "Email__FromEmail=noreply@yourdomain.com" \
    "Email__FromDisplayName=LightNap System" \
    "ASPNETCORE_ENVIRONMENT=Production"

# Maintenance App Settings
az webapp config appsettings set \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $MAINTENANCE_APP_NAME \
    --settings \
    "ConnectionStrings__DefaultConnection=Server=tcp:$SQL_SERVER_NAME.database.windows.net,1433;Database=$SQL_DATABASE_NAME;User ID=lightnapadmin;Password=YourStrongPassword123!;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;" \
    "ASPNETCORE_ENVIRONMENT=Production"

# Docker image'ları build ve push
echo "🔨 Docker image'ları build ediliyor ve push ediliyor..."

# Web API image build ve push
echo "📦 Web API Docker image build ediliyor..."
docker build -t $ACR_NAME.azurecr.io/lightnap-webapi:latest -f LightNap.WebApi/Dockerfile .
docker push $ACR_NAME.azurecr.io/lightnap-webapi:latest

# Maintenance Service image build ve push
echo "📦 Maintenance Service Docker image build ediliyor..."
docker build -t $ACR_NAME.azurecr.io/lightnap-maintenance:latest -f LightNap.MaintenanceService/Dockerfile .
docker push $ACR_NAME.azurecr.io/lightnap-maintenance:latest

# Angular uygulamasını build etme
echo "🎨 Angular uygulaması build ediliyor..."
cd lightnap-ng
npm install
npm run build --prod
cd ..

# Angular build'ini Web API'ye kopyalama
echo "📁 Angular build dosyaları Web API'ye kopyalanıyor..."
cp -r lightnap-ng/dist/* LightNap.WebApi/wwwroot/

# Deployment tamamlandı
echo "✅ Azure deployment tamamlandı!"
echo ""
echo "📋 Deployment Bilgileri:"
echo "Resource Group: $RESOURCE_GROUP_NAME"
echo "Web API URL: https://$WEB_API_APP_NAME.azurewebsites.net"
echo "Maintenance Service URL: https://$MAINTENANCE_APP_NAME.azurewebsites.net"
echo "SQL Server: $SQL_SERVER_NAME.database.windows.net"
echo "Container Registry: $ACR_NAME.azurecr.io"
echo ""
echo "🔐 Güvenlik Notları:"
echo "- SQL Server şifresini değiştirin"
echo "- JWT anahtarını güvenli bir şekilde saklayın"
echo "- Key Vault kullanarak hassas bilgileri yönetin"
echo ""
echo "🚀 Uygulamanız hazır!" 