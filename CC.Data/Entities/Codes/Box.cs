using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CC.Data.Entities.Base;

namespace CC.Data.Entities.Codes;

public class Box : Entity
{
    [MaxLength(30)] public string? MarkingCode { get; set; }

    public Guid ReportTaskGuid { get; set; }

    public int LineId { get; set; }

    public int? PalletId { get; set; }

    public Pallet? Pallet { get; set; }

    public List<Product> Products { get; set; } = new List<Product>();
}