using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PrinterHandler.Models;

namespace CamFusion.Services
{
    public class PrinterHandlerTest
    {
        public static void TestPrintRequest()
        {
            // Create sample print request data
            var printRequest = new PrintRequest
            {
                PrinterAddress = "192.168.1.100", // Example printer IP
                LabelTemplatePath = @"C:\Labels\TestTemplate.btw",
                LabelData = new Dictionary<string, string>
                {
                    { "NAME", "Test Product" },
                    { "BATCH", "0001" },
                    { "SDATE", "01.01.24" },
                    { "GTIN", "1234567890123" }
                },
                Copies = 2
            };

            string jsonData = JsonConvert.SerializeObject(printRequest);
            Console.WriteLine($"Sample print request JSON: {jsonData}");
            
            // This shows the JSON format that will be sent to PrinterHandler
            Console.WriteLine("\nThis is the format that will be sent to the PrinterHandler executable.");
        }
    }
}