# Portfolio Tracker API

A backend-only .NET 8 application that models investment portfolios, holdings, and transactions. Designed with production-oriented patterns including EF Core migrations, dependency injection, multi-stage Docker build, and optional Ansible-based deployment.

---

## Architecture

**Tech Stack**

* .NET 8 Web API
* Entity Framework Core (SQLite provider)
* Docker (multi-stage build)
* Ansible (optional deployment automation)

**Data Model**

* Users
* Portfolios (1 User → n Portfolios)
* Holdings (1 Portfolio → n Holdings)
* Transactions (1 Portfolio → n Transactions; also linked to User)

Schema definition is located in [docs/schema.md](/PortfolioTrackerApi/docs/schema.md) .
Database schema is created by EF Core migrations located in `PortfolioTrackerApi/Migrations/`.

---

### Endpoints

High-level route overview.

### Users

- `GET    /api/User` – list users
- `GET    /api/User/{id}` – get a single user
- `POST   /api/User` – create user
- `PUT    /api/User/{id}` – update user
- `DELETE /api/User/{id}` – delete/deactivate user (if enabled)

### Portfolios

- `GET    /api/Portfolio` – list portfolios
- `GET    /api/Portfolio/{id}` – get single portfolio with holdings and basic info
- `POST   /api/Portfolio` – create portfolio
- `PUT    /api/Portfolio/{id}` – update portfolio metadata / cash
- `DELETE /api/Portfolio/{id}` – delete portfolio

### Holdings

- `GET    /api/Holding` – list holdings (optionally filter by portfolio)
- `GET    /api/Holding/{id}` – get single holding
- `POST   /api/Holding` – create holding
- `PUT    /api/Holding/{id}` – update holding (qty, cost basis, etc.)
- `DELETE /api/Holding/{id}` – delete holding

### Transactions

- `GET    /api/Transaction` – list transactions (filterable by portfolio, symbol, date)
- `GET    /api/Transaction/{id}` – get single transaction
- `POST   /api/Transaction` – create transaction (Buy/Sell/Deposit/etc.)
- `DELETE /api/Transaction/{id}` – delete transaction (if allowed)

#### See [API reference](/PortfolioTrackerApi/docs/api.md) and [data schema](/PortfolioTrackerApi/docs/schema.md) for details.
---

## Running Locally

### Prerequisites

* .NET SDK 8
* SQLite (optional; DB file is created automatically)

### Development run

```bash
cd PortfolioTrackerApi
dotnet restore
dotnet run
```

API will start on the port configured in launch settings (commonly 5233 or 7000+ for HTTPS).

Swagger available at:

```
http://localhost:{port}/swagger
```

---

## Docker

### Build

```bash
cd PortfolioTrackerApi
docker build -t portfolio-tracker-api .
```

### Run

```bash
docker run --rm -p 8080:8080 portfolio-tracker-api
```

Test:

```bash
curl http://localhost:8080/api/User
```

---

## Deployment (Optional)

Deployment automation is provided via Ansible.

### Files:

```
infra/
  inventory.ini
  deploy.yml
```

### Run deployment

```bash
ansible-galaxy collection install community.docker
ansible-playbook -i infra/inventory.ini infra/deploy.yml
```

This installs Docker on the target host, syncs source code, builds the image, and starts the container.

---

## Project Layout

```
PortfolioTrackerApi/
  Controllers/
  Models/
  Data/
  Migrations/
  Dtos/
  Dockerfile
  appsettings.json
  appsettings.Development.json
  Program.cs
docs/
  schema.md
infra/ (optional)
  inventory.ini
  deploy.yml
```