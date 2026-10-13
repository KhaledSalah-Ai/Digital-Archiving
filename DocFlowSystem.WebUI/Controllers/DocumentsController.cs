using DocFlowSystem.Application.DTOs;
using DocFlowSystem.Application.Interfaces;
using DocFlowSystem.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocFlowSystem.WebUI.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        // عرض كافة الوثائق والفاكسات مع إمكانية البحث
        public async Task<IActionResult> Index(string? searchTerm)
        {
            ViewBag.CurrentSearch = searchTerm; // للحفاظ على الكلمة مكتوبة داخل مربع البحث
            var documents = await _documentService.GetAllDocumentsAsync(searchTerm);
            return View(documents);
        }

        // 2. عرض تفاصيل وثيقة محددة
        public async Task<IActionResult> Details(int id)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null) return NotFound();
            return View(document);
        }

        // 3. شاشة إضافة فاكس جديد (GET)
        public IActionResult Create()
        {
            return View(new DocumentCreateViewModel());
        }

        // 4. حفظ الفاكس الجديد (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new DocumentDto
            {
                DocumentNumber = model.DocumentNumber,
                RegistrationNumber = model.RegistrationNumber,
                IssueDate = model.IssueDate,
                Subject = model.Subject,
                SourceEntity = model.SourceEntity,
                DestinationDepartment = model.DestinationDepartment,
                IsUnderFollowUp = model.IsUnderFollowUp,
                ReceiverName = model.ReceiverName
            };

            await _documentService.CreateDocumentAsync(dto, model.AttachmentFile);
            return RedirectToAction(nameof(Index));
        }
        // 1. عرض صفحة التعديل (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var doc = await _documentService.GetDocumentByIdAsync(id);
            if (doc == null) return NotFound();

            var viewModel = new DocumentCreateViewModel
            {
                DocumentNumber = doc.DocumentNumber,
                RegistrationNumber = doc.RegistrationNumber,
                IssueDate = doc.IssueDate,
                Subject = doc.Subject,
                SourceEntity = doc.SourceEntity,
                DestinationDepartment = doc.DestinationDepartment,
                IsUnderFollowUp = doc.IsUnderFollowUp,
                ReceiverName = doc.ReceiverName
            };

            ViewBag.DocumentId = id;
            ViewBag.CurrentAttachment = doc.AttachmentFilePath;
            return View(viewModel);
        }

        // 2. حفظ التعديلات (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DocumentId = id;
                return View(model);
            }

            var dto = new DocumentDto
            {
                Id = id,
                DocumentNumber = model.DocumentNumber,
                RegistrationNumber = model.RegistrationNumber,
                IssueDate = model.IssueDate,
                Subject = model.Subject,
                SourceEntity = model.SourceEntity,
                DestinationDepartment = model.DestinationDepartment,
                IsUnderFollowUp = model.IsUnderFollowUp,
                ReceiverName = model.ReceiverName
            };

            var result = await _documentService.UpdateDocumentAsync(dto, model.AttachmentFile);
            if (!result) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // 3. تأكيد الحذف (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _documentService.DeleteDocumentAsync(id);
            return RedirectToAction(nameof(Index));
        }
        // قرار رئيس الفرع 
        // 1. إضافة قرار رئيس الفرع (GET)
        public async Task<IActionResult> ManagerDecision(int id)
        {
            var doc = await _documentService.GetDocumentByIdAsync(id);
            if (doc == null) return NotFound();

            var model = new ManagerDecisionViewModel
            {
                DocumentId = doc.Id,
                ManagerDecision = doc.ManagerDecision ?? string.Empty
            };

            ViewBag.DocumentNumber = doc.DocumentNumber;
            ViewBag.Subject = doc.Subject;
            return View(model);
        }

        // إضافة قرار رئيس الفرع (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagerDecision(ManagerDecisionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _documentService.UpdateDocumentStatusAsync(model.DocumentId, model.StatusId, model.ManagerDecision);
            return RedirectToAction(nameof(Index));
        }

        // 2. إضافة إجراء القسم المختص (GET)
        public async Task<IActionResult> SectionAction(int id)
        {
            var doc = await _documentService.GetDocumentByIdAsync(id);
            if (doc == null) return NotFound();

            var model = new SectionActionViewModel
            {
                DocumentId = doc.Id,
                ActionTaken = doc.ActionTaken ?? string.Empty
            };

            ViewBag.DocumentNumber = doc.DocumentNumber;
            ViewBag.Subject = doc.Subject;
            ViewBag.ManagerDecision = doc.ManagerDecision;
            return View(model);
        }

        // إضافة إجراء القسم المختص (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SectionAction(SectionActionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _documentService.AddSectionActionAsync(model.DocumentId, model.ActionTaken, model.ActionFile);
            return RedirectToAction(nameof(Index));
        }

        
    }
}