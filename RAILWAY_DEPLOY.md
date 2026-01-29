# Deploy on Railway

This app is set up to deploy on [Railway](https://railway.app) using the **Dockerfile** (`railway.toml` → `builder = "DOCKERFILE"`).

## 1. Connect the repo

1. [Railway Dashboard](https://railway.app/dashboard) → **New Project** → **Deploy from GitHub repo**.
2. Select `AnastasiiaKhru/AnastasiiaPortfolio` and the **main** branch.
3. Railway will build from the Dockerfile and deploy on every push to **main**.

## 2. Environment variables

Configure these in your Railway service → **Variables** (or via `railway.toml` / CLI). Use the same structure as `appsettings.example.json`:

| Variable | Description |
|----------|-------------|
| `MongoDB__ConnectionString` | MongoDB Atlas connection string |
| `MongoDB__DatabaseName` | e.g. `AnastasiiaPortfolio` |
| `EmailSettings__SmtpServer` | e.g. `smtp.gmail.com` |
| `EmailSettings__SmtpPassword` | Gmail app password (if using SMTP); contact form uses `mailto:` so optional |

Use **double underscore** `__` for nested keys (e.g. `MongoDB__ConnectionString`).

## 3. Port

The Dockerfile sets `ASPNETCORE_URLS=http://+:${PORT}`. Railway injects `PORT` at runtime; no extra config needed.

## 4. Optional: custom domain

In your Railway service → **Settings** → **Domains**, add a custom domain or use the default `*.up.railway.app` URL.
