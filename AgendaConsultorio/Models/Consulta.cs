using System;
using System.ComponentModel.DataAnnotations;

namespace sistemaodonto
{
    public class Consulta
    {
        [Key]  
        public int Id { get; set; }  
        public string CPFDoPaciente { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }

        
        public Consulta() { }

        // Construtor com parâmetros
        public Consulta(string cpfPaciente, DateTime data, TimeSpan horaInicio, TimeSpan horaFim)
        {
            CPFDoPaciente = cpfPaciente;
            Data = data;
            HoraInicio = horaInicio;
            HoraFim = horaFim;
        }
    }
}
