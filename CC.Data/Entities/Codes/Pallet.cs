using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CC.Data.Entities.Base;
using CC.Data.Entities.Tasks;

namespace CC.Data.Entities.Codes;

public class Pallet : Entity
{
    [MaxLength(30)] public string? MarkingCode { get; set; }

    public bool IsFulled { get; set; }

    public Guid ReportTaskGuid { get; set; }

    public int LineId { get; set; }

    public int ReportTaskId { get; set; }

    public ReportTask? ReportTask { get; set; }

    public List<Box> Boxes { get; set; } = new List<Box>();
}