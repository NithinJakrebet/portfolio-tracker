# HTTP API Reference

This document describes the HTTP endpoints exposed by the Portfolio Tracker API, including URLs, request/response payloads, and status codes.

Base URL examples:
- Local dev: `http://localhost:{port}`
- Docker: `http://localhost:8080`

Swagger/OpenAPI:
- `GET /swagger/index.html`

> Note: Controller routes use `[ApiController]` and `[Route("api/[controller]")]`, so the segment name matches the controller name (e.g., `UserController` → `/api/User`).

---

## Users

### GET /api/User

List all users.

# HTTP API Reference

This document describes the HTTP endpoints exposed by the Portfolio Tracker API, including URLs, request/response payloads, and status codes.

Base URL examples:
- Local dev: `http://localhost:{port}`
- Docker: `http://localhost:8080`

Swagger/OpenAPI:
- `GET /swagger/index.html`

> Note: Controller routes use `[ApiController]` and `[Route("api/[controller]")]`, so the segment name matches the controller name (e.g., `UserController` → `/api/User`).

---

## Users

### GET /api/User

List all users.

**Request**

```http
GET /api/User HTTP/1.1
Accept: application/json
```

**Response 200**`

**Response 200**

```json
[
  {
    "userId": "21b0f15b-f47a-4884-ab6b-5e524b341de8",
    "email": "jordan.garcia613@example.com",
    "portfolios": [],
    "transactions": [],
    "passwordHash": "hash-cc35f29a-372a-4b80-adc8-4407ac2c11f6",
    "fullName": "Jordan Garcia",
    "isActive": true,
    "createdAt": "2025-11-13T07:01:44.798444",
    "updatedAt": "2025-11-13T07:01:44.798444"
  }
]
```

---

### GET /api/User/{id}

Get a single user by `UserId`.

**Request**

```http
GET /api/User/21b0f15b-f47a-4884-ab6b-5e524b341de8 HTTP/1.1
Accept: application/json
```

**Response 200**

```json
{
  "userId": "21b0f15b-f47a-4884-ab6b-5e524b341de8",
  "email": "jordan.garcia613@example.com",
  "portfolios": [],
  "transactions": [],
  "passwordHash": "hash-cc35f29a-372a-4b80-adc8-4407ac2c11f6",
  "fullName": "Jordan Garcia",
  "isActive": true,
  "createdAt": "2025-11-13T07:01:44.798444",
  "updatedAt": "2025-11-13T07:01:44.798444"
}
```

**Response 404**

```json
{}
```

---

### POST /api/User

Create a user.

**Request**

```http
POST /api/User HTTP/1.1
Content-Type: application/json

{
  "email": "alice.smith@example.com",
  "password": "PlainTextOrHashedDependingOnDto",
  "fullName": "Alice Smith"
}
```

> In code you currently have `UserCreateDto`; adjust the payload here to match your DTO fields (`email`, `password`, `fullName`, etc.).

**Response 201**

```json
{
  "userId": "f4b96d3b-5d55-4dd3-a5c9-5c5b2a33d9fc",
  "email": "alice.smith@example.com",
  "portfolios": [],
  "transactions": [],
  "passwordHash": "hash-...derived...",
  "fullName": "Alice Smith",
  "isActive": true,
  "createdAt": "2025-11-13T07:20:00Z",
  "updatedAt": "2025-11-13T07:20:00Z"
}
```

**Errors**

* `400 Bad Request` – invalid email, missing fields, etc.
* `409 Conflict` – email already exists.

---

### PUT /api/User/{id}

Update user profile (e.g., `FullName`).

**Request**

```http
PUT /api/User/f4b96d3b-5d55-4dd3-a5c9-5c5b2a33d9fc HTTP/1.1
Content-Type: application/json

{
  "email": "alice.smith@example.com",
  "fullName": "Alice A. Smith"
}
```

**Response 204**

Empty body.

**Errors**

* `404 Not Found` – user not found.
* `409 Conflict` – new email not unique (if you allow email changes).

---

### DELETE /api/User/{id}

Delete or deactivate a user (depending on implementation).

**Request**

```http
DELETE /api/User/f4b96d3b-5d55-4dd3-a5c9-5c5b2a33d9fc HTTP/1.1
```

**Response 204**

Empty body.

---

## Portfolios

### GET /api/Portfolio

List portfolios.

**Request**

```http
GET /api/Portfolio HTTP/1.1
Accept: application/json
```

**Response 200**

```json
[
  {
    "portfolioId": "2d8a5c0b-3f95-4d8a-a4a1-18f723f2a5a2",
    "holdings": [],
    "transactions": [],
    "userId": "f4b96d3b-5d55-4dd3-a5c9-5c5b2a33d9fc",
    "user": null,
    "ownerName": "Alice Smith",
    "label": "Retirement 2045",
    "cashBalance": 14500.50,
    "createdAt": "2025-11-10T09:00:00Z",
    "updatedAt": "2025-11-13T07:25:00Z"
  }
]
```

---

### GET /api/Portfolio/{id}

Get single portfolio with holdings and transactions (depending on controller includes).

**Request**

```http
GET /api/Portfolio/2d8a5c0b-3f95-4d8a-a4a1-18f723f2a5a2 HTTP/1.1
```

**Response 200 (example)**

```json
{
  "portfolioId": "2d8a5c0b-3f95-4d8a-a4a1-18f723f2a5a2",
  "holdings": [
    {
      "holdingId": "6c19c0fb-0834-4cf7-ae57-dff4a9b3c5c9",
      "portfolioId": "2d8a5c0b-3f95-4d8a-a4a1-18f723f2a5a2",
      "portfolio": null,
      "symbol": "AAPL",
      "exchange": "NASDAQ",
      "quantity": 12.500000,
      "avgCostBasis": 175.350000,
      "createdAt": "2025-11-10T09:05:00Z",
      "updatedAt": "2025-11-13T07:25:00Z"
    }
  ],
  "transactions": [],
  "userId": "f4b96d3b-5d55-4dd3-a5c9-5c5b2a33d9fc",
  "user": null,
  "ownerName": "Alice Smith",
  "label": "Retirement 2045",
  "cashBalance": 14500.50,
  "createdAt": "2025-11-10T09:00:00Z",
  "updatedAt": "2025-11-13T07:25:00Z"
}
```

---

### POST /api/Portfolio

Create portfolio for a user.

**Request**

```http
POST /api/Portfolio HTTP/1.1
Content-Type: application/json

{
  "userId": "f4b96d3b-5d55-4dd3-a5c9-5c5b2a33d9fc",
  "ownerName": "Alice Smith",
  "label": "Trading Account",
  "cashBalance": 5000.00
}
```

**Response 201**

```json
{
  "portfolioId": "1a11b9f8-562e-4e94-9c3f-95ecbf7a5b4e",
  "holdings": [],
  "transactions": [],
  "userId": "f4b96d3b-5d55-4dd3-a5c9-5c5b2a33d9fc",
  "user": null,
  "ownerName": "Alice Smith",
  "label": "Trading Account",
  "cashBalance": 5000.00,
  "createdAt": "2025-11-13T07:30:00Z",
  "updatedAt": "2025-11-13T07:30:00Z"
}
```

---

### PUT /api/Portfolio/{id}

Update portfolio metadata or cash.

**Request**

```http
PUT /api/Portfolio/1a11b9f8-562e-4e94-9c3f-95ecbf7a5b4e HTTP/1.1
Content-Type: application/json

{
  "label": "Trading Account - High Risk",
  "cashBalance": 6200.00
}
```

**Response 204**

Empty body.

---

### DELETE /api/Portfolio/{id}

Delete portfolio.

**Request**

```http
DELETE /api/Portfolio/1a11b9f8-562e-4e94-9c3f-95ecbf7a5b4e HTTP/1.1
```

**Response 204**

Empty body.

---

## Holdings

### GET /api/Holding

List holdings (optionally filtered via query, if implemented).

**Request**

```http
GET /api/Holding HTTP/1.1
Accept: application/json
```

**Response 200**

```json
[
  {
    "holdingId": "6c19c0fb-0834-4cf7-ae57-dff4a9b3c5c9",
    "portfolioId": "2d8a5c0b-3f95-4d8a-a4a1-18f723f2a5a2",
    "portfolio": null,
    "symbol": "AAPL",
    "exchange": "NASDAQ",
    "quantity": 12.500000,
    "avgCostBasis": 175.350000,
    "createdAt": "2025-11-10T09:05:00Z",
    "updatedAt": "2025-11-13T07:25:00Z"
  }
]
```

---

### GET /api/Holding/{id}

```http
GET /api/Holding/6c19c0fb-0834-4cf7-ae57-dff4a9b3c5c9 HTTP/1.1
```

**Response 200**

```json
{
  "holdingId": "6c19c0fb-0834-4cf7-ae57-dff4a9b3c5c9",
  "portfolioId": "2d8a5c0b-3f95-4d8a-a4a1-18f723f2a5a2",
  "portfolio": null,
  "symbol": "AAPL",
  "exchange": "NASDAQ",
  "quantity": 12.500000,
  "avgCostBasis": 175.350000,
  "createdAt": "2025-11-10T09:05:00Z",
  "updatedAt": "2025-11-13T07:25:00Z"
}
```

---

### POST /api/Holding

Create a holding.

**Request**

```http
POST /api/Holding HTTP/1.1
Content-Type: application/json

{
  "portfolioId": "2d8a5c0b-3f95-4d8a-a4a1-18f723f2a5a2",
  "symbol": "MSFT",
  "exchange": "NASDAQ",
  "quantity": 8.000000,
  "avgCostBasis": 320.000000
}
```

**Response 201**

```json
{
  "holdingId": "c957a1b7-2f2c-4e13-8b7c-65330a6e13c6",
  "portfolioId": "2d8a5c0b-3f95-4d8a-a4a1-18f723f2a5a2",
  "portfolio": null,
  "symbol": "MSFT",
  "exchange": "NASDAQ",
  "quantity": 8.000000,
  "avgCostBasis": 320.000000,
  "createdAt": "2025-11-13T07:40:00Z",
  "updatedAt": "2025-11-13T07:40:00Z"
}
```

---

### PUT /api/Holding/{id}

Update a holding.

**Request**

```http
PUT /api/Holding/c957a1b7-2f2c-4e13-8b7c-65330a6e13c6 HTTP/1.1
Content-Type: application/json

{
  "quantity": 10.000000,
  "avgCostBasis": 310.000000
}
```

**Response 204**

Empty body.

---

### DELETE /api/Holding/{id}

```http
DELETE /api/Holding/c957a1b7-2f2c-4e13-8b7c-65330a6e13c6 HTTP/1.1
```

**Response 204**

Empty body.

---

## Transactions

### GET /api/Transaction

List transactions (base implementation).

**Request**

```http
GET /api/Transaction HTTP/1.1
Accept: application/json
```

**Response 200**

```json
[
  {
    "transactionId": "5e1457ac-0674-4c41-8bd8-af30e66d3f2a",
    "portfolioId": "2d8a5c0b-3f95-4d8a-a4a1-18f723f2a5a2",
    "portfolio": null,
    "userId": "f4b96d3b-5d55-4dd3-a5c9-5c5b2a33d9fc",
    "user": null,
    "type": "Buy",
    "symbol": "AAPL",
    "quantity": 5.000000,
    "price": 180.000000,
    "grossAmount": -900.00,
    "fee": -1.00,
    "currency": "USD",
    "externalRef": "ORD-12345",
    "notes": "Initial buy",
    "executedAt": "2025-11-10T09:10:00Z",
    "createdAt": "2025-11-10T09:11:00Z",
    "updatedAt": "2025-11-10T09:11:00Z"
  }
]
```

---

### GET /api/Transaction/{id}

```http
GET /api/Transaction/5e1457ac-0674-4c41-8bd8-af30e66d3f2a HTTP/1.1
```

**Response 200**

```json
{
  "transactionId": "5e1457ac-0674-4c41-8bd8-af30e66d3f2a",
  "portfolioId": "2d8a5c0b-3f95-4d8a-a4a1-18f723f2a5a2",
  "portfolio": null,
  "userId": "f4b96d3b-5d55-4dd3-a5c9-5c5b2a33d9fc",
  "user": null,
  "type": "Buy",
  "symbol": "AAPL",
  "quantity": 5.000000,
  "price": 180.000000,
  "grossAmount": -900.00,
  "fee": -1.00,
  "currency": "USD",
  "externalRef": "ORD-12345",
  "notes": "Initial buy",
  "executedAt": "2025-11-10T09:10:00Z",
  "createdAt": "2025-11-10T09:11:00Z",
  "updatedAt": "2025-11-10T09:11:00Z"
}
```

---

### POST /api/Transaction

Create a transaction. Business rules (via CHECK constraints) enforce that:

* Types `Buy`/`Sell`/`Split` require `Symbol`, `Quantity > 0`, and `Price >= 0`.
* Types `Deposit`/`Dividend`/`Interest` require `GrossAmount >= 0`.
* Types `Withdrawal`/`Fee` require `GrossAmount <= 0`.

**Request (Buy example)**

```http
POST /api/Transaction HTTP/1.1
Content-Type: application/json

{
  "portfolioId": "2d8a5c0b-3f95-4d8a-a4a1-18f723f2a5a2",
  "userId": "f4b96d3b-5d55-4dd3-a5c9-5c5b2a33d9fc",
  "type": "Buy",
  "symbol": "MSFT",
  "quantity": 3.000000,
  "price": 330.000000,
  "grossAmount": -990.00,
  "fee": -1.00,
  "currency": "USD",
  "externalRef": "ORD-67890",
  "notes": "Top up position",
  "executedAt": "2025-11-13T07:50:00Z"
}
```

**Response 201**

```json
{
  "transactionId": "b7f724b5-7ac7-4b9b-bdf8-1c2b8bdd6e31",
  "portfolioId": "2d8a5c0b-3f95-4d8a-a4a1-18f723f2a5a2",
  "portfolio": null,
  "userId": "f4b96d3b-5d55-4dd3-a5c9-5c5b2a33d9fc",
  "user": null,
  "type": "Buy",
  "symbol": "MSFT",
  "quantity": 3.000000,
  "price": 330.000000,
  "grossAmount": -990.00,
  "fee": -1.00,
  "currency": "USD",
  "externalRef": "ORD-67890",
  "notes": "Top up position",
  "executedAt": "2025-11-13T07:50:00Z",
  "createdAt": "2025-11-13T07:50:01Z",
  "updatedAt": "2025-11-13T07:50:01Z"
}
```

**Errors**

* `400 Bad Request` – violates CHECK constraints (e.g., missing symbol for Buy, wrong grossAmount sign).
* `404 Not Found` – portfolio or user not found.

---

### DELETE /api/Transaction/{id}

```http
DELETE /api/Transaction/b7f724b5-7ac7-4b9b-bdf8-1c2b8bdd6e31 HTTP/1.1
```



**Response 204**

Empty body.