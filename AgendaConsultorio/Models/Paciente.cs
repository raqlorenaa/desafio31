using System;
using System.ComponentModel.DataAnnotations;


namespace sistemaodonto
{
    
    public class Paciente
    {  
        [Key]
        public string CPF { get; private set; }
        public string Nome { get; private set; }
        public DateTime DataNascimento { get; private set; }

        // Construtor sem parâmetros para o Entity Framework
        public Paciente() {}

        // Construtor com parâmetros
        public Paciente(string cpf, string nome, DateTime dataNascimento)
        {
            CPF = cpf;
            Nome = nome;
            DataNascimento = dataNascimento;
        }

        public int CalcularIdade()
        {
            int idade = DateTime.Now.Year - DataNascimento.Year;
            if (DateTime.Now.DayOfYear < DataNascimento.DayOfYear)
                idade--;
            return idade;
        }
    }
}
