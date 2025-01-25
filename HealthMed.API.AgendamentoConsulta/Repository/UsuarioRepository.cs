using System.Net.Mail;
using System.Text.RegularExpressions;
using Azure.Identity;
using HealthMed.API.AgendamentoConsulta.Models;


namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public class UsuarioRepository
    {
        public static void Validate(Usuario usuario)
        {
            if (!String.IsNullOrEmpty(usuario.Email))
            {
                try
                {
                    MailAddress m = new MailAddress(usuario.Email);
                }
                catch (FormatException)
                {
                    throw new FormatException("Email inválido");
                }
            }
        }

        public static bool ValidateCPF(string cpf)
        {
            // Remove caracteres não numéricos
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11 || !long.TryParse(cpf, out _))
                return false;

            // Verifica se todos os dígitos são iguais
            bool allSameDigits = true;
            for (int i = 1; i < 11 && allSameDigits; i++)
            {
                if (cpf[i] != cpf[0])
                    allSameDigits = false;
            }

            if (allSameDigits)
                return false;

            // Calcula o primeiro dígito verificador
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (cpf[i] - '0') * (10 - i);
            }

            int remainder = sum % 11;
            int firstVerifierDigit = (remainder < 2) ? 0 : 11 - remainder;

            if (cpf[9] - '0' != firstVerifierDigit)
                return false;

            // Calcula o segundo dígito verificador
            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += (cpf[i] - '0') * (11 - i);
            }

            remainder = sum % 11;
            int secondVerifierDigit = (remainder < 2) ? 0 : 11 - remainder;

            return cpf[10] - '0' == secondVerifierDigit;
        }
    }
}
