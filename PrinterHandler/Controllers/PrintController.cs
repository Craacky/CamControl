using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using PrinterHandler.Models;
using PrinterHandler.Services;

namespace PrinterHandler.Controllers
{
    public class PrintController
    {
        private readonly BartenderPrintService _bartenderService;

        public PrintController()
        {
            _bartenderService = new BartenderPrintService();
        }

        public void HandlePrintRequest(string jsonData)
        {
            try
            {
                // Deserialize the JSON data
                var printRequest = JsonConvert.DeserializeObject<PrintRequest>(jsonData);
                
                if (printRequest == null)
                {
                    Console.WriteLine("Error: Invalid JSON data received");
                    return;
                }
                
                // Perform the print operation
                var result = _bartenderService.PrintLabel(printRequest);
                
                // Output the result
                if (result.Success)
                {
                    Console.WriteLine($"Success: {result.Message}");
                }
                else
                {
                    Console.WriteLine($"Error: {result.Error}");
                }
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON Error: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}