PRAGMA foreign_keys = ON;

-- =========================
-- vehicles
-- =========================
CREATE TABLE IF NOT EXISTS vehicles (
    id TEXT NOT NULL PRIMARY KEY,  -- GUID (e.g. '6f1c1b8e-...')
    license_plate TEXT NOT NULL
        CHECK (length(license_plate) <= 20),
    make TEXT NOT NULL
        CHECK (length(make) <= 100),
    model TEXT NOT NULL
        CHECK (length(model) <= 100),
    manufacturing_date TEXT NOT NULL -- ISO-8601 datetime string recommended
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_vehicles_license_plate
ON vehicles (license_plate);

-- =========================
-- customers
-- =========================
CREATE TABLE IF NOT EXISTS customers (
    id TEXT NOT NULL PRIMARY KEY,  -- GUID
    id_document TEXT NOT NULL
        CHECK (length(id_document) <= 15),
    name TEXT NOT NULL
        CHECK (length(name) <= 50),
    surname TEXT NOT NULL
        CHECK (length(surname) <= 100)
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_customers_id_document
ON customers (id_document);

-- =========================
-- renting_history
-- =========================
CREATE TABLE IF NOT EXISTS renting_history (
    id TEXT NOT NULL PRIMARY KEY,  -- GUID
    id_vehicle TEXT NOT NULL,
    id_customer TEXT NOT NULL,
    start_date TEXT NOT NULL,
    end_date TEXT NULL,

    -- End date can't be before start date
    CHECK (end_date IS NULL OR end_date >= start_date),

    FOREIGN KEY (id_vehicle) REFERENCES vehicles(id) ON DELETE RESTRICT,
    FOREIGN KEY (id_customer) REFERENCES customers(id) ON DELETE RESTRICT
);

-- Helpful indexes for lookups
CREATE INDEX IF NOT EXISTS ix_renting_history_vehicle
ON renting_history (id_vehicle);

CREATE INDEX IF NOT EXISTS ix_renting_history_customer
ON renting_history (id_customer);

-- Enforce: one active rent per customer (end_date IS NULL)
CREATE UNIQUE INDEX IF NOT EXISTS ux_renting_history_customer_open
ON renting_history (id_customer)
WHERE end_date IS NULL;

-- Enforce: one active rent per vehicle (end_date IS NULL)
CREATE UNIQUE INDEX IF NOT EXISTS ux_renting_history_vehicle_open
ON renting_history (id_vehicle)
WHERE end_date IS NULL;