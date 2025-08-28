using System;
using System.ComponentModel.DataAnnotations;
using CC.Data.Entities.Base;

namespace CC.Data.Entities.Codes;

public sealed class Product : Entity
{
    [MaxLength(30)] public string? MarkingCode { get; set; }
    
    public Guid ReportTaskGuid { get; set; }
    
    public int LineId { get; set; }

    public int? BoxId { get; set; }
    
    public Box? Box { get; set; }
}