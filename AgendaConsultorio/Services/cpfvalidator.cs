using System;

public class CPFValidator
{
    public static bool ValidarCPF(string cpf)
    {
        // Remove qualquer máscara ou espaços no CPF
        cpf = cpf.Replace(".", "").Replace("-", "").Trim();

        // Verifica se o CPF tem exatamente 11 dígitos
        if (cpf.Length != 11 || !long.TryParse(cpf, out _))
            return false;

        // Verifica se todos os dígitos são iguais
        if (new string(cpf[0], cpf.Length) == cpf)
            return false;

        // Calcula o primeiro dígito verificador (J)
        int somaJ = 0;
        for (int i = 0; i < 9; i++)
        {
            somaJ += (cpf[i] - '0') * (10 - i);
        }
        
        int restoJ = somaJ % 11;
        int primeiroDigitoVerificador = restoJ < 2 ? 0 : 11 - restoJ;

        // Verifica o primeiro dígito verificador
        if (primeiroDigitoVerificador != cpf[9] - '0')
            return false;

        // Calcula o segundo dígito verificador (K)
        int somaK = 0;
        for (int i = 0; i < 10; i++)
        {
            somaK += (cpf[i] - '0') * (11 - i);
        }

        int restoK = somaK % 11;
        int segundoDigitoVerificador = restoK < 2 ? 0 : 11 - restoK;

        // Verifica o segundo dígito verificador
        if (segundoDigitoVerificador != cpf[10] - '0')
            return false;

        // CPF é válido
        return true;
    }
}
