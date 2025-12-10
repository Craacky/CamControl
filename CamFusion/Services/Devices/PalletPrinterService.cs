using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using CC.Core.Devices.Impl;
using CC.Core.Services.Impl;
using CC.Data.Entities.Settings;
using CamFusion.Services;

namespace CamFusion.Services.Devices;

public class PalletPrinterService : PrinterDevice
{
    public PalletPrinterService(
        DeviceSettings deviceSettings,
        LineSettings lineSettings,
        LocalDb localDbService,
        ReportTaskService reportTaskService
    )
        : base(deviceSettings, lineSettings, localDbService, reportTaskService)
    {
    }

    public override void PrintCode()
    {
        if (Device.IsUsed && _isRun)
        {
            // Prepare label data for the PrinterHandler
            var labelData = new Dictionary<string, string>();

            // Load label data from report task and attributes
            var reportTask = LocalDbService.ReportTaskDataService.GetWithInclude(
                r => r.Guid == ReportTaskService.CurrentReportTask.Guid,
                rt => rt.Nomenclature
            );

            if (reportTask == null)
            {
                System.Diagnostics.Debug.WriteLine("Error: Report task not found");
                return; // Exit if report task is not found
            }

            var attributes = LocalDbService
                .AttributeDataService.GetAll(a => a.NomenclatureId == reportTask.NomenclatureId)
                .ToList();

            if (attributes.Count < 8) // Need at least 8 attributes based on indexing
            {
                System.Diagnostics.Debug.WriteLine("Error: Not enough attributes for label data");
                return; // Exit if there aren't enough attributes
            }

            // Add all the required label data
            if (reportTask.Nomenclature == null)
            {
                System.Diagnostics.Debug.WriteLine("Error: Report task nomenclature not found");
                return; // Exit if nomenclature is not found
            }

            labelData["NAME"] = reportTask.Nomenclature.Name.Replace("\"", "\\[\"\"]");
            labelData["MARK"] = attributes[0].Value.Replace("\"", "\\[\"\"]");
            labelData["STB"] = attributes[7].Value;
            labelData["BATCH"] = $"{ReportTaskService.CurrentReportTask.LotNumber:0000}";
            labelData["SDATE"] = ReportTaskService.CurrentReportTask.ManufactureDate.ToString("dd.MM.yy");
            labelData["EDATE"] = ReportTaskService.CurrentReportTask.ExpiryDate.ToString("dd.MM.yy");
            labelData["SCODEDATE"] = ReportTaskService.CurrentReportTask.ManufactureDate.ToString("yyMMdd");
            labelData["ECODEDATE"] = ReportTaskService.CurrentReportTask.ExpiryDate.ToString("yyMMdd");
            labelData["ITEMS"] = (ReportTaskService.Statistic.CountBoxInCurrentPallet * 12).ToString();
            labelData["GTIN"] = reportTask.Nomenclature.Gtin;
            labelData["SERIAL"] = $"{ReportTaskService.Statistic.PalletCodes.Count}";

            // Use PrinterHandler to print the label
            var printerHandlerService = new PrinterHandlerService();
            var result = printerHandlerService.PrintLabel(
                DeviceSettings.Ip, // Printer address
                @"C:\Labels\PalletTemplate.btw", // Template path - will be configurable later
                labelData,
                1 // Number of copies
            );

            if (!result.Success)
            {
                // Log or handle the error appropriately
                System.Diagnostics.Debug.WriteLine($"PrinterHandler Error: {result.Error}");
            }
        }
    }

    public override void LoadTemplates()
    {
        if (Device.IsConnected && _isRun)
        {
            LoadPatternTask();
            LoadPatternMessage();
        }
    }

    protected override void SendCommandToClearTask()
    {
        //string commandToClearTask = $"{(char)1}FGA---r--------{(char)23}\r\n";
        //_client.SendMessage(commandToClearTask);
    }

    protected override void SendCommadToSetAutoStatus()
    {
        //string commandToSetAutoStatus = $"\u0027!S\r\n";
        //_client.SendMessage(commandToSetAutoStatus);
    }

    private void LoadPatternTask()
    {
        var reportTask =
            LocalDbService.ReportTaskDataService.GetWithInclude(
                r => r.Guid == ReportTaskService.CurrentReportTask.Guid,
                rt => rt.Nomenclature
            );
        var attributes = LocalDbService
            .AttributeDataService.GetAll(a => a.NomenclatureId == reportTask!.NomenclatureId)
            .ToList();

        if (Device.IsConnected && !File.Exists("TSCLABEL.txt"))
        {
            MessageBox.Show("Отсутствует файл с шаблоном печати. Печать невозможна.");
            Stop();
        }
        else
        {
            StreamReader reader = new("TSCLABEL.txt", Encoding.UTF8);
            patternTask = reader.ReadToEnd();
            patternTask = patternTask.Replace(
                "<NAME>",
                reportTask.Nomenclature.Name.Replace("\"", "\\[\"]")
            );
            patternTask = patternTask.Replace(
                "<MARK>",
                attributes[0].Value.Replace("\"", "\\[\"]")
            );
            patternTask = patternTask.Replace("<STB>", attributes[7].Value);
            /*patternTask = patternTask.Replace("<NETTO>", attributes[8].Value);

            patternTask = patternTask.Replace("<BRUTTO>", attributes[9].Value);
            patternTask = patternTask.Replace("<NETTOP>", attributes[13].Value);
            patternTask = patternTask.Replace("<NETTOG>", attributes[13].Value);
            /*string minTemp = attributes[16].Value;
            string maxTemp = attributes[17].Value;
            patternTask = patternTask.Replace("<CONDITION>", $"Хранить при температуре от {minTemp} до {maxTemp}");
            patternTask = patternTask.Replace("<MINT>", $"{minTemp}");
            patternTask = patternTask.Replace("<MAXT>", $"{maxTemp}");
            */
            patternTask = patternTask.Replace(
                "<BATCH>",
                $"{ReportTaskService.CurrentReportTask.LotNumber:0000}"
            );

            patternTask = patternTask.Replace(
                "<SDATE>",
                ReportTaskService.CurrentReportTask.ManufactureDate.ToString("dd.MM.yy")
            );
            patternTask = patternTask.Replace(
                "<EDATE>",
                ReportTaskService.CurrentReportTask.ExpiryDate.ToString("dd.MM.yy")
            );

            patternTask = patternTask.Replace(
                "<SCODEDATE>",
                ReportTaskService.CurrentReportTask.ManufactureDate.ToString("yyMMdd")
            );
            patternTask = patternTask.Replace(
                "<ECODEDATE>",
                ReportTaskService.CurrentReportTask.ExpiryDate.ToString("yyMMdd")
            );
            //patternTask = patternTask.Replace(
            //    "<ITEMS>",
            //    Convert.ToString(
            //        Convert.ToInt32(ReportTaskService.CurrentReportTask.CountBoxInPallet)
            //            * Convert.ToInt32(ReportTaskService.CurrentReportTask.CountProductInBox)
            //    )
            //);
            patternTask = patternTask.Replace("<ITEMS>",
                (ReportTaskService.Statistic.CountBoxInCurrentPallet * 12).ToString());
            patternTask = patternTask.Replace(
                "<GTIN>",
                ReportTaskService.CurrentReportTask.Nomenclature!.Gtin
            );

            _ = _client.SendMessageAsync(patternTask);
        }
    }

    private void LoadPatternMessage()
    {
        if (Device.IsConnected && !File.Exists("TSCPrint.txt"))
        {
            MessageBox.Show("Отсутствует файл с шаблоном печати. Печать невозможна.");
            Disconnect();
        }
        else
        {
            StreamReader reader = new("TSCPrint.txt", Encoding.UTF8);
            patternMessage = reader.ReadToEnd();

            patternMessage = patternMessage.Replace("<SIZE>", "2");
        }
    }
}