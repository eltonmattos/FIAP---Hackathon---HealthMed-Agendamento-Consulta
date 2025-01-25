using Microsoft.Data.SqlClient;

namespace HealthMed.API.AgendamentoConsulta.Database
{
    public class DBConnection
    {
        public SqlConnection? connection { get; private set; }

        public DBConnection(String? connectionString)
        {
            this.connection = new SqlConnection(connectionString);
        }
    }
}