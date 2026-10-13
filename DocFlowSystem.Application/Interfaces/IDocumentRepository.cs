using DocFlowSystem.Domain.Entities;

namespace DocFlowSystem.Application.Interfaces
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> GetAllAsync();
        Task<Document?> GetByIdAsync(int id);
        Task AddAsync(Document document);
        void Update(Document document);
        void Delete(Document document);
        Task<bool> SaveChangesAsync();
    }
}