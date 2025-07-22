using Microsoft.EntityFrameworkCore;
using ProjectPRN232.Models;

namespace ProjectPRN232.Service
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        private readonly NewsDbContext _context;
        public NewsArticleRepository(NewsDbContext context)
        {
            _context = context;
        }

        public IQueryable<NewsArticle> GetAll()
        {
            return _context.NewsArticles
                .Include(x => x.Category)
                .Include(x => x.CreatedBy)
                .Include(x => x.UpdatedBy);
        }

        public async Task<NewsArticle?> GetByIdAsync(int id)
        {
            return await _context.NewsArticles
                .Include(x => x.Category)
                .Include(x => x.CreatedBy)
                .Include(x => x.UpdatedBy)
                .FirstOrDefaultAsync(x => x.NewsArticleId == id);
        }

        public async Task<NewsArticle> AddAsync(NewsArticle article)
        {
            _context.NewsArticles.Add(article);
            await _context.SaveChangesAsync();
            return article;
        }

        public async Task UpdateAsync(NewsArticle article)
        {
            _context.NewsArticles.Update(article);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(NewsArticle article)
        {
            _context.NewsArticles.Remove(article);
            await _context.SaveChangesAsync();
        }

    }

}
