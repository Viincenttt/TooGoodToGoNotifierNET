resource "azurerm_storage_account" "tgtg_notifier_function_storage" {
  name                     = "storagetgtgnotifier${var.environment}"
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
}