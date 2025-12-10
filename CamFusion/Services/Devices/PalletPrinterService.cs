using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using CC.Core.Devices.Impl;
using CC.Core.Services.Impl;
using CC.Data.Entities.Settings;

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
            LoadPatternTask();
            string messageToSend = patternMessage;
            messageToSend = messageToSend.Replace(
                "<SERIAL>",
                $"{ReportTaskService.Statistic.PalletCodes.Count}"
            );

            _ = _client.SendMessageAsync(messageToSend);
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