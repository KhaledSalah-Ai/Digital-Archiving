namespace DocFlowSystem.Application.DTOs
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string SourceEntity { get; set; } = string.Empty;
        public string DestinationDepartment { get; set; } = string.Empty;

        public string? AttachmentFilePath { get; set; }
        public string? ActionFilePath { get; set; }

        public string StatusName { get; set; } = string.Empty; // لعرض اسم الحالة باللغة العربية
        public int StatusId { get; set; }

        public string? ManagerDecision { get; set; }
        public string? ActionTaken { get; set; }
        public bool IsUnderFollowUp { get; set; }
        public string? ReceiverName { get; set; }
    }
}