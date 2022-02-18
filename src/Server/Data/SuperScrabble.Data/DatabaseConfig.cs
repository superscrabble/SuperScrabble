using static SuperScrabble.Common.GlobalConstants;

namespace SuperScrabble.Data;

public static class DatabaseConfig
{
    public static readonly string ConnectionString =
        $"Server={ServerName};Database={SystemName};Integrated Security=True;";

    public const string ServerName = ".\\SQLEXPRESS";
}
