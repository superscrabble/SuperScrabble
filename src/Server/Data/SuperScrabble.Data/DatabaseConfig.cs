using static SuperScrabble.Common.GlobalConstants;

namespace SuperScrabble.Data;

public static class DatabaseConfig
{
    // Fallback for EF Core design-time tooling only; the application reads
    // ConnectionStrings:DefaultConnection from configuration (see Program.cs).
    // TrustServerCertificate is required because Microsoft.Data.SqlClient encrypts
    // by default since EF Core 7 and local instances have no trusted certificate.
    public static string ConnectionString =>
        $"Server={ServerName};Database={SystemName};Integrated Security=True;TrustServerCertificate=True;";

    public const string ServerName = ".\\SQLEXPRESS";
}
