using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaOdonto.Persistence;
using sistemaodonto;
using System;
using System.Collections.Generic;

namespace sistemaodonto
{
    class ConsultorioApp
    {
        static List<Consulta> consultas = new List<Consulta>();
        static PacienteService pacienteService;
        static AgendaService agendaService;

        static void Main(string[] args)
        {
            // Carregar as configurações do arquivo appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Configuração da injeção de dependência para PostgreSQL
            var serviceProvider = new ServiceCollection()
                .AddDbContext<SistemaOdontoDbContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))) // Usando Npgsql para PostgreSQL
                .AddTransient<PacienteService>()
                .AddTransient<AgendaService>()
                .BuildServiceProvider();

            // Injeção de dependência para as services
            pacienteService = serviceProvider.GetService<PacienteService>();
            agendaService = serviceProvider.GetService<AgendaService>();

            while (true)
            {
                Console.WriteLine("\nMenu Principal");
                Console.WriteLine("1 - Cadastro de pacientes");
                Console.WriteLine("2 - Agenda");
                Console.WriteLine("3 - Fim");
                Console.Write("Escolha uma opção: ");
                if (!int.TryParse(Console.ReadLine(), out int opcao))
                {
                    Console.WriteLine("Opção inválida!");
                    continue;
                }

                switch (opcao)
                {
                    case 1:
                        MenuCadastroPacientes();
                        break;
                    case 2:
                        MenuAgenda();
                        break;
                    case 3:
                        Console.WriteLine("Encerrando o sistema...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }
            }
        }

        static void MenuCadastroPacientes()
        {
            while (true)
            {
                Console.WriteLine("\nMenu Cadastro de Pacientes");
                Console.WriteLine("1 - Cadastrar novo paciente");
                Console.WriteLine("2 - Excluir paciente");
                Console.WriteLine("3 - Listar pacientes (ordenado por CPF)");
                Console.WriteLine("4 - Listar pacientes (ordenado por nome)");
                Console.WriteLine("5 - Voltar ao menu principal");
                Console.Write("Escolha uma opção: ");

                if (!int.TryParse(Console.ReadLine(), out int opcao))
                {
                    Console.WriteLine("Opção inválida!");
                    continue;
                }

                switch (opcao)
                {
                    case 1:
                        pacienteService.CadastrarPaciente();
                        break;
                    case 2:
                        Console.Write("CPF do paciente: ");
                        string cpf = Console.ReadLine();
                        pacienteService.ExcluirPaciente(cpf);
                        break;
                    case 3:
                        pacienteService.ListarPacientes("CPF");
                        break;
                    case 4:
                        pacienteService.ListarPacientes("Nome");
                        break;
                    case 5:
                        return;
                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }
            }
        }

        static void MenuAgenda()
        {
            while (true)
            {
                Console.WriteLine("\nMenu Agenda");
                Console.WriteLine("1 - Agendar consulta");
                Console.WriteLine("2 - Cancelar agendamento");
                Console.WriteLine("3 - Listar agenda");
                Console.WriteLine("4 - Voltar ao menu principal");
                Console.Write("Escolha uma opção: ");

                if (!int.TryParse(Console.ReadLine(), out int opcao))
                {
                    Console.WriteLine("Opção inválida!");
                    continue;
                }

                switch (opcao)
                {
                    case 1:
                        agendaService.AgendarConsulta();
                        break;
                    case 2:
                        agendaService.CancelarAgendamento();
                        break;
                    case 3:
                        agendaService.ListarAgenda();
                        break;
                    case 4:
                        return;
                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }
            }
        }
    }
}
