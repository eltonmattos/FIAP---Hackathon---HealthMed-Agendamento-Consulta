using Microsoft.Data.SqlClient;

namespace HealthMed.API.AgendamentoConsulta.Services
{
    public class DBConnection(String? connectionString)
    {
        public SqlConnection? Connection { get; private set; } = new SqlConnection(connectionString);
    }
}