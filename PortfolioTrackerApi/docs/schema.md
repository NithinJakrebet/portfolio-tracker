# Data Schema

Source of truth for entity design and relationships for the Portfolio Tracker API. Runtime schema is created via EF Core migrations.

- Timestamps: all `DateTime` are UTC (`CreatedAt`, `UpdatedAt`, `ExecutedAt`).
- Money: `decimal(18,2)` for amounts; `decimal(18,6)` for quantities/prices.
- Enum: `TransactionType` stored as `int` (C# enum order).

---

## Users

| Column       | Type          | Nullable | Notes                         |
| ------------ | ------------- | -------- | ----------------------------- |
| UserId       | Guid (PK)     | No       | Primary key                   |
| Email        | string(255)   | No       | Unique; indexed               |
| PasswordHash | string        | No       | Application-managed hash      |
| FullName     | string        | No       | Display name                  |
| IsActive     | bool          | No       | Default `true`                |
| CreatedAt    | DateTime (UTC)| No       | Default now                   |
| UpdatedAt    | DateTime (UTC)| No       | Updated on changes            |

Indexes:

- Unique: `Email`

Relationships:

- 1 User → n Portfolios
- 1 User → n Transactions

---

## Portfolios

| Column      | Type          | Nullable | Notes                              |
| ----------- | ------------- | -------- | ---------------------------------- |
| PortfolioId | Guid (PK)     | No       | Primary key                        |
| UserId      | Guid (FK)     | No       | FK → Users.UserId                  |
| OwnerName   | string        | No       | Copied from user full name         |
| Label       | string        | No       | e.g., “Retirement”, “Brokerage”    |
| CashBalance | decimal(18,2) | No       | Current cash in portfolio currency |
| CreatedAt   | DateTime (UTC)| No       |                                    |
| UpdatedAt   | DateTime (UTC)| No       |                                    |

Indexes:

- Index: `UserId`

Relationships:

- 1 Portfolio → n Holdings
- 1 Portfolio → n Transactions

---

## Holdings

| Column       | Type          | Nullable | Notes                               |
| ------------ | ------------- | -------- | ----------------------------------- |
| HoldingId    | Guid (PK)     | No       | Primary key                         |
| PortfolioId  | Guid (FK)     | No       | FK → Portfolios.PortfolioId         |
| Symbol       | string        | No       | Ticker, e.g., `AAPL`                |
| Exchange     | string        | Yes      | e.g., `NASDAQ`, `NYSE`              |
| Quantity     | decimal(18,6) | No       | Supports fractional shares          |
| AvgCostBasis | decimal(18,2) | No       | Average per-share cost              |
| CreatedAt    | DateTime (UTC)| No       |                                     |
| UpdatedAt    | DateTime (UTC)| No       |                                     |

Possible indexes (via Fluent API if desired):

- Index: `PortfolioId`
- Index: `Symbol`
- Optional unique composite: (`PortfolioId`, `Symbol`)

---

## Transactions

| Column        | Type           | Nullable | Notes                                                                  |
| ------------- | -------------- | -------- | ---------------------------------------------------------------------- |
| TransactionId | Guid (PK)      | No       | Primary key                                                            |
| PortfolioId   | Guid (FK)      | No       | FK → Portfolios.PortfolioId                                            |
| UserId        | Guid (FK)      | No       | FK → Users.UserId                                                      |
| Type          | int (enum)     | No       | `0=Buy, 1=Sell, 2=Deposit, 3=Withdrawal, 4=Dividend, 5=Split, 6=Fee, 7=Interest` |
| Symbol        | string(12)     | Yes      | Required for position-related types (Buy/Sell/Dividend/Split/Fee)      |
| Quantity      | decimal(18,6)  | Yes      | For share-based operations                                             |
| Price         | decimal(18,6)  | Yes      | Per-unit price                                                         |
| GrossAmount   | decimal(18,2)  | No       | Total cash impact of the transaction                                  |
| Fee           | decimal(18,2)  | No       | Default `0.00`                                                         |
| Currency      | string(3)      | No       | ISO code, e.g., `USD`                                                  |
| ExternalRef   | string         | Yes      | External/broker reference                                              |
| Notes         | string         | Yes      | Free text                                                              |
| ExecutedAt    | DateTime (UTC) | No       | When transaction executed                                              |
| CreatedAt     | DateTime (UTC) | No       |                                                                        |
| UpdatedAt     | DateTime (UTC) | No       |                                                                        |

Indexes (from attributes):

- (`PortfolioId`, `ExecutedAt`)
- (`PortfolioId`, `Symbol`, `ExecutedAt`)

---

## Relationships (ER)

- Users (1) → (n) Portfolios  
- Users (1) → (n) Transactions  
- Portfolios (1) → (n) Holdings  
- Portfolios (1) → (n) Transactions  

```text
Users ──< Portfolios ──< Holdings
   └────< Transactions
```
   