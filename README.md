# PeriodicIpCheck

Simple app to check if your IP has changed, logging this changes and sending email notifications. 

## Exmple config file:

```
{
  "ResultsFilePath": "ip_stats.txt",
  "LogFilePath": "ip_stats.log",
  "LogOnlyChanges": true,
  "NotificationEmail": "email_address_to_send_notifications",
  "SmtpServer": "your_smtp_server",
  "SmtpPort": 587,
  "SenderEmail": "your_email_address",
  "SenderPassword": "your_password"
}
```
