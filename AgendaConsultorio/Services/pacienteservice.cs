using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SistemaOdonto.Persistence;

namespace sistemaodonto
{
    public class PacienteService
    {
        private readonly SistemaOdontoDbContext _context;

        // Construtor recebe o DbContext
        public PacienteService(SistemaOdontoDbContext context)
        {
            _context = context;
        }

        // Método GetPacientes
        public List<Paciente> GetPacientes()
        {
            return _context.Pacientes.ToList(); // Busca todos os pacientes do banco
        }

        public void CadastrarPaciente()
        {
            string cpf = ObterCPF();
            string nome = ObterNome();
            DateTime dataNascimento = ObterDataNascimento();

            // Garantindo que a data de nascimento seja UTC
            dataNascimento = DateTime.SpecifyKind(dataNascimento, DateTimeKind.Utc);

            Paciente paciente = new Paciente(cpf, nome, dataNascimento);
            _context.Pacientes.Add(paciente); // Adiciona ao banco de dados
            _context.SaveChanges(); // Salva as mudanças no banco
            Console.WriteLine("Paciente cadastrado com sucesso!");
        }

        private string ObterCPF()
        {
            while (true)
            {
                Console.Write("CPF: ");
                string cpf = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(cpf) && CPFValidator.ValidarCPF(cpf))
                {
                    // Verifica se o CPF já está cadastrado no banco
                    if (_context.Pacientes.Any(p => p.CPF == cpf))
                    {
                        Console.WriteLine("Erro: CPF já cadastrado.");
                        continue;
                    }
                    return cpf;
                }

                Console.WriteLine("Erro: CPF inválido ou vazio. Tente novamente.");
            }
        }

        private string ObterNome()
        {
            while (true)
            {
                Console.Write("Nome: ");
                string nome = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(nome) && nome.Length >= 5)
                {
                    return nome;
                }

                Console.WriteLine("Erro: O nome deve ter pelo menos 5 caracteres. Tente novamente.");
            }
        }

        private DateTime ObterDataNascimento()
        {
            while (true)
            {
                Console.Write("Data de nascimento (DDMMAAAA): ");
                string dataInput = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(dataInput) &&
                    DateTime.TryParseExact(dataInput, "ddMMyyyy", null, DateTimeStyles.None, out DateTime dataNascimento))
                {
                    // Garantindo que a data seja tratada como UTC
                    return DateTime.SpecifyKind(dataNascimento, DateTimeKind.Utc);
                }

                Console.WriteLine("Erro: Data de nascimento inválida. Tente novamente.");
            }
        }

        public void ExcluirPaciente(string cpf)
{
    var paciente = _context.Pacientes.FirstOrDefault(p => p.CPF == cpf);

    if (paciente == null)
    {
        Console.WriteLine("Erro: paciente não encontrado.");
        return;
    }

    // Ajustar a data para UTC se necessário
    var hojeUtc = DateTime.UtcNow.Date;

    // Verificar se há consultas futuras para o paciente
    bool possuiConsultasFuturas = _context.Consultas
        .Any(c => c.CPFDoPaciente == cpf && c.Data.Date >= hojeUtc);

    if (possuiConsultasFuturas)
    {
        Console.WriteLine("Erro: paciente possui consultas futuras agendadas.");
        return;
    }

    // Remove o paciente do banco
    _context.Pacientes.Remove(paciente);
    _context.SaveChanges();

    Console.WriteLine("Paciente excluído com sucesso!");
}


        public void ListarPacientes(string ordem)
        {
            var lista = ordem == "CPF" 
                ? _context.Pacientes.OrderBy(p => p.CPF) 
                : _context.Pacientes.OrderBy(p => p.Nome);

            Console.WriteLine("CPF      Nome                     Dt.Nasc  Idade");

            foreach (var p in lista)
            {
                Console.WriteLine($"{p.CPF} {p.Nome,-25} {p.DataNascimento:dd/MM/yyyy} {p.CalcularIdade()}");
            }
        }

        public void ListarPacientesPorNome()
        {
            ListarPacientes("Nome");
        }

        public void ListarPacientesPorCPF()
        {
            ListarPacientes("CPF");
        }
    }
}
