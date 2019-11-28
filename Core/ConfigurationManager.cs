using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Imms
{
    public class ConfigurationManager
    {
        public readonly static IConfiguration Configuration;
        public readonly static ConnectionString ConnectionString;

        static ConfigurationManager()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            ConnectionString = new ConnectionString();
            ConnectionString.ConnectionUrl = Configuration["ConnectionStrings:IMMS-CONN:ConnectionUrl"];
            ConnectionString.ProviderType = (ProviderType) Enum.Parse(typeof(ProviderType), Configuration["ConnectionStrings:IMMS-CONN:ProviderType"]);

            ConfigurationManager.LogLevel = (LogLevel) Enum.Parse(typeof(LogLevel),Configuration["Logging:LogLevel:Imms.Logger"]);
        }

        public readonly static LogLevel LogLevel;                
    }

    public class ConnectionString
    {
        public string ConnectionUrl { get; set; }
        public ProviderType ProviderType { get; set; }
    }

    public enum ProviderType
    {
        SqlServer,
        MySql,
        Oracle
    }
}
