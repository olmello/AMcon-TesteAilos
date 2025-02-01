namespace Questao5.Infrastructure.Database.QueryStore.Responses
{
    public class ExtratoBancarioQuery
    {
        public int NumeroConta { get; set; }
        public string NomeTitular { get; set; }
        public string DataExtrato { get; set; }
        public double SaldoAtual { get; set; }
    }
}