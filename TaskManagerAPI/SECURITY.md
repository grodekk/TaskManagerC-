# Security Notes

## Known transitive dependency advisories

`dotnet list package --vulnerable --include-transitive` currently reports
one high-severity advisory:

- SQLitePCLRaw.lib.e_sqlite3 2.1.11
  - GHSA-2m69-gcr7-jv3q / CVE-2025-6965
  - Transitive dependency of the EF Core / SQLite stack.
  - No patched version is currently listed in the 2.1.x line; EF Core's
    Microsoft.EntityFrameworkCore.Sqlite still pins >= 2.1.11.
  - A manual major-version override is intentionally avoided until
    compatibility with the current EF Core stack is verified.
  - Tracked upstream: dotnet/efcore#38257

## Resolved

- Microsoft.OpenApi was upgraded from 2.0.0 to 2.7.5 (GHSA-v5pm-xwqc-g5wc),
  resolving the advisory without breaking the ASP.NET Core OpenAPI
  integration.

## Current mitigation

Direct .NET and EF Core packages are kept on the latest compatible
10.0.x patch versions. The remaining advisory is tracked as an
upstream dependency issue and will be revisited once EF Core ships
a compatible SQLite package update.