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
2. **Database Migration: Ensure LocalDB is installed and run dotnet ef database update.**
3. **Run Application: Execute dotnet run to launch the Blazor server.**
