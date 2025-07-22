using ProjectPRN232.Models;

namespace ProjectPRN232.Service
{
    public interface INewsArticleRepository
    {
        IQueryable<NewsArticle> GetAll();
        Task<NewsArticle?> GetByIdAsync(int id);
        Task<NewsArticle> AddAsync(NewsArticle article);
        Task UpdateAsync(NewsArticle article);
        Task DeleteAsync(NewsArticle article);
    }

}
