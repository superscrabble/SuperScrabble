namespace SuperScrabble.Data
{
    using static SuperScrabble.Common.GlobalConstants;

    public static class DatabaseConfig
    {
        public static readonly string ConnectionString = $"Server={ServerName};Database={SystemName}DB;Integrated Security=True;";

        private const string ServerName = ".\\SQLEXPRESS";
    }
}
