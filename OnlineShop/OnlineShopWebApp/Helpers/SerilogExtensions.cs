using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;

namespace OnlineShopWebApp.Helpers
{
    public static class SerilogExtensions
    {
        public static LoggerConfiguration WriteToDatabase(this LoggerConfiguration loggerConfiguration, string databaseType, string connectionString)
        {
            switch (databaseType.ToLower())
            {
                case "mssql":
                    var sinkOptions = new MSSqlServerSinkOptions
                    {
                        TableName = "Logs",
                        AutoCreateSqlTable = true
                    };

                    loggerConfiguration.WriteTo.MSSqlServer(
                        connectionString: connectionString,
                        sinkOptions: sinkOptions,
                        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information);
                    break;

                case "postgresql":
                    loggerConfiguration.WriteTo.PostgreSQL(
                        connectionString: connectionString,
                        tableName: "Logs",
                        needAutoCreateTable: true,
                        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information);
                    break;

                case "mysql":
                    loggerConfiguration.WriteTo.MySQL(
                        connectionString: connectionString,
                        tableName: "Logs",
                        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information);
                    break;

                default:
                    throw new InvalidOperationException("Unsupported database type for logging");
            }

            return loggerConfiguration;
        }
    }
}
