using DocFlowSystem.Application.DTOs;
using DocFlowSystem.Application.Interfaces;
using DocFlowSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DocFlowSystem.Application.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _repository;

        public DocumentService(IDocumentRepository repository)
        {
            _repository = repository;
        }

        // 1. جلب كافة الوثائق والفاكسات مع دعم البحث والفلترة
        public async Task<IEnumerable<DocumentDto>> GetAllDocumentsAsync(string? searchTerm = null)
        {
            var docs = await _repository.GetAllAsync();

            // فلترة النتائج في حال تم إدخال كلمة بحث
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                docs = docs.Where(d =>
                    (d.DocumentNumber != null && d.DocumentNumber.ToLower().Contains(searchTerm)) ||
                    (d.RegistrationNumber != null && d.RegistrationNumber.ToLower().Contains(searchTerm)) ||
                    (d.Subject != null && d.Subject.ToLower().Contains(searchTerm)) ||
                    (d.SourceEntity != null && d.SourceEntity.ToLower().Contains(searchTerm))
                );
            }

            return docs.Select(d => new DocumentDto
            {
                Id = d.Id,
                DocumentNumber = d.DocumentNumber,
                RegistrationNumber = d.RegistrationNumber,
                IssueDate = d.IssueDate,
                Subject = d.Subject,
                SourceEntity = d.SourceEntity,
                DestinationDepartment = d.DestinationDepartment,
                AttachmentFilePath = d.AttachmentFilePath,
                ActionFilePath = d.ActionFilePath,
                StatusId = (int)d.Status,
                StatusName = GetArabicStatusName(d.Status),
                ManagerDecision = d.ManagerDecision,
                ActionTaken = d.ActionTaken,
                IsUnderFollowUp = d.IsUnderFollowUp,
                ReceiverName = d.ReceiverName
            });
        }

        // 2. جلب وثيقة محددة بالمعرف
        public async Task<DocumentDto?> GetDocumentByIdAsync(int id)
        {
            var d = await _repository.GetByIdAsync(id);
            if (d == null) return null;

            return new DocumentDto
            {
                Id = d.Id,
                DocumentNumber = d.DocumentNumber,
                RegistrationNumber = d.RegistrationNumber,
                IssueDate = d.IssueDate,
                Subject = d.Subject,
                SourceEntity = d.SourceEntity,
                DestinationDepartment = d.DestinationDepartment,
                AttachmentFilePath = d.AttachmentFilePath,
                ActionFilePath = d.ActionFilePath,
                StatusId = (int)d.Status,
                StatusName = GetArabicStatusName(d.Status),
                ManagerDecision = d.ManagerDecision,
                ActionTaken = d.ActionTaken,
                IsUnderFollowUp = d.IsUnderFollowUp,
                ReceiverName = d.ReceiverName
            };
        }

        // 3. إنشاء فاكس / وثيقة جديدة
        public async Task<int> CreateDocumentAsync(DocumentDto dto, IFormFile? attachmentFile)
        {
            var doc = new Document
            {
                DocumentNumber = dto.DocumentNumber,
                RegistrationNumber = dto.RegistrationNumber,
                IssueDate = dto.IssueDate,
                Subject = dto.Subject,
                SourceEntity = dto.SourceEntity,
                DestinationDepartment = dto.DestinationDepartment,
                IsUnderFollowUp = dto.IsUnderFollowUp,
                ReceiverName = dto.ReceiverName,
                Status = DocumentStatus.New
            };

            if (attachmentFile != null && attachmentFile.Length > 0)
            {
                doc.AttachmentFilePath = await SaveFileAsync(attachmentFile, "attachments");
            }

            await _repository.AddAsync(doc);
            await _repository.SaveChangesAsync();
            return doc.Id;
        }

        // 4. تعديل بيانات الوثيقة والملف المرفق
        public async Task<bool> UpdateDocumentAsync(DocumentDto dto, IFormFile? newAttachmentFile)
        {
            var doc = await _repository.GetByIdAsync(dto.Id);
            if (doc == null) return false;

            doc.DocumentNumber = dto.DocumentNumber;
            doc.RegistrationNumber = dto.RegistrationNumber;
            doc.IssueDate = dto.IssueDate;
            doc.Subject = dto.Subject;
            doc.SourceEntity = dto.SourceEntity;
            doc.DestinationDepartment = dto.DestinationDepartment;
            doc.IsUnderFollowUp = dto.IsUnderFollowUp;
            doc.ReceiverName = dto.ReceiverName;
            doc.ManagerDecision = dto.ManagerDecision;
            doc.ActionTaken = dto.ActionTaken;

            if (newAttachmentFile != null && newAttachmentFile.Length > 0)
            {
                doc.AttachmentFilePath = await SaveFileAsync(newAttachmentFile, "attachments");
            }

            _repository.Update(doc);
            return await _repository.SaveChangesAsync();
        }

        // 5. حذف وثيقة
        public async Task<bool> DeleteDocumentAsync(int id)
        {
            var doc = await _repository.GetByIdAsync(id);
            if (doc == null) return false;

            _repository.Delete(doc);
            return await _repository.SaveChangesAsync();
        }

        // 6. تحديث قرار وحالة رئيس الفرع
        public async Task<bool> UpdateDocumentStatusAsync(int id, int statusId, string? managerDecision)
        {
            var doc = await _repository.GetByIdAsync(id);
            if (doc == null) return false;

            doc.Status = (DocumentStatus)statusId;
            if (!string.IsNullOrEmpty(managerDecision))
            {
                doc.ManagerDecision = managerDecision;
            }

            _repository.Update(doc);
            return await _repository.SaveChangesAsync();
        }

        // 7. إضافة إجراء وتأشيرة القسم المختص
        public async Task<bool> AddSectionActionAsync(int id, string actionTaken, IFormFile? actionFile)
        {
            var doc = await _repository.GetByIdAsync(id);
            if (doc == null) return false;

            doc.ActionTaken = actionTaken;
            doc.Status = DocumentStatus.Completed;

            if (actionFile != null && actionFile.Length > 0)
            {
                doc.ActionFilePath = await SaveFileAsync(actionFile, "actions");
            }

            _repository.Update(doc);
            return await _repository.SaveChangesAsync();
        }

        // دالة مساعدة لحفظ الملفات في مجلد wwwroot
        private async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/{folderName}/" + uniqueFileName;
        }

        // دالة تحويل الحالة لرقم عربي
        private static string GetArabicStatusName(DocumentStatus status) => status switch
        {
            DocumentStatus.New => "جديد",
            DocumentStatus.PendingAction => "قيد الإجراء",
            DocumentStatus.Completed => "تم الانتهاء",
            DocumentStatus.Archived => "مؤرشف",
            _ => "غير محدد"
        };
    }
}