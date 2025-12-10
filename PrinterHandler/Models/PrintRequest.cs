using System.Collections.Generic;

namespace PrinterHandler.Models
{
    public class PrintRequest
    {
        public string PrinterAddress { get; set; }
        public string LabelTemplatePath { get; set; }
        public Dictionary<string, string> LabelData { get; set; }
        public int Copies { get; set; } = 1;
    }
}