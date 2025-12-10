using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PrinterHandler.Models;
using Seagull.BarTender.Print;

namespace PrinterHandler.Services
{
    public class BartenderPrintService : IDisposable
    {
        private Engine _btEngine;
        private bool _disposed = false;

        public BartenderPrintService()
        {
            _btEngine = new Engine(true);
        }

        public Models.PrintResult PrintLabel(Models.PrintRequest request)
        {
            LabelFormatDocument btFormat = null;

            try
            {
                // Validate request
                if (request == null)
                {
                    return new Models.PrintResult
                    {
                        Success = false,
                        Error = "Print request cannot be null"
                    };
                }

                // Determine label template path
                string labelTemplatePath = !string.IsNullOrEmpty(request.LabelTemplatePath)
                    ? request.LabelTemplatePath
                    : @"C:\Labels\Template.btw";

                // Validate template path exists
                if (!System.IO.File.Exists(labelTemplatePath))
                {
                    return new Models.PrintResult
                    {
                        Success = false,
                        Error = $"Label template not found: {labelTemplatePath}"
                    };
                }

                // Open the format
                btFormat = _btEngine.Documents.Open(labelTemplatePath);
                
                if (btFormat == null)
                {
                    return new Models.PrintResult
                    {
                        Success = false,
                        Error = $"Could not open label template: {labelTemplatePath}"
                    };
                }

                // Set printer if specified
                if (!string.IsNullOrEmpty(request.PrinterAddress))
                {
                    btFormat.PrintSetup.PrinterName = request.PrinterAddress;
                }
                
                // Set label data
                if (request.LabelData != null)
                {
                    foreach (var data in request.LabelData)
                    {
                        try
                        {
                            btFormat.SubStrings[data.Key].Value = data.Value;
                        }
                        catch (Exception)
                        {
                            System.Diagnostics.Debug.WriteLine($"Warning: SubString '{data.Key}' not found in template");
                        }
                    }
                }

                Messages messages;
                Result result = btFormat.Print("LabelPrint", out messages);
                
                // Check for errors or warnings
                string errorMessage = null;
                if (messages != null && messages.Count > 0)
                {
                    var messageList = new List<string>();
                    foreach (Message msg in messages)
                    {
                        messageList.Add($"{msg.Severity}: {msg.Text}");
                    }
                    errorMessage = string.Join("; ", messageList);
                }

                if (result == 0) // 0 indicates success
                {
                    return new Models.PrintResult
                    {
                        Success = true,
                        Message = "Label printed successfully"
                    };
                }
                else
                {
                    return new Models.PrintResult
                    {
                        Success = false,
                        Error = $"BarTender print failed with result code: {result}. {errorMessage}"
                    };
                }
            }
            catch (COMException comEx)
            {
                return new Models.PrintResult
                {
                    Success = false,
                    Error = $"COM Exception during BarTender printing: {comEx.Message} (HRESULT: 0x{comEx.HResult:X})"
                };
            }
            catch (Exception ex)
            {
                return new Models.PrintResult
                {
                    Success = false,
                    Error = $"Exception during BarTender printing: {ex.Message}"
                };
            }
            finally
            {
                // Always close and release the format
                if (btFormat != null)
                {
                    try
                    {
                        btFormat.Close(SaveOptions.DoNotSaveChanges);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error closing format: {ex.Message}");
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    if (_btEngine != null)
                    {
                        try
                        {
                            _btEngine.Stop();
                            _btEngine.Dispose();
                            _btEngine = null;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error disposing BarTender engine: {ex.Message}");
                        }
                    }
                }

                _disposed = true;
            }
        }

        ~BartenderPrintService()
        {
            Dispose(false);
        }
    }
}