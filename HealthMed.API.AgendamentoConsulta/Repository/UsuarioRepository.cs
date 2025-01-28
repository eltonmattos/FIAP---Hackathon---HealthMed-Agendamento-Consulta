using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Azure.Identity;
using HealthMed.API.AgendamentoConsulta.Services;
using HealthMed.API.AgendamentoConsulta.Models;
using Microsoft.Data.SqlClient;


namespace HealthMed.API.AgendamentoConsulta.Repository
{
    public abstract class UsuarioRepository
    {
        public static void ValidatePassword(String password)
        {
            if (string.IsNullOrWhiteSpace(password) ||
            password.Length < 6 ||
            !password.Any(char.IsUpper) ||
            !password.Any(char.IsLower) ||
            !password.Any(char.IsDigit))
            {
                throw new FormatException(@"Comprimento mínimo: A senha deve ser composta de:
                                            - Pelo menos 8 caracteres.
                                            - Pelo menos uma letra maiúscula.
                                            - Pelo menos uma letra minúscula.
                                            - Pelo menos um número.");
            }
        }

        public static void ValidateEmail(String email)
        {
            if (!String.IsNullOrEmpty(email))
            {
                try
                {
                    MailAddress m = new MailAddress(email);
                }
                catch (FormatException)
                {
                    throw new FormatException("Email inválido");
                }
            }
        }

        public static void ValidateCPF(String cpf)
        {
            // Remove caracteres não numéricos
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11 || !long.TryParse(cpf, out _))
                throw new FormatException("CPF inválido");

            // Verifica se todos os dígitos são iguais
            bool allSameDigits = true;
            for (int i = 1; i < 11 && allSameDigits; i++)
            {
                if (cpf[i] != cpf[0])
                    allSameDigits = false;
            }

            if (allSameDigits)
                throw new FormatException("CPF inválido");

            // Calcula o primeiro dígito verificador
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (cpf[i] - '0') * (10 - i);
            }

            int remainder = sum % 11;
            int firstVerifierDigit = (remainder < 2) ? 0 : 11 - remainder;

            if (cpf[9] - '0' != firstVerifierDigit)
                throw new FormatException("CPF inválido");

            // Calcula o segundo dígito verificador
            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += (cpf[i] - '0') * (11 - i);
            }

            remainder = sum % 11;
            int secondVerifierDigit = (remainder < 2) ? 0 : 11 - remainder;

            if (cpf[10] - '0' != secondVerifierDigit)
                throw new FormatException("CPF inválido");
        }
    }
}
