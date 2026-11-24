# Commerce Bank Budgeting App

A responsive, full-stack financial management web application built with Microsoft Blazor and Entity Framework Core. This system was developed as a high-fidelity prototype for Commerce Bank to demonstrate modern budgeting solutions and AI-integrated financial advisory.

## Core Features

* **Secure Authentication:** User lifecycle management including signup, login, and prototype password recovery via Twilio SendGrid.
* **Dynamic Budgeting:** Robust tracking systems allowing users to create, manage, and monitor budgets across custom date ranges.
* **Transaction Engine:** Real-time logging and account-linked financial transaction history.
* **AI Financial Advisor:** Advanced integration with the Ollama AI chatbot to provide personalized, data-driven financial tips.

## Technology Stack

* **Frontend:** Microsoft Blazor (WebAssembly/Server)
* **Backend:** ASP.NET Core
* **ORM/Database:** Entity Framework Core with Local SQL Database integration
* **External Services:** * Twilio SendGrid (Email Communication)
    * Ollama (Local LLM for AI Advisory)

## System Architecture

The application leverages the **ASP.NET Core** framework to maintain a clean separation between the UI and the data layer. By using **Entity Framework Core**, the system ensures type-safe database queries and scalable data modeling, suitable for financial applications.



## Setup Instructions

1. **Clone the Repository**:
   ```bash
   git clone [https://github.com/greglinv/Commerce-Bank-Budgeting-App.git](https://github.com/greglinv/Commerce-Bank-Budgeting-App.git)
   cd Commerce-Bank-Budgeting-App
   ```
2. **Database Migration: Ensure LocalDB is installed and run dotnet ef database update.**
3. **Run Application: Execute dotnet run to launch the Blazor server.**

## Configuration

Secrets are never stored in source control. Locally, use .NET user secrets (from the `BlazorApp` folder); in Azure, use App Service environment variables (`:` becomes `__`, e.g. `Twilio__AuthToken`).

| Setting | Purpose | Default |
|---|---|---|
| `ConnectionStrings:BlazorAppContext` | SQL Server / Azure SQL connection | LocalDB (`appsettings.json`) |
| `Twilio:AccountSid`, `Twilio:AuthToken`, `Twilio:VerifyServiceSid` | Password-recovery email via Twilio Verify | none (recovery shows "not configured") |
| `Ollama:Url`, `Ollama:Model` | Ollama server for AI features | `http://localhost:11434`, `llama3.2` |
| `EnableDebugPages` | Serve the scaffolded CRUD pages (`/useraccounts`, `/transactions`, ...) outside Development | `false` |

```powershell
dotnet user-secrets set "Twilio:AccountSid" "<your account SID>"
dotnet user-secrets set "Twilio:AuthToken" "<your auth token>"
dotnet user-secrets set "Twilio:VerifyServiceSid" "<your Verify service SID>"
```

## Deploying to Azure (free tiers)

Uses an App Service **Free F1** plan and the **Azure SQL Database free offer**.

1. **Database:** In the Azure portal, create an Azure SQL Database and click **Apply offer** for the free offer. Set *behavior when free limit is reached* to **Auto-pause the database until next month** (prevents charges). Use SQL authentication. Under Networking, allow Azure services and add your client IP.
2. **Schema:** From the `BlazorApp` folder, apply the migrations to Azure SQL:
   ```powershell
   dotnet tool restore
   dotnet ef database update --connection "Server=tcp:<server>.database.windows.net,1433;Database=<db>;User ID=<user>;Password=<password>;Encrypt=True;"
   ```
3. **Web app:** Create an App Service (Publish: Code, Runtime: .NET, Pricing plan: **Free F1**). Under *Configuration*, turn **Web sockets On** and **ARR affinity On** (required by Blazor Server). Under *Environment variables*, add a connection string named `BlazorAppContext` (type **SQLAzure**) and the `Twilio__*` settings.
4. **Publish:** In Visual Studio, right-click the project → **Publish** → Azure → App Service, and pick the web app. (If you use GitHub Actions instead, the project lives in `BlazorApp/`, not the repo root.)

**Free-tier notes:** F1 has no Always On, so the first request after idle is slow; the free database also pauses and resumes (the app retries the connection automatically). F1 allows about 5 concurrent WebSocket connections, i.e. ~5 simultaneous users. There is no Ollama server in Azure, so AI features need `Ollama:Url` pointed at a reachable server.
