using ProjectPRN232.Models;

namespace ProjectPRN232.Service
{
    public interface ISystemAccountRepository
    {
        Task<SystemAccount?> GetByEmailAsync(string email);
        Task<SystemAccount?> GetByIdAsync(int id);
        Task AddAccountAsync(SystemAccount account);
        void Add(SystemAccount account);
        void Update(SystemAccount account);
        void Delete(SystemAccount account);
        Task SaveChangesAsync();
        Task<List<SystemAccount>> GetAllAsync();
    }
}
