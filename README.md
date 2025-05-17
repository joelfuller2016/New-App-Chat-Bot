# New-App-Chat-Bot

This repository contains a minimal sample project for a C# chat bot using the OpenAI API. The project is built with **ASP.NET WebForms** targeting **.NET Framework 4.8** and demonstrates how to:

- Configure different OpenAI models and API keys via an admin panel (protected by HTTP Basic authentication)
- Persist chat history and API responses in a SQLite database
- Configure Google OAuth credentials from the admin panel and initiate login
- View raw chat logs and errors through a debug page

For a detailed plan and folder structure see [docs/project-plan.md](docs/project-plan.md).

> **Build Requirements**: The solution targets **.NET Framework 4.8** and must be built on Windows using Visual Studio or MSBuild. .NET CLI on Linux is insufficient.

> **Note**: The implementation is intentionally lightweight and serves as a starting point. Some parts (e.g. Google OAuth flow) are provided as placeholders.

