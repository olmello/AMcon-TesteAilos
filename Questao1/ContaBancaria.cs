using System;

namespace Questao1
{
    class ContaBancaria {

        public readonly double TarifaSaque = 3.5;

        public int Numero { get; private set; }
        public string Titular { get; private set; }
        public double Saldo { get; private set; }

        /*
         * Validações de CRUD, prefiro utilizar fast fail validation numa abordagem command/query (CQRS),
         * assim evito lançar exceções de modo desnecessário, já que, numero da conta/deposito inicial negativos ou nome maior/menor que (x) ou nulo,
         * são erros esperados, e não uma exceção e fato
         */

        public ContaBancaria(int numero, string titular)
        {
            if (numero <= 0) throw new ArgumentException("Número da conta inválido.");
            if (string.IsNullOrEmpty(titular)) throw new ArgumentException("Nome do titulat inválido.");

            Numero = numero;
            Titular = titular;
        }

        public ContaBancaria(int numero, string titular, double depositoInicial) : this(numero, titular)
        {
            Deposito(depositoInicial);
        }

        public void Deposito(double quantia)
        {
            if (quantia < 0) quantia *= -1;

            Saldo += quantia;
        }
        public void Saque(double quantia)
        {
            if (quantia < 0) quantia *= -1;

            Saldo -= quantia + TarifaSaque;
        }

        public void AlterarNomeTitular(string novoNomeTitular)
        {
            if (string.IsNullOrEmpty(novoNomeTitular)) return;

            Titular = novoNomeTitular;
        }

        public override string ToString()
        {
            return $"Conta {Numero}, Titular: {Titular}, Saldo: $ {Saldo:F2}";
        }
    }
}