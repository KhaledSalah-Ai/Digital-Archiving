using DocFlowSystem.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace DocFlowSystem.Application.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentDto>> GetAllDocumentsAsync(string? searchTerm = null);
        Task<DocumentDto?> GetDocumentByIdAsync(int id);
        Task<int> CreateDocumentAsync(DocumentDto dto, IFormFile? attachmentFile);
        Task<bool> UpdateDocumentAsync(DocumentDto dto, IFormFile? newAttachmentFile); // جديد
        Task<bool> DeleteDocumentAsync(int id); // جديد
        Task<bool> UpdateDocumentStatusAsync(int id, int statusId, string? managerDecision);
        Task<bool> AddSectionActionAsync(int id, string actionTaken, IFormFile? actionFile);
    }
}