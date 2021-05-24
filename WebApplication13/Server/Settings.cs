using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication13.Server
{
    public static class Settings
    {
        public static bool UseHttps => true;

        public static bool IsDeployToHeroku => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DYNO"));

        public static bool IsDeployToIIS => false;

        public static IdentityServerDBEnum IdentityServerDB => IdentityServerDBEnum.Sqlite;

        public static bool UseCors => false;
        public static string CorsPolicy => "CorsPolicy";
        
    }

    public enum IdentityServerDBEnum
    {
        SqlServer,
        Sqlite


    }
}


