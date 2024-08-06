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

variable "telegram_chat_id" {
  type    = string
  description = "The telegram chat id to send messages to"
}

variable "tgtg_user_email" {
  type    = string
  description = "The e-mail address of your tgtg account"
}