using ProjectPRN232.Models;

namespace ProjectPRN232.Service
{
    public interface ITokenService
    {
        string GenerateToken(SystemAccount account);
    }
}
