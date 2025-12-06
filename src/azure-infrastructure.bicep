@description('The name of the resource group')
param resourceGroupName string

@description('The location for all resources')
param location string = resourceGroup().location

@description('The name of the App Service Plan')
param appServicePlanName string = 'lightnap-appservice-plan'

@description('The name of the Web API App Service')
param webApiAppName string = 'lightnap-webapi'

@description('The name of the Maintenance Service App Service')
param maintenanceAppName string = 'lightnap-maintenance'

@description('The name of the SQL Server')
param sqlServerName string = 'lightnap-sqlserver'

@description('The name of the SQL Database')
param sqlDatabaseName string = 'LightNap'

@description('The name of the Container Registry')
param acrName string = 'lightnapregistry'

@description('The name of the Key Vault')
param keyVaultName string = 'lightnap-keyvault'

@description('The name of the Storage Account')
param storageAccountName string = 'lightnapstorage'

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'P1v3'
    tier: 'PremiumV3'
  }
  kind: 'linux'
  reserved: true
}

// Container Registry
resource acr 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: acrName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}

// Storage Account
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
  }
}

// SQL Server
resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: 'lightnapadmin'
    administratorLoginPassword: 'YourStrongPassword123!'
    version: '12.0'
  }
}

// SQL Database
resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
  name: '${sqlServer.name}/${sqlDatabaseName}'
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
  }
}

// Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    accessPolicies: []
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
  }
}

// Web API App Service
resource webApiApp 'Microsoft.Web/sites@2023-01-01' = {
  name: webApiAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOCKER|mcr.microsoft.com/dotnet/aspnet:9.0'
      appSettings: [
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'false'
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_URL'
          value: 'https://${acr.loginServer}'
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_USERNAME'
          value: acr.listCredentials().username
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_PASSWORD'
          value: acr.listCredentials().passwords[0].value
        }
        {
          name: 'SQL_CONNECTION_STRING'
          value: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Database=${sqlDatabaseName};User ID=lightnapadmin;Password=YourStrongPassword123!;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;'
        }
        {
          name: 'JWT_KEY'
          value: 'YourSuperSecretJwtKeyForProductionEnvironment2024!'
        }
        {
          name: 'JWT_ISSUER'
          value: 'https://${webApiApp.properties.defaultHostName}'
        }
        {
          name: 'JWT_AUDIENCE'
          value: 'https://${webApiApp.properties.defaultHostName}'
        }
        {
          name: 'EMAIL_FROM'
          value: 'noreply@yourdomain.com'
        }
        {
          name: 'EMAIL_DISPLAY_NAME'
          value: 'LightNap System'
        }
        {
          name: 'SMTP_HOST'
          value: 'smtp.sendgrid.net'
        }
        {
          name: 'SMTP_USER'
          value: 'apikey'
        }
        {
          name: 'SMTP_PASSWORD'
          value: 'YourSendGridApiKey'
        }
      ]
    }
  }
}

// Maintenance Service App Service
resource maintenanceApp 'Microsoft.Web/sites@2023-01-01' = {
  name: maintenanceAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOCKER|mcr.microsoft.com/dotnet/aspnet:9.0'
      appSettings: [
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'false'
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_URL'
          value: 'https://${acr.loginServer}'
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_USERNAME'
          value: acr.listCredentials().username
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_PASSWORD'
          value: acr.listCredentials().passwords[0].value
        }
        {
          name: 'SQL_CONNECTION_STRING'
          value: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Database=${sqlDatabaseName};User ID=lightnapadmin;Password=YourStrongPassword123!;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;'
        }
      ]
    }
  }
}

// Outputs
output webApiUrl string = webApiApp.properties.defaultHostName
output maintenanceUrl string = maintenanceApp.properties.defaultHostName
output acrLoginServer string = acr.properties.loginServer
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName 