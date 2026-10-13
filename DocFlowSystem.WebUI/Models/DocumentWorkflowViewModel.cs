using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DocFlowSystem.WebUI.Models
{
    public class ManagerDecisionViewModel
    {
        public int DocumentId { get; set; }

        [Required(ErrorMessage = "يرجى كتابة قرار/توجيه رئيس الفرع")]
        [Display(Name = "قرار رئيس الفرع / التأشيرة")]
        public string ManagerDecision { get; set; } = string.Empty;

        [Display(Name = "تحديث حالة المعاملة")]
        public int StatusId { get; set; } = 2; // PendingAction (قيد الإجراء)
    }

    public class SectionActionViewModel
    {
        public int DocumentId { get; set; }

        [Required(ErrorMessage = "يرجى كتابة تفاصيل الإجراء المتخذ")]
        [Display(Name = "الإجراء المتخذ بواسطة القسم")]
        public string ActionTaken { get; set; } = string.Empty;

        [Display(Name = "ملف الإجراء / الرد المرفق")]
        public IFormFile? ActionFile { get; set; }
    }
}