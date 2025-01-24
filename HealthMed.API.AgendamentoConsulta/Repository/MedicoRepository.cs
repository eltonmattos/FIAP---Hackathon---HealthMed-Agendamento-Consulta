namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public class Medico : Usuario
    {
        public required string CPF { get; set; }
        public required string CRM { get; set; }
    }

}
