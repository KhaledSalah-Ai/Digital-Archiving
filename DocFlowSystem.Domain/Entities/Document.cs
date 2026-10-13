namespace DocFlowSystem.Domain.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;     // رقم الفاكس / المعاملة
        public string RegistrationNumber { get; set; } = string.Empty; // رقم القيد
        public DateTime IssueDate { get; set; } = DateTime.Now;        // تاريخ الصدور / الوصول
        public string Subject { get; set; } = string.Empty;            // الموضوع

        public string SourceEntity { get; set; } = string.Empty;        // الجهة الوارد منها
        public string DestinationDepartment { get; set; } = string.Empty; // القسم المختص

        // مسارات الملفات المخزنة في السيرفر
        public string? AttachmentFilePath { get; set; }  // مسار وثيقة الفاكس الأصلية
        public string? ActionFilePath { get; set; }      // مسار ملف الإجراء المتخذ

        // سير العمل والقرارات
        public DocumentStatus Status { get; set; } = DocumentStatus.New;
        public string? ManagerDecision { get; set; }     // قرار رئيس الفرع
        public string? ActionTaken { get; set; }         // الإجراء المتخذ

        // المتابعة والاستلام
        public bool IsUnderFollowUp { get; set; }
        public string? ReceiverName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum DocumentStatus
    {
        New = 1,
        PendingAction = 2,
        Completed = 3,
        Archived = 4
    }
}