# Security Notes

## Known transitive dependency advisories

`dotnet list package --vulnerable --include-transitive`
currently reports two high-severity advisories:

- `Microsoft.OpenApi 2.0.0`
  - GHSA-v5pm-xwqc-g5wc
  - Transitive dependency of the ASP.NET Core OpenAPI stack.
  - The affected Microsoft.OpenApi versions include 2.0.0.
  - A manual upgrade to an incompatible major version is intentionally
    avoided to prevent breaking the current ASP.NET Core OpenAPI integration.

- `SQLitePCLRaw.lib.e_sqlite3 2.1.11`
  - GHSA-2m69-gcr7-jv3q
  - Transitive dependency of the EF Core / SQLite stack.
  - Version 2.1.11 is affected and no patched version is listed for this
    package line in the advisory.
  - A manual major-version override is intentionally avoided until
    compatibility with the current EF Core stack is verified.

## Current mitigation

Direct .NET and EF Core packages are kept on the latest compatible
10.0.x patch versions used by this project.

The remaining advisories are tracked as upstream dependency issues.
They should be revisited when compatible upstream packages become available.