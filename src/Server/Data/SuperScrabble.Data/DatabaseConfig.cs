namespace SuperScrabble.Data
{
    using static SuperScrabble.Common.GlobalConstants;

    public static class DatabaseConfig
    {
        public static readonly string ConnectionString =
            $"Server={ServerName};Database={SystemName};Integrated Security=True;";

        public const string ServerName = ".\\SQLEXPRESS";
    }
}
