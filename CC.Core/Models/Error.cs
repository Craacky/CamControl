using System.Collections.Generic;

namespace CC.Core.Models;

public class Error
{
    public string? TypeError { get; set; }
    public string? ProductCode { get; set; }

    public List<string> BoxCodes { get; set; } = new List<string>();
    public List<string> PalletCodes { get; set; } = new List<string>();
}