import os, sqlite3, random, datetime, requests
from faker import Faker

DB_PATH = os.environ.get("DB_PATH", "app.db")
N = int(os.environ.get("N", "300"))
ALPHA_KEY = os.environ.get("ALPHA_VANTAGE_KEY")  # optional API key
fake = Faker()

TOP_SYMBOLS = [
    "AAPL","MSFT","AMZN","NVDA","GOOGL","META","BRK.B","TSLA","LLY","JPM",
    "V","UNH","XOM","MA","PG","AVGO","JNJ","COST","HD","ADBE"
]

def fetch_meta(symbol):
    if not ALPHA_KEY:
        return f"{symbol} Portfolio"
    url = "https://www.alphavantage.co/query"
    params = {"function": "OVERVIEW", "symbol": symbol, "apikey": ALPHA_KEY}
    try:
        r = requests.get(url, timeout=10)
        if r.ok:
            data = r.json()
            name = data.get("Name") or symbol
            sector = data.get("Sector") or "General"
            return f"{name} ({sector})"
    except Exception:
        pass
    return f"{symbol} Portfolio"

def main():
    conn = sqlite3.connect(DB_PATH)
    cur = conn.cursor()
    cur.execute("""
    CREATE TABLE IF NOT EXISTS Portfolios (
        PortfolioId TEXT PRIMARY KEY,
        OwnerName   TEXT NOT NULL,
        Label       TEXT NOT NULL,
        CashBalance REAL NOT NULL,
        CreatedAt   TEXT NOT NULL,
        UpdatedAt   TEXT NOT NULL
    )
    """)

    rows = []
    for _ in range(N):
        pid = os.popen("uuidgen").read().strip() or fake.uuid4()
        owner = fake.name()
        cash = round(random.uniform(500.0, 50000.0), 2)
        label = fetch_meta(random.choice(TOP_SYMBOLS))

        # simulate realistic timestamps (created before updated)
        created = datetime.datetime.utcnow() - datetime.timedelta(days=random.randint(1, 60))
        updated = created + datetime.timedelta(days=random.randint(0, 30))
        created_str = created.replace(microsecond=0).isoformat() + "Z"
        updated_str = updated.replace(microsecond=0).isoformat() + "Z"

        rows.append((pid, owner, label, cash, created_str, updated_str))

    cur.executemany("""
        INSERT INTO Portfolios (PortfolioId, OwnerName, Label, CashBalance, CreatedAt, UpdatedAt)
        VALUES (?, ?, ?, ?, ?, ?)
    """, rows)
    conn.commit()
    conn.close()
    print(f"Inserted {len(rows)} rows into Portfolios in {DB_PATH} (alpha={'on' if ALPHA_KEY else 'off'})")

if __name__ == "__main__":
    main()
