resource "azurerm_storage_account" "tgtg_notifier_function_storage" {
  name                     = "stortgtgnotifier${var.environment}"
  resource_group_name      = azurerm_resource_group.tgtg_rg.name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_storage_account" "tgtg_notifier_itemcache_storage" {
  name                     = "storegtgnotifiercache${var.environment}"
  resource_group_name      = azurerm_resource_group.tgtg_rg.name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_service_plan" "tgtg_notifier_function_service_plan" {
  name                = "sp-tgtgnotifier-app-${var.environment}"
  location            = var.location
  resource_group_name = azurerm_resource_group.tgtg_rg.name
  os_type             = "Windows"
  sku_name            = "Y1"
}

resource "azurerm_role_assignment" "tgtg_notifier_function_cache_role_assignment" {
  scope                = azurerm_storage_account.tgtg_notifier_itemcache_storage.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = azurerm_windows_function_app.tgtg_notifier_function.identity[0].principal_id
}

resource "azurerm_role_assignment" "tgtg_notifier_function_keyvault_role_assignment" {
  scope                = azurerm_key_vault.tgtg_notifier_keyvault.id
  role_definition_name = "Key Vault Administrator"
  principal_id         = azurerm_windows_function_app.tgtg_notifier_function.identity[0].principal_id
}

resource "azurerm_windows_function_app" "tgtg_notifier_function" {
  name                       = "func-tgtgnotifier-app-${var.environment}"
  location                   = var.location
  resource_group_name        = azurerm_resource_group.tgtg_rg.name
  service_plan_id            = azurerm_service_plan.tgtg_notifier_function_service_plan.id
  storage_account_name       = azurerm_storage_account.tgtg_notifier_function_storage.name
  storage_account_access_key = azurerm_storage_account.tgtg_notifier_function_storage.primary_access_key

  site_config {
    application_stack {
      dotnet_version              = "v7.0"
      use_dotnet_isolated_runtime = true
    }
  }

  app_settings = {
    BlobStorageCache__Blob = "items-cache.json"
    BlobStorageCache__Container = "toogoodtogoitemcache"
    BlobStorageCache__Uri = azurerm_storage_account.tgtg_notifier_itemcache_storage.primary_blob_endpoint
    FavoritesScannerTriggerTime = "0 */5 7-20 * * *"
    KeyvaultUri = azurerm_key_vault.tgtg_notifier_keyvault.vault_uri
    Notifications__Telegram__ChatId = var.telegram_chat_id
    Notifications__Telegram__BotToken = "todo"
    TooGoodToGo__Email = var.tgtg_user_email
  }

  identity {
    type = "SystemAssigned"
  }
}