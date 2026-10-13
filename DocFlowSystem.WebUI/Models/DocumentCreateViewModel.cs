using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DocFlowSystem.WebUI.Models
{
    public class DocumentCreateViewModel
    {
        [Required(ErrorMessage = "يرجى إدخال رقم المعاملة")]
        [Display(Name = "رقم الفاكس / المعاملة")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "يرجى إدخال رقم القيد")]
        [Display(Name = "رقم القيد")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "يرجى اختيار التاريخ")]
        [Display(Name = "تاريخ الورود / الصدور")]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "يرجى إدخال الموضوع")]
        [Display(Name = "الموضوع")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "يرجى تحديد الجهة الوارد منها")]
        [Display(Name = "الجهة الوارد منها")]
        public string SourceEntity { get; set; } = string.Empty;

        [Required(ErrorMessage = "يرجى تحديد القسم المختص")]
        [Display(Name = "القسم المختص")]
        public string DestinationDepartment { get; set; } = string.Empty;

        [Display(Name = "ملف وثيقة الفاكس الأصلية")]
        public IFormFile? AttachmentFile { get; set; }

        [Display(Name = "متابعة المكتب")]
        public bool IsUnderFollowUp { get; set; }

        [Display(Name = "اسم المستلم")]
        public string? ReceiverName { get; set; }
    }
}