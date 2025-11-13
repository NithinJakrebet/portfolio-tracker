# Data Schema

> Source of truth for entity design, relationships, and constraints for the Portfolio Tracker API.
> Runtime schema is created via **EF Core Migrations**; this doc is for humans and reviews.

## Conventions

* **Time**: all timestamps are UTC (`CreatedAt`, `UpdatedAt` on every entity).
* **Money**: use `decimal` (never `double`). Suggested precision: `decimal(18,2)` for amounts, `decimal(18,6)` for per-share/quantity.
* **Soft delete** (optional): add `IsDeleted` + global filters if needed later.
* **Enums**: store as strings for readability (or ints + lookup table if preferred).
* **Cascades**: deleting a parent removes children (User → Portfolios; Portfolio → Holdings/Transactions).

---

## Users

**Purpose:** account & ownership of portfolios.

| Column       | Type      | Nullable | Constraints / Notes              |
| ------------ | --------- | -------- | -------------------------------- |
| UserId       | Guid (PK) | No       | Primary key                      |
| Email        | string    | No       | Unique (case-insensitive); index |
| PasswordHash | string    | No       | Or use external IdP sub later    |
| FullName     | string    | No       |                                  |
| IsActive     | bool      | No       | Default `true`                   |
| CreatedAt    | DateTime  | No       | UTC; default now                 |
| UpdatedAt    | DateTime  | No       | UTC; auto-updated on write       |

**Indexes**

* Unique: **Email**
* (Optional) Non-unique: **IsActive**

**Relationships**

* 1 User → *n* Portfolios (cascade on delete)

---

## Portfolios

**Purpose:** container for holdings, cash, and transactions.

| Column       | Type          | Nullable | Constraints / Notes                         |
| ------------ | ------------- | -------- | ------------------------------------------- |
| PortfolioId  | Guid (PK)     | No       | Primary key                                 |
| UserId       | Guid (FK)     | No       | FK → Users.UserId; index; cascade on delete |
| Label        | string        | No       | e.g., “Retirement 2045”                     |
| BaseCurrency | string (3)    | No       | ISO-4217 code, e.g., “USD”                  |
| CashBalance  | decimal(18,2) | No       | Running cash at portfolio level             |
| CreatedAt    | DateTime      | No       | UTC                                         |
| UpdatedAt    | DateTime      | No       | UTC                                         |

**Indexes / Uniqueness**

* Index: **UserId**
* (Optional) Unique composite: (**UserId**, **Label**) to avoid duplicate names per user

**Relationships**

* 1 Portfolio → *n* Holdings (cascade)
* 1 Portfolio → *n* Transactions (cascade)

---

## Holdings

**Purpose:** current position per instrument in a portfolio.

| Column       | Type          | Nullable | Constraints / Notes                                          |
| ------------ | ------------- | -------- | ------------------------------------------------------------ |
| HoldingId    | Guid (PK)     | No       | Primary key                                                  |
| PortfolioId  | Guid (FK)     | No       | FK → Portfolios.PortfolioId; index; cascade on delete        |
| Symbol       | string        | No       | e.g., “AAPL”; index                                          |
| Exchange     | string        | Yes      | e.g., “NASDAQ”                                               |
| Quantity     | decimal(18,6) | No       | Supports fractional shares                                   |
| AvgCostBasis | decimal(18,6) | No       | Average per-share cost (can be recomputed from transactions) |
| CreatedAt    | DateTime      | No       | UTC                                                          |
| UpdatedAt    | DateTime      | No       | UTC                                                          |

**Indexes / Uniqueness**

* Unique composite: (**PortfolioId**, **Symbol**) — one holding row per symbol per portfolio
* Index: **Symbol**

---

## Transactions

**Purpose:** immutable audit trail of all changes to cash/positions.

| Column        | Type          | Nullable | Constraints / Notes                                                                                   |
| ------------- | ------------- | -------- | ----------------------------------------------------------------------------------------------------- |
| TransactionId | Guid (PK)     | No       | Primary key                                                                                           |
| PortfolioId   | Guid (FK)     | No       | FK → Portfolios.PortfolioId; index; cascade on delete                                                 |
| UserId        | Guid (FK)     | No       | FK → Users.UserId (who initiated); index                                                              |
| Type          | string (enum) | No       | One of: `Buy`, `Sell`, `Deposit`, `Withdrawal`, `Dividend`, `Split`, `Fee`, `Interest`                |
| Symbol        | string        | Yes      | Required for position-affecting types (Buy/Sell/Dividend/Split/Fee); null for pure cash ops           |
| Quantity      | decimal(18,6) | Yes      | Required for `Buy`/`Sell`/`Split`; > 0                                                                |
| Price         | decimal(18,6) | Yes      | Execution price per unit for `Buy`/`Sell`; ≥ 0                                                        |
| GrossAmount   | decimal(18,2) | No       | Signed cash flow: +Deposit/Dividend/Interest, −Withdrawal/Fee; for Buy/Sell equals signed total value |
| Fee           | decimal(18,2) | Yes      | Default 0.00                                                                                          |
| Currency      | string (3)    | No       | Default = portfolio currency                                                                          |
| ExecutedAt    | DateTime      | No       | UTC; when it happened                                                                                 |
| ExternalRef   | string        | Yes      | Broker/order id                                                                                       |
| Notes         | string        | Yes      | Freeform                                                                                              |
| CreatedAt     | DateTime      | No       | UTC                                                                                                   |
| UpdatedAt     | DateTime      | No       | UTC                                                                                                   |

**Indexes**

* (**PortfolioId**, **ExecutedAt**) — common timeline queries
* (**PortfolioId**, **Symbol**, **ExecutedAt**) — position histories

**Validation / Business Rules (enforced in app layer or DB constraints)**

* If **Type** ∈ {`Buy`,`Sell`,`Split`} ⇒ **Symbol** NOT NULL, **Quantity** > 0
* If **Type** ∈ {`Deposit`,`Withdrawal`,`Fee`,`Dividend`,`Interest`} ⇒ **Quantity** NULL
* **GrossAmount** sign must match **Type**
* (Optional) **ExecutedAt** not in the future

---

## Relationships (ER)

* **Users (1) → (n) Portfolios**
* **Portfolios (1) → (n) Holdings**
* **Portfolios (1) → (n) Transactions**
* **Users (1) → (n) Transactions** (initiator)

```
Users ──< Portfolios ──< Holdings
   └────< Transactions
```

---

## Notes & Future Extensions

* Add **Prices** table if you want to persist vendor quotes (Symbol, Source, Price, At).
* Add **Orders/Executions** if you later simulate order lifecycles separate from Transactions.
* Add **Tags** (many-to-many) for Portfolios or Transactions for flexible reporting.

---

## EF Core Implementation Hints (not code)

* Each entity gets `CreatedAt/UpdatedAt`; update `UpdatedAt` in a SaveChanges interceptor.
* Configure unique composites via Fluent API:

  * Users.Email (unique)
  * Portfolios: (UserId, Label) (optional)
  * Holdings: (PortfolioId, Symbol)
* Store `Transaction.Type` as string enum for readability (HasConversion).
* Use cascade deletes for parent→child relationships.

---

**Runtime:** This document is **advisory**; migrations define the executable schema.
**Docker:** Not required to consume this file. If you want containerized seeding or SQL init, add explicit steps in your Dockerfile/entrypoint.
