using HealthMed.API.AgendamentoConsulta.Database;
using HealthMed.API.AgendamentoConsulta.Models;
using Microsoft.Data.SqlClient;
using System.Text;

namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public class DisponibilidadeMedicoRepository(IConfiguration configuration)
    {
        DBConnection? sqldb;
        String? dbname = String.Empty;
        public readonly IConfiguration _config = configuration;

        public Guid Post(DisponibilidadeMedico disponibilidadeMedico)
        {
            sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            if (sqldb == null || sqldb.connection == null)
                throw new Exception("SQL ERROR");

            using (sqldb.connection)
            {
                Guid idDisponibilidadeMedico = Guid.NewGuid();
                var query = new StringBuilder();
                dbname = this._config.GetValue<string>("DatabaseName");
                query.Append($@"INSERT INTO {dbname}.dbo.DisponibilidadeMedico 
                            ([Id],
                            [DiaSemana],
                            [InicioPeriodo],
                            [FimPeriodo],
                            [Validade],
                            [IdMedico]) 
                            VALUES (
                            '{idDisponibilidadeMedico}',
                            {disponibilidadeMedico.DiaSemana},
                            '{disponibilidadeMedico.InicioPeriodo}', 
                            '{disponibilidadeMedico.FimPeriodo}', 
                            '{disponibilidadeMedico.Validade.Date.ToString()}', 
                            '{disponibilidadeMedico.Medico}'
                            )");

                sqldb.connection.Open();
                SqlCommand command = new(query.ToString(), sqldb.connection);
                command.ExecuteNonQuery();
                sqldb.connection.Close();
                return idDisponibilidadeMedico;
            }
        }

        public List<DisponibilidadeMedico> Get(DateTime data)
        {
            List<DisponibilidadeMedico> disponibilidades = [];

            //sqldb = new DBConnection(this._config.GetConnectionString("ConnectionString"));

            //if (sqldb == null || sqldb.connection == null)
            //    throw new Exception("SQL ERROR");

            //using (sqldb.connection)
            //{
            //    Guid idDisponibilidadeMedico = Guid.NewGuid();
            //    var query = new StringBuilder();
            //    dbname = this._config.GetValue<string>("DatabaseName");
            //    query.Append($@"INSERT INTO {dbname}.dbo.DisponibilidadeMedico 
            //                ([Id],
            //                [DiaSemana],
            //                [InicioPeriodo],
            //                [FimPeriodo],
            //                [Validade],
            //                [IdMedico]) 
            //                VALUES (
            //                '{idDisponibilidadeMedico}',
            //                {disponibilidadeMedico.diaSemana},
            //                '{disponibilidadeMedico.inicioPeriodo}', 
            //                '{disponibilidadeMedico.fimPeriodo}', 
            //                '{disponibilidadeMedico.validade.Date.ToString()}', 
            //                '{disponibilidadeMedico.medico}'
            //                )");

            //    sqldb.connection.Open();
            //    SqlCommand command = new SqlCommand(query.ToString(), sqldb.connection);
            //    command.ExecuteNonQuery();
            //    sqldb.connection.Close();
            //    return idDisponibilidadeMedico;
            //}
            return disponibilidades;
        }
    }
}
