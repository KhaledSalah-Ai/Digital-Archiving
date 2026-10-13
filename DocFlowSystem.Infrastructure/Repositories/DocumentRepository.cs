using DocFlowSystem.Application.Interfaces;
using DocFlowSystem.Domain.Entities;
using DocFlowSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DocFlowSystem.Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _context.Documents
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<Document?> GetByIdAsync(int id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task AddAsync(Document document)
        {
            await _context.Documents.AddAsync(document);
        }

        public void Update(Document document)
        {
            _context.Documents.Update(document);
        }

        public void Delete(Document document)
        {
            _context.Documents.Remove(document);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}