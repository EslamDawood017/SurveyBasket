using System.ComponentModel.DataAnnotations.Schema;

namespace SurveyBasket.Api.Entities;

public class AuditableEntity
{
    [ForeignKey(nameof(CreatedBy))]  
    public int CreatedById { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UpdatedBy))] 
    public int? UpdatedById { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public ApplicationUser CreatedBy { get; set; } = default!;
    public ApplicationUser? UpdatedBy { get; set; }
}

