using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using PrinterHandler.Models;

namespace CamFusion.Services
{
    public class PrinterHandlerService
    {
        private readonly string _printerHandlerPath;

        public PrinterHandlerService()
        {
            // Try multiple potential paths for the PrinterHandler executable
            string[] possiblePaths = {
                // Path when running from CamFusion output directory
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "PrinterHandler", "bin", "Debug", "net6.0-windows", "PrinterHandler.exe"),
                // Path when both are built to the same output directory
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PrinterHandler.exe"),
                // Relative path from solution root
                Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.FullName ?? AppDomain.CurrentDomain.BaseDirectory, "PrinterHandler", "bin", "Debug", "net6.0-windows", "PrinterHandler.exe"),
                // Common output path
                @"..\..\PrinterHandler\bin\Debug\net6.0-windows\PrinterHandler.exe"
            };

            _printerHandlerPath = possiblePaths[0]; // Start with the first option
            foreach (string path in possiblePaths)
            {
                string fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    _printerHandlerPath = fullPath;
                    break;
                }
            }
        }

        public PrintResult PrintLabel(string printerAddress, string labelTemplatePath, Dictionary<string, string> labelData, int copies = 1)
        {
            var printRequest = new PrintRequest
            {
                PrinterAddress = printerAddress,
                LabelTemplatePath = labelTemplatePath,
                LabelData = labelData,
                Copies = copies
            };

            string jsonData = JsonConvert.SerializeObject(printRequest);

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = _printerHandlerPath,
                    Arguments = $"\"{jsonData}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(_printerHandlerPath)
                };

                using (var process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    int exitCode = process.ExitCode;

                    if (exitCode == 0)
                    {
                        return new PrintResult
                        {
                            Success = true,
                            Message = output
                        };
                    }
                    else
                    {
                        return new PrintResult
                        {
                            Success = false,
                            Error = string.IsNullOrEmpty(error) ? output : error
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new PrintResult
                {
                    Success = false,
                    Error = $"Exception while calling PrinterHandler: {ex.Message}"
                };
            }
        }
    }
}