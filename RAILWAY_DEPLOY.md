# Deploy on Railway

This app is set up to deploy on [Railway](https://railway.app) using the **Dockerfile** (`railway.toml` → `builder = "DOCKERFILE"`).

## 1. Connect the repo

1. [Railway Dashboard](https://railway.app/dashboard) → **New Project** → **Deploy from GitHub repo**.
2. Select `AnastasiiaKhru/AnastasiiaPortfolio` and the **main** branch.
3. Railway will build from the Dockerfile and deploy on every push to **main**.

## 2. Environment variables

Configure these in your Railway service → **Variables**:

| Variable | Description |
|----------|-------------|
| `MongoDB__ConnectionString` | MongoDB Atlas connection string (required) |
| `MongoDB__DatabaseName` | e.g. `AnastasiiaPortfolio` |
| `EmailSettings__SmtpServer` | e.g. `smtp.gmail.com` (optional) |
| `EmailSettings__SmtpPassword` | Gmail app password if using SMTP (optional; contact form uses `mailto:`) |

Use **double underscore** `__` for nested keys (e.g. `MongoDB__ConnectionString`).

## 3. Port

The app listens on **0.0.0.0** and the **PORT** that Railway injects at runtime (default 8080). In Railway → **Settings** → **Networking** / your domain, ensure the **target port** matches (e.g. 8080).

## 4. Optional: custom domain

In your Railway service → **Settings** → **Domains**, add a custom domain or use the default `*.up.railway.app` URL.

---

## Troubleshooting: "This error appears to be caused by the application"

1. **Health check**  
   Open `https://<your-app>.up.railway.app/health`.  
   - If you see `ok`: the app is running; the failure is likely when using MongoDB (home page, etc.).  
   - If you get an error: the app may be failing at startup (e.g. missing config).

2. **Variables**  
   In Railway → **Variables**, ensure `MongoDB__ConnectionString` and `MongoDB__DatabaseName` are set.  
   If `MongoDB__ConnectionString` is missing in Production, the app throws at startup with a clear message.

3. **MongoDB Atlas network access**  
   In [MongoDB Atlas](https://cloud.mongodb.com) → your project → **Network Access** → **Add IP Address**  
   - Either add Railway’s IPs (if you use a static egress), or  
   - Use **Allow access from anywhere** (`0.0.0.0/0`) for testing.  
   Otherwise Atlas can block Railway and the app will error when loading the home page.

4. **Logs**  
   In Railway → **Deployments** → your deployment → **Deploy Logs** and **HTTP Logs**.  
   Reproduce the error (visit the site), then check for exception stack traces or 500 responses.

5. **"Application Failed to Respond" (502)**  
   The app must listen on **0.0.0.0** and the **PORT** Railway injects. This project does that in code. In Railway → **Settings** → your **domain**, set the **target port** to match (e.g. **8080**). If it was 3000 or something else, change it to 8080.
