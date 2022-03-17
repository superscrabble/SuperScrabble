using static SuperScrabble.Common.GlobalConstants;

namespace SuperScrabble.Data;

public static class DatabaseConfig
{
    public static string ConnectionString => !IsProduction
        ? $"Server={ServerName};Database={SystemName};Integrated Security=True;"
        : $"Server=my-server\\SQLEXPRESS;Database={SystemName};Integrated Security=True;";

    public static bool IsProduction = true;

    public const string ServerName = ".\\SQLEXPRESS";
}
