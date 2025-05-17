# Project Plan: Chat Bot Web Application

## Overview

This project is a minimal web application built with **ASP.NET WebForms (.NET Framework 4.8)**. It exposes a simple chat bot using the OpenAI API. An admin panel allows management of the API key, model selection and Google OAuth credentials. The panel is protected by HTTP Basic authentication. SQLite is used for storage. Chat logs and API responses are saved for debugging. A debug screen displays stored logs and errors.

## Goals

1. Self-contained ASP.NET WebApp site targeting .NET Framework 4.8.
2. SQLite database automatically created on application start if missing.
3. Admin panel with configuration for API key, model and Google OAuth credentials.
4. Admin panel protected via basic authentication.
5. Login via Google OAuth.
6. Chat page to interact with OpenAI.
7. Debug page showing raw JSON chat logs and errors.
8. Unit tests for core modules.

## Architecture

The application follows SOLID principles with small classes:

- `DbManager` handles database creation and queries.
- `OpenAIService` wraps calls to the OpenAI API.
- `AuthManager` provides Google OAuth integration (simplified placeholder).
- `ErrorLogger` persists application errors.
- WebForms pages (`.aspx`) interact with these services.

### Proposed Enhancements

- Introduce dedicated repository and service layers to cleanly separate data
  access from WebForms code-behind.
- Replace HTTP Basic authentication with a cookie-based approach that leverages
  the Google OAuth login.
- Store sensitive values (API key, admin password, Google secrets) in encrypted
  form and hash passwords with a per-user salt.
- Include anti-forgery tokens on all admin forms to mitigate CSRF attacks.
- Centralize HTTP calls via `HttpClient` instances managed by dependency
  injection to improve testability and performance.
- Add explicit references to `System.Web` so the project builds using
  cross-platform reference assemblies.


## Folder Structure

```
/ (repo root)
├─ README.md
├─ docs/
│  └─ project-plan.md
├─ src/
│  └─ ChatBot/
│     ├─ ChatBot.csproj
│     ├─ Global.asax
│     ├─ Web.config
│     ├─ App_Code/
│     │  ├─ DbManager.cs
│     │  ├─ OpenAIService.cs
│     │  ├─ ErrorLogger.cs
│     │  └─ AuthManager.cs
│     ├─ Admin/
│     │  ├─ Admin.aspx
│     │  └─ Admin.aspx.cs
│     ├─ Chat/
│     │  ├─ Chat.aspx
│     │  └─ Chat.aspx.cs
│     ├─ Debug/
│     │  ├─ Debug.aspx
│     │  └─ Debug.aspx.cs
│     └─ Login.aspx
├─ tests/
│  ├─ ChatBot.Tests.csproj
│  ├─ DbManagerTests.cs
│  └─ OpenAIServiceTests.cs
```

## Development Tasks

1. **Create SQLite helper**: builds DB with tables `Config`, `Chats`, `Errors` and inserts default admin credentials.
2. **Implement OpenAI service**: simple POST request to OpenAI API.
3. **Admin panel**: update configuration for API key, model and Google OAuth credentials. Protect the page with basic auth.
4. **Login page**: integrate Google OAuth (outline code only).
5. **Chat page**: send messages and display responses.
6. **Debug page**: show chat logs and errors in JSON format.
7. **Unit tests**: verify DB creation and API calls (mocked).
8. **Documentation**: keep README and doc files up to date.

## Notes

- This repository contains skeleton code suitable for extending. Some implementation details (e.g. Google OAuth flow) are placeholders and require completion in a real environment.
- The solution targets **.NET Framework 4.8**, which requires a Windows build environment.
- Bootstrap is included via CDN for styling.

