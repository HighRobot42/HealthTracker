DO $$
BEGIN
    -- Create role account_ro if it does not exist
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'account_ro') THEN
        CREATE ROLE account_ro;
    END IF;

    -- Create role account_rw if it does not exist
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'account_rw') THEN
        CREATE ROLE account_rw;
    END IF;

    -- Create role healthtracker_db_maintainers if it does not exist
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'healthtracker_db_maintainers') THEN
        CREATE ROLE because_db_maintainers;
    END IF;
END
$$;

GRANT SELECT ON ALL TABLES IN SCHEMA public TO account_ro;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO account_rw;
GRANT ALL PRIVILEGES ON DATABASE "HealthTracker" TO healthtracker_db_maintainers;