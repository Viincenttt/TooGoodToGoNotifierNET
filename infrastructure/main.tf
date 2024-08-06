resource "azurerm_resource_group" "tgtg_rg" {
  location = var.location
  name     = "rg-tgtgnotifier-app-${var.environment}"
}