using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaOdonto.Persistence;

namespace sistemaodonto
{
    public class AgendaService
    {
        private readonly SistemaOdontoDbContext _dbContext;
        private readonly PacienteService _pacienteService;

        public AgendaService(SistemaOdontoDbContext dbContext, PacienteService pacienteService)
        {
            _dbContext = dbContext;
            _pacienteService = pacienteService;
        }

        public void AgendarConsulta()
        {
            Console.Write("CPF do paciente: ");
            string cpf = Console.ReadLine()?.Trim();

            // Verificar se o paciente existe no banco de dados
            var paciente = _pacienteService.GetPacientes().FirstOrDefault(p => p.CPF == cpf);
            if (paciente == null)
            {
                Console.WriteLine("Erro: paciente não encontrado.");
                return;
            }

            // Obter a data da consulta
            Console.Write("Data da consulta (DDMMYYYY): ");
            string dataInput = Console.ReadLine()?.Trim();
            if (dataInput == null || dataInput.Length != 8 || 
                !DateTime.TryParseExact(dataInput, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out DateTime data))
            {
                Console.WriteLine("Erro: data inválida.");
                return;
            }

            // Obter a hora de início
            Console.Write("Hora de início (HHMM): ");
            string horaInicioInput = Console.ReadLine()?.Trim();
            if (horaInicioInput == null || horaInicioInput.Length != 4 ||
                !TimeSpan.TryParseExact(horaInicioInput, "hhmm", null, out TimeSpan horaInicio))
            {
                Console.WriteLine("Erro: hora inválida.");
                return;
            }

            // Obter a hora de término
            Console.Write("Hora de término (HHMM): ");
            string horaFimInput = Console.ReadLine()?.Trim();
            if (horaFimInput == null || horaFimInput.Length != 4 ||
                !TimeSpan.TryParseExact(horaFimInput, "hhmm", null, out TimeSpan horaFim))
            {
                Console.WriteLine("Erro: hora inválida.");
                return;
            }

            // Validar os horários
            if (horaFim <= horaInicio)
            {
                Console.WriteLine("Erro: horário de término deve ser posterior ao horário de início.");
                return;
            }

            if (horaInicio < TimeSpan.FromHours(8) || horaFim > TimeSpan.FromHours(19))
            {
                Console.WriteLine("Erro: horários devem estar entre 08:00 e 19:00.");
                return;
            }

            if ((horaInicio.Minutes % 15 != 0) || (horaFim.Minutes % 15 != 0))
            {
                Console.WriteLine("Erro: horários devem ser múltiplos de 15 minutos.");
                return;
            }

            // Combinar data e horários e ajustar para UTC
            DateTime inicio = DateTime.SpecifyKind(data.Add(horaInicio), DateTimeKind.Utc);
            DateTime fim = DateTime.SpecifyKind(data.Add(horaFim), DateTimeKind.Utc);

            // Verificar se há conflito no banco de dados
            var consultaExistente = _dbContext.Consultas.AsEnumerable() // Avaliar localmente devido a limitações do LINQ
                .FirstOrDefault(c =>
                {
                    var consultaInicio = DateTime.SpecifyKind(c.Data, DateTimeKind.Utc);
                    var consultaFim = consultaInicio.AddMinutes((c.HoraFim - c.HoraInicio).TotalMinutes);
                    return consultaInicio.Date == inicio.Date &&
                        (inicio < consultaFim && fim > consultaInicio);
                });

            if (consultaExistente != null)
            {
                Console.WriteLine("Erro: horário já agendado.");
                return;
            }

            // Criar nova consulta
            var novaConsulta = new Consulta
            {
                CPFDoPaciente = cpf,
                Data = inicio,
                HoraInicio = horaInicio,
                HoraFim = horaFim
            };

            _dbContext.Consultas.Add(novaConsulta);
            _dbContext.SaveChanges();

            Console.WriteLine("Consulta agendada com sucesso!");
        }


        public void CancelarAgendamento()
        {
            Console.Write("CPF do paciente: ");
            string cpf = Console.ReadLine()?.Trim();

            var consulta = _dbContext.Consultas
                .FirstOrDefault(c => c.CPFDoPaciente == cpf && c.Data >= DateTime.UtcNow);
            if (consulta == null)
            {
                Console.WriteLine("Erro: nenhuma consulta futura encontrada para esse CPF.");
                return;
            }

            _dbContext.Consultas.Remove(consulta);
            _dbContext.SaveChanges();

            Console.WriteLine("Consulta cancelada com sucesso!");
        }

        public void ListarAgenda()
        {
            var consultas = _dbContext.Consultas
                .OrderBy(c => c.Data)
                .ToList();

            if (!consultas.Any())
            {
                Console.WriteLine("Nenhuma consulta agendada.");
                return;
            }

            foreach (var consulta in consultas)
            {
                var dataLocal = consulta.Data.ToLocalTime(); // Converter para o horário local
                Console.WriteLine($"CPF: {consulta.CPFDoPaciente}, Data: {dataLocal:dd/MM/yyyy}, " +
                                  $"Hora: {consulta.HoraInicio:hh\\:mm} às {consulta.HoraFim:hh\\:mm}");
            }
        }
    }
}
