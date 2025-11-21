#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

DB_PATH="/tmp/chirp.db"
rm -f "$DB_PATH"

echo "Recreating SQLite database at ${DB_PATH} via EF Core migrations and app seeding..."
ConnectionStrings__DefaultConnection="Data Source=${DB_PATH}" dotnet run --project src/Chirp.Web -- --seed-only >/tmp/chirp-seed.log
echo "Done. Log available at /tmp/chirp-seed.log"
