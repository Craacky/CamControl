using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CC.Data.Entities.Base;
using CC.Data.Entities.Tasks;

namespace CC.Data.Entities.Codes;

public enum VirtualBoxStatus
{
    Created,
    LabelGenerated,
    Verified,
    Expired,
    Timeout,
    Error
}

public class VirtualBox : Entity
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    
    public Guid ReportTaskGuid { get; set; }
    
    public int ReportTaskId { get; set; }
    
    public virtual ReportTask? ReportTask { get; set; }
    
    [MaxLength(30)]
    public string? BoxLabelCode { get; set; }
    
    [MaxLength(4000)]
    public string? ProductCodesJson { get; set; }
    
    public VirtualBoxStatus Status { get; set; } = VirtualBoxStatus.Created;
    
    public bool IsInMemory { get; set; } = true;
    
    public DateTime? ExpiredAt { get; set; }
    
    public DateTime? VerifiedAt { get; set; }
    
    [MaxLength(500)]
    public string? ErrorMessage { get; set; }
    
    [MaxLength(500)]
    public string? TimeoutReason { get; set; }
}

