param location string = resourceGroup().location
param redisName string = 'tgtg-redis'

resource redisCache 'Microsoft.Cache/redis@2023-04-01' = {
  location: location
  name: redisName
  properties: {
    sku: {
      name: 'Basic'
      capacity: 0
      family: 'C' 
    }
    redisVersion: '6'
    enableNonSslPort: false
    minimumTlsVersion: '1.2'
  }
}
