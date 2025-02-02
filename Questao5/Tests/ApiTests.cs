using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Entities;
using Xunit;

namespace Questao5.Tests
{
    public class ApiTests
    {
        [Fact]
        public void DeveCriarMovimentoCorretamente()
        {
            var (idContaCorrente, tipoMovimento, valor) = (Guid.NewGuid().ToString().ToUpper(), "D", 100.50);

            Movimento movimento_substituto = Substitute.For<Movimento>(idContaCorrente, tipoMovimento, valor);

            Exception exception = Record.Exception(() => Substitute.For<Movimento>(idContaCorrente, tipoMovimento, valor));
            Assert.Null(exception);

            Assert.Equal(idContaCorrente, movimento_substituto.IdContaCorrente);
            Assert.Equal(tipoMovimento, movimento_substituto.TipoMovimento);
            Assert.Equal(valor, movimento_substituto.Valor);

            Assert.True(movimento_substituto.Valor > 0D);
        }

        [Fact]
        public void DeveCriarMovimentoComErro()
        {
            var (idContaCorrente, tipoMovimento, valor) = ("123456", "X", -100.50);

            Movimento movimento_substituto = Substitute.For<Movimento>(idContaCorrente, tipoMovimento, valor);

            Exception exception = Record.Exception(() => Substitute.For<Movimento>(idContaCorrente, tipoMovimento, valor));
            Assert.Null(exception);

            Assert.Equal(idContaCorrente, movimento_substituto.IdContaCorrente);
            Assert.Equal(tipoMovimento, movimento_substituto.TipoMovimento);
            Assert.Equal(valor, movimento_substituto.Valor);

            Assert.True(movimento_substituto.Valor > 0);
        }


        [Fact]
        public void DeveCriarIdempotenciaCorretamente()
        {
            var (chaveIdempotencia, requisicao, resultado) = 
                (Guid.NewGuid().ToString().ToUpper(), 
                new EfetuarMovimentacaoFinanceiraCommand { ContaId = Guid.NewGuid().ToString(), RequisicaoId = Guid.NewGuid().ToString() , TipoMovimento = "D", ValorTotal = 10.50 }.ToString(), 
                string.Empty);

            Idempotencia movimento_substituto = Substitute.For<Idempotencia>(chaveIdempotencia, requisicao, resultado);

            Exception exception = Record.Exception(() => Substitute.For<Idempotencia>(chaveIdempotencia, requisicao, resultado));
            Assert.Null(exception);

            Assert.Equal(chaveIdempotencia, movimento_substituto.ChaveIdempotencia);
            Assert.Equal(requisicao, movimento_substituto.Requisicao);
            Assert.Equal(resultado, movimento_substituto.Resultado);
        }

        [Fact]
        public void DeveCriarIdempotenciaComErro()
        {
            var (chaveIdempotencia, requisicao, resultado) = (Guid.NewGuid().ToString().ToUpper(), string.Empty, string.Empty);

            Idempotencia movimento_substituto = Substitute.For<Idempotencia>(chaveIdempotencia, requisicao, resultado);

            Exception exception = Record.Exception(() => Substitute.For<Idempotencia>(chaveIdempotencia, requisicao, resultado));
            Assert.Null(exception);

            Assert.Equal(chaveIdempotencia, movimento_substituto.ChaveIdempotencia);
            Assert.Equal(requisicao, movimento_substituto.Requisicao);
            Assert.Equal(resultado, movimento_substituto.Resultado);
        }
    }
}
