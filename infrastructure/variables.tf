variable "environment" {
  type        = string
  description = "Environment name"
  validation {
    condition     = contains(["dev", "acc", "prd"], var.environment)
    error_message = "Environment name must be one of: [dev, acc, prd]"
  }
}

variable "location" {
  type    = string
  default = "westeurope"
  description = "The location of the resource group"
}