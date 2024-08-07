data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "tgtg_notifier_keyvault" {
  name                        = "kv-tgtg${var.environment}"
  location                    = var.location
  resource_group_name         = azurerm_resource_group.tgtg_rg.name
  enabled_for_disk_encryption = true
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false

  sku_name = "standard"
}