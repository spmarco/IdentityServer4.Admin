using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories
{
    class ConnectionDatabase
    {
        public static IDbConnection Get(IConfiguration configuration)
        {
            var con = new SqlConnection(configuration.GetConnectionString("AdminConnection"));
            con.Open();

            return con;
        }
    }
}
