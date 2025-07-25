using Microsoft.EntityFrameworkCore;
using ProjectPRN232.Models;
using ProjectPRN232.Service;
using System.Threading.Tasks;

public class SystemAccountRepository : ISystemAccountRepository
{
    private readonly NewsDbContext _context;

    public SystemAccountRepository(NewsDbContext context)
    {
        _context = context;
    }
    public async Task<SystemAccount?> GetByIdAsync(int id)
    {
        return await _context.SystemAccounts
            .FirstOrDefaultAsync(a => a.AccountId == id);
    }
    public async Task<SystemAccount?> GetByEmailAsync(string email)
    {
        return await _context.SystemAccounts
            .FirstOrDefaultAsync(a => a.AccountEmail == email);
    }


    public async Task AddAccountAsync(SystemAccount account)
    {
        _context.SystemAccounts.Add(account);
        await _context.SaveChangesAsync();
    }
    public void Add(SystemAccount account)
    {
        throw new NotImplementedException();
    }

    public void Delete(SystemAccount account)
    {
        throw new NotImplementedException();
    }

    public void Update(SystemAccount account)
    {
        _context.SystemAccounts.Update(account);
    }
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
    public async Task<List<SystemAccount>> GetAllAsync()
    {
        return await _context.SystemAccounts
            .Where(a => a.AccountRole != (int)Role.Admin) // không hiện admin lên
            .ToListAsync();
    }


}
