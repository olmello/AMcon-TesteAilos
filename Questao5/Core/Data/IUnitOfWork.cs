namespace Questao5.Core.Data
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }
}
