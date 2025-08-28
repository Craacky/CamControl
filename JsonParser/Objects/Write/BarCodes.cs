using System.Collections.Generic;

namespace JsonParser.Objects.Write;

public class BarCodes
{
    public string? BarCode { get; set; }
    public int Level { get; set; }
    public int NumberInTask { get; set; }
    public int? Weight { get; set; }
    public ICollection<BarCodes>? ChildBarCodes { get; set; }
}