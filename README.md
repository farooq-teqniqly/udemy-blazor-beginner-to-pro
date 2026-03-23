# Udemy Blazor: Beginner to Pro

## Docker setup (do this first)

Complete these steps **before** running `docker compose up`. The stack expects a local `.env` file and `secrets/servers.json` (both are gitignored).

1. **Secrets for pgAdmin**

   - Create a `secrets` folder in the repository root.
   - Copy `servers.example.json` into `secrets/` and rename the copy to `servers.json`.
   - Open `secrets/servers.json` and replace the placeholder values. The `Username` value must match the PostgreSQL user you set in `.env` as `POSTGRES_USER`.

2. **Environment variables**

   - Copy `.env.example` to `.env` in the repository root.
   - Open `.env` and replace every placeholder with your real values (PostgreSQL credentials and pgAdmin login).

Then start the stack:

```bash
docker compose up
```

PostgreSQL is exposed on port `5432`, and pgAdmin on `http://localhost:8080`.
