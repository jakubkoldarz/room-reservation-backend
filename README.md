# Room Reservation – Backend

ASP.NET Core (.NET 10) API with PostgreSQL. Runs with Docker Compose.

## Requirements

- Git
- Docker Desktop (with Docker Compose)

## Get the code

```bash
git clone https://github.com/jakubkoldarz/room-reservation-backend.git
cd room-reservation-backend
```

## Configure

Copy the example environment file:

```bash
cp .env.example .env
```

Then edit `.env`:

- `JWT_SECRET`: any random text, at least 32 characters.
- `SMTP_*`: a mailbox that sends verification codes and notifications (for Gmail: `smtp.gmail.com`, port `587` and an app password).
- `CORS__ALLOWED_ORIGINS__0`: the frontend address (default `http://localhost:5173`).

## Run

```bash
docker compose up --build
```

| Service  | Address                                |
| -------- | -------------------------------------- |
| API      | http://localhost:5000                  |
| API docs | http://localhost:5000/scalar           |
| pgAdmin  | http://localhost:8080 (`PGADMIN_PORT`) |

Migrations run automatically when the API starts. They also create the default roles (User, Receptionist, Manager, SuperAdmin) and their permissions.

## Seed the database

Seeding **deletes the whole database**, creates it again and adds test users.

1. Set `SEED_DATABASE=true` in `.env`.
2. Start the API: `docker compose up --build`. Wait for the log `Database was recreated and seeded`.
3. Set `SEED_DATABASE=false` in `.env` and run `docker compose up -d`. Otherwise the database is wiped on every start.

Seeded accounts:

| Email             | Password   | Role       |
| ----------------- | ---------- | ---------- |
| `admin@rooms.com` | `password` | SuperAdmin |
| `user@rooms.com`  | `password` | User       |

## Frontend

See [room-reservation-frontend](https://github.com/jakubkoldarz/room-reservation-frontend).
