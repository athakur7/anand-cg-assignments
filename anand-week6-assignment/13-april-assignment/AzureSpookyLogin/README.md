# AzureSpookyLogin

`AzureSpookyLogin` is a .NET 8 ASP.NET Core web application that accepts a user submission, sends JSON to an Azure Logic App HTTP trigger, inserts the data into Azure SQL Database, and sends a Gmail notification after the database insert succeeds.

## Features

- ASP.NET Core MVC web app on .NET 8
- Form submission with server-side validation
- JSON payload sent to Azure Logic Apps
- Logic App workflow inserts rows into Azure SQL Database
- Gmail notification after successful insert
- Configuration-based Logic App callback URL

## Solution overview

### Application flow

1. User submits the form in the web app.
2. The app generates a unique `id`.
3. The app posts JSON to the Logic App HTTP trigger.
4. The Logic App inserts the payload into `dbo.SpookyRequests`.
5. The Logic App sends a Gmail email with the inserted values.

### JSON payload

The app sends this structure to the Logic App:

```json
{
  "id": "<guid>",
  "name": "<user name>",
  "email": "<user email>",
  "phone": "<user phone>"
}
```

## Prerequisites

- .NET 8 SDK
- Azure subscription access
- Azure SQL Database access
- Azure Logic App with:
  - HTTP request trigger
  - SQL insert action
  - Gmail send mail action

## Project files of interest

- `Controllers/HomeController.cs` - posts the JSON payload to the Logic App
- `Models/SpookyRequest.cs` - request model and validation
- `Views/Home/Index.cshtml` - form UI
- `Program.cs` - service registration and MVC pipeline
- `appsettings.json` - configuration
- `appsettings.Development.json` - local development settings
- `logicapp-at-logic-app3-definition.json` - Logic App workflow definition

## Configuration

The app reads the Logic App callback URL from configuration:

- `LogicApp:RequestUrl`

### Local development

Set the Logic App URL locally using one of these approaches:

#### Option 1: `appsettings.Development.json`

```json
{
  "LogicApp": {
    "RequestUrl": "<logic-app-http-trigger-url>"
  }
}
```

#### Option 2: Environment variable

```powershell
$env:LogicApp__RequestUrl = "<logic-app-http-trigger-url>"
```

## Security notes

- Do not commit Logic App callback URLs, passwords, connection strings, or OAuth secrets to git.
- Keep sensitive values in environment variables, user secrets, or Azure Key Vault.
- The checked-in configuration should contain placeholders only.
- Rotate any exposed secret URLs or passwords immediately if they were previously committed.

## Azure resources used

- Azure Logic App: `at-logic-app3`
- Azure SQL Server: `ananddevsqlci.database.windows.net`
- Azure SQL Database: `ananddb`
- SQL table: `dbo.SpookyRequests`
- Gmail connector for email notifications

## Logic App workflow

The workflow definition in `logicapp-at-logic-app3-definition.json` contains:

- Trigger: `When_an_HTTP_request_is_received`
- Action: `Insert_row_(V2)`
- Action: `Send_email_(Gmail)`

### Trigger schema

```json
{
  "type": "object",
  "properties": {
    "id": { "type": "string" },
    "name": { "type": "string" },
    "email": { "type": "string" },
    "phone": { "type": "string" }
  },
  "required": ["id", "name", "email", "phone"]
}
```

## Database table

The Logic App inserts records into `dbo.SpookyRequests`.

Expected columns:

- `Id` `NVARCHAR(64)` primary key
- `Name` `NVARCHAR(200)`
- `Email` `NVARCHAR(320)`
- `Phone` `NVARCHAR(30)`
- `CreatedAt` `DATETIME2`

## Run locally

1. Restore packages:

```powershell
dotnet restore
```

2. Set the Logic App URL:

```powershell
$env:LogicApp__RequestUrl = "<logic-app-http-trigger-url>"
```

3. Run the app:

```powershell
dotnet run
```

4. Open the site in your browser and submit the form.

## Test the Logic App manually

You can test the workflow directly with a JSON body:

```powershell
$body = @{
  id = [guid]::NewGuid().ToString()
  name = "Test User"
  email = "test@example.com"
  phone = "9876543210"
} | ConvertTo-Json

Invoke-RestMethod -Method Post `
  -Uri "<logic-app-http-trigger-url>" `
  -ContentType "application/json" `
  -Body $body
```

## Verify the database insert

```powershell
sqlcmd -S ananddevsqlci.database.windows.net -d ananddb -U anand -P "<sql-password>" -N -C -Q "SELECT TOP 10 Id, Name, Email, Phone, CreatedAt FROM dbo.SpookyRequests ORDER BY CreatedAt DESC;"
```

## Email notification

The Gmail action sends an email with:

- Subject: the submitted name and id
- Body: the submitted `id`, `name`, `email`, and `phone`

## Notes

- `appsettings.Development.json` should remain secret-free in git.
- Temporary workflow export files should not contain passwords or callback URLs.
- If you rotate the SQL password or Logic App URL, update the local configuration only.
