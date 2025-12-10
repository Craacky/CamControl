#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CamFusion.Services.Devices;
using CC.Core.Devices.Impl;
using CC.Core.Models;
using CC.Core.Services;
using CC.Core.Services.Impl;
using CC.Data.EFCore;
using CC.Data.Entities.Codes;
using CC.Data.Entities.Settings;
using SocketManager;

namespace CamFusion.Services;

public class DeviceService : DeviceServiceBase
{
    public DeviceService(ISettingsService settingsService,
        ReportTaskService? reportTaskService,
        LocalDb localDbService,
        ProcessingCodeService processingCodeService) : base(settingsService, reportTaskService, localDbService,
        processingCodeService)
    {
        VirtualBoxService = new VirtualBoxService(localDbService, reportTaskService, processingCodeService);
        SettingsService.SettingsChanged += SettingsService_SettingsChanged;
    }

    public new void StartDevices()
    {
        if (ReportTaskService?.CurrentReportTask == null || VirtualBoxService == null)
        {
            // нет активного задания — не запускаем устройства, чтобы избежать NRE
            return;
        }

        base.StartDevices();
        VirtualBoxService.StartTimeoutWatcher(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(1));
    }

    public new void StopDevices()
    {
        VirtualBoxService?.StopTimeoutWatcher();
        base.StopDevices();
    }

    private void SettingsService_SettingsChanged()
    {
        // When settings change, disconnect current devices and recreate them
        DisconnectDevices();
        CreateDevice();
    }


    public override void CreateDevice()
    {
        StopDevices();
        DisconnectDevices();

        StatisticCameraService = null!;
        ProductCamera1Service = null!;
        ProductCamera2Service = null!;
        VerificationCamera1Service = null!;
        VerificationCamera2Service = null!;
        BoxPrinterService = null!;
        PalletPrinterService = null!;
        TransportPrinterService = null!;

        Devices = new ObservableCollection<Device>();

        if (Settings.LocalDb.IsUsed)
        {
            Devices.Add(LocalDbService.Device);
            LocalDbService.ConnectionChanged += LocalDBService_ConnectionChanged;
        }

        if (SettingsService.Settings.StatisticCamera != null)
        {
            if (SettingsService.Settings.Line != null)
                StatisticCameraService = new GeneralCameraService(
                    SettingsService.Settings.StatisticCamera,
                    SettingsService.Settings.Line);
            if (SettingsService.Settings.StatisticCamera.IsUsed)
            {
                Devices.Add(StatisticCameraService.Device);
                ((GeneralCameraService)StatisticCameraService).ConnectionChanged +=
                    CameraService_ConnectionChanged;
                ((GeneralCameraService)StatisticCameraService).MessageReceived +=
                    StatisticCameraService_MessageReceived;
            }
        }

        if (SettingsService.Settings.Line != null)
        {
            if (SettingsService.Settings.ProductCamera1 != null)
            {
                ProductCamera1Service = new GeneralCameraService(SettingsService.Settings.ProductCamera1,
                    SettingsService.Settings.Line);
                if (SettingsService.Settings.ProductCamera1.IsUsed)
                {
                    Devices.Add(ProductCamera1Service.Device);
                    ((GeneralCameraService)ProductCamera1Service).ConnectionChanged +=
                        CameraService_ConnectionChanged;
                    ((GeneralCameraService)ProductCamera1Service).MessageReceived +=
                        ProductCamera1Service_MessageReceived;
                }
            }

            if (SettingsService.Settings.ProductCamera2 != null)
            {
                ProductCamera2Service = new GeneralCameraService(SettingsService.Settings.ProductCamera2,
                    SettingsService.Settings.Line);
                if (SettingsService.Settings.ProductCamera2.IsUsed)
                {
                    Devices.Add(ProductCamera2Service.Device);
                    ((GeneralCameraService)ProductCamera2Service).ConnectionChanged += CameraService_ConnectionChanged;
                    ((GeneralCameraService)ProductCamera2Service).MessageReceived += ProductCamera2Service_MessageReceived;
                }
            }

            if (SettingsService.Settings.VerificationCamera1 != null)
            {
                VerificationCamera1Service = new GeneralCameraService(SettingsService.Settings.VerificationCamera1,
                    SettingsService.Settings.Line);
                if (SettingsService.Settings.VerificationCamera1.IsUsed)
                {
                    Devices.Add(VerificationCamera1Service.Device);
                    ((GeneralCameraService)VerificationCamera1Service).ConnectionChanged += CameraService_ConnectionChanged;
                    ((GeneralCameraService)VerificationCamera1Service).MessageReceived += VerificationCamera1Service_MessageReceived;
                }
            }

            if (SettingsService.Settings.VerificationCamera2 != null)
            {
                VerificationCamera2Service = new GeneralCameraService(SettingsService.Settings.VerificationCamera2,
                    SettingsService.Settings.Line);
                if (SettingsService.Settings.VerificationCamera2.IsUsed)
                {
                    Devices.Add(VerificationCamera2Service.Device);
                    ((GeneralCameraService)VerificationCamera2Service).ConnectionChanged += CameraService_ConnectionChanged;
                    ((GeneralCameraService)VerificationCamera2Service).MessageReceived += VerificationCamera2Service_MessageReceived;
                }
            }

            if (SettingsService.Settings.BoxPrinter != null)
            {
                BoxPrinterService = new BoxPrinterService(SettingsService.Settings.BoxPrinter,
                    SettingsService.Settings.Line,
                    LocalDbService,
                    ReportTaskService);
                if (SettingsService.Settings.BoxPrinter != null && SettingsService.Settings.BoxPrinter.IsUsed)
                {
                    Devices.Add(BoxPrinterService.Device);
                    ((BoxPrinterService)BoxPrinterService).ConnectionChanged += PrinterDeviceService_ConnectionChanged;
                }
            }

            if (SettingsService.Settings.PalletPrinter != null)
            {
                PalletPrinterService = new PalletPrinterService(SettingsService.Settings.PalletPrinter,
                    SettingsService.Settings.Line,
                    LocalDbService,
                    ReportTaskService);
            }

            if (SettingsService.Settings.TransportPrinter != null)
            {
                TransportPrinterService = new TransportPrinterService(SettingsService.Settings.TransportPrinter,
                    SettingsService.Settings.Line,
                    LocalDbService,
                    ReportTaskService);
            }
        }

        if (SettingsService.Settings.PalletPrinter != null && SettingsService.Settings.PalletPrinter.IsUsed)
        {
            Devices.Add(PalletPrinterService.Device);
            ((PalletPrinterService)PalletPrinterService).ConnectionChanged += PrinterDeviceService_ConnectionChanged;
        }

        if (SettingsService.Settings.TransportPrinter != null && SettingsService.Settings.TransportPrinter.IsUsed)
        {
            Devices.Add(TransportPrinterService.Device);
            ((TransportPrinterService)TransportPrinterService).ConnectionChanged += PrinterDeviceService_ConnectionChanged;
        }

        ConnectDevices();
        FindedDevice();
    }

    private void StatisticCameraService_MessageReceived(Client client, DateTime datetime, string message)
    {
        Task.Run(() =>
        {
            if (ReportTaskService.CurrentReportTask == null)
                return;

            var code = ParseCodeFromMessage(message);
            if (string.IsNullOrEmpty(code))
                return;

            if (!ProcessingCodeService.IsProductCode(code))
                return;

            if (!ProcessingCodeService.IsProductCodeTheCurrentTask(code))
                return;

            ReportTaskService.Statistic.StatisticsCounter++;
            ReportTaskService.Statistic.CameraReadingResults.Insert(0, new CameraReadingResult()
            {
                StatisticCameraReadingResult = "Считано",
                ProductCamera1ReadingResult = "Ожидание",
                ProductCamera2ReadingResult = "Ожидание",
                VerificationCamera1ReadingResult = "Ожидание",
                VerificationCamera2ReadingResult = "Ожидание"
            });
        });
    }

    private void ProductCamera1Service_MessageReceived(Client client, DateTime datetime, string message)
    {
        Task.Run(() =>
        {
            ProcessFormationCameraMessage(message, "Camera1");
        });
    }

    private void ProductCamera2Service_MessageReceived(Client client, DateTime datetime, string message)
    {
        Task.Run(() =>
        {
            ProcessFormationCameraMessage(message, "Camera2");
        });
    }

    private void ProcessFormationCameraMessage(string message, string cameraName)
    {
        if (ReportTaskService.CurrentReportTask == null)
            return;

        var codes = ParseCodesFromMessage(message);
        if (codes == null || codes.Count == 0)
            return;

        var expectedCount = Convert.ToInt32(ReportTaskService.CurrentReportTask.CountProductInBox);
        if (codes.Count != expectedCount)
        {
            ReportTaskService.Statistic.CameraReadingResults.Insert(0, new CameraReadingResult()
            {
                StatisticCameraReadingResult = "Ожидание",
                ProductCamera1ReadingResult = cameraName == "Camera1" ? $"Ошибка: получено {codes.Count}, ожидалось {expectedCount}" : "Ожидание",
                ProductCamera2ReadingResult = cameraName == "Camera2" ? $"Ошибка: получено {codes.Count}, ожидалось {expectedCount}" : "Ожидание",
                VerificationCamera1ReadingResult = "Ожидание",
                VerificationCamera2ReadingResult = "Ожидание"
            });
            return;
        }

        // дубликаты внутри пакета
        if (codes.Count != codes.Distinct().Count())
        {
            ReportTaskService.Statistic.CameraReadingResults.Insert(0, new CameraReadingResult()
            {
                StatisticCameraReadingResult = "Ожидание",
                ProductCamera1ReadingResult = cameraName == "Camera1" ? "Ошибка: дубликаты в пакете" : "Ожидание",
                ProductCamera2ReadingResult = cameraName == "Camera2" ? "Ошибка: дубликаты в пакете" : "Ожидание",
                VerificationCamera1ReadingResult = "Ожидание",
                VerificationCamera2ReadingResult = "Ожидание"
            });
            return;
        }

        var validCodes = new List<string>();
        foreach (var code in codes)
        {
            if (!ProcessingCodeService.IsProductCode(code))
            {
                validCodes.Clear();
                break;
            }

            if (!ProcessingCodeService.IsProductCodeTheCurrentTask(code))
            {
                validCodes.Clear();
                break;
            }

            if (ProcessingCodeService.IsRepeatProductCode(code))
            {
                validCodes.Clear();
                break;
            }

            validCodes.Add(code);
        }

        if (validCodes.Count != expectedCount)
        {
            ReportTaskService.Statistic.CameraReadingResults.Insert(0, new CameraReadingResult()
            {
                StatisticCameraReadingResult = "Ожидание",
                ProductCamera1ReadingResult = cameraName == "Camera1" ? $"Ошибка: валидных кодов {validCodes.Count}, ожидалось {expectedCount}" : "Ожидание",
                ProductCamera2ReadingResult = cameraName == "Camera2" ? $"Ошибка: валидных кодов {validCodes.Count}, ожидалось {expectedCount}" : "Ожидание",
                VerificationCamera1ReadingResult = "Ожидание",
                VerificationCamera2ReadingResult = "Ожидание"
            });
            return;
        }

        try
        {
            var virtualBox = VirtualBoxService.CreateVirtualBox(
                ReportTaskService.CurrentReportTask.Guid,
                ReportTaskService.CurrentReportTask.Id,
                validCodes);

            ReportTaskService.Statistic.CameraReadingResults.Insert(0, new CameraReadingResult()
            {
                StatisticCameraReadingResult = "Ожидание",
                ProductCamera1ReadingResult = cameraName == "Camera1" ? "Виртуальный короб создан" : "Ожидание",
                ProductCamera2ReadingResult = cameraName == "Camera2" ? "Виртуальный короб создан" : "Ожидание",
                VerificationCamera1ReadingResult = "Ожидание",
                VerificationCamera2ReadingResult = "Ожидание"
            });
        }
        catch (Exception ex)
        {
            ReportTaskService.Statistic.CameraReadingResults.Insert(0, new CameraReadingResult()
            {
                StatisticCameraReadingResult = "Ожидание",
                ProductCamera1ReadingResult = cameraName == "Camera1" ? $"Ошибка: {ex.Message}" : "Ожидание",
                ProductCamera2ReadingResult = cameraName == "Camera2" ? $"Ошибка: {ex.Message}" : "Ожидание",
                VerificationCamera1ReadingResult = "Ожидание",
                VerificationCamera2ReadingResult = "Ожидание"
            });
        }
    }

    private void VerificationCamera1Service_MessageReceived(Client client, DateTime datetime, string message)
    {
        Task.Run(async () =>
        {
            await ProcessVerificationCameraMessage(message, "Camera1");
        });
    }

    private void VerificationCamera2Service_MessageReceived(Client client, DateTime datetime, string message)
    {
        Task.Run(async () =>
        {
            await ProcessVerificationCameraMessage(message, "Camera2");
        });
    }

    private async Task ProcessVerificationCameraMessage(string message, string cameraName)
    {
        if (ReportTaskService.CurrentReportTask == null)
            return;

        var codes = ParseCodesFromMessage(message);
        if (codes == null || codes.Count != 2)
        {
            ReportTaskService.Statistic.CameraReadingResults.Insert(0, new CameraReadingResult()
            {
                StatisticCameraReadingResult = "Ожидание",
                ProductCamera1ReadingResult = "Ожидание",
                ProductCamera2ReadingResult = "Ожидание",
                VerificationCamera1ReadingResult = cameraName == "Camera1" ? $"Ошибка: получено {codes?.Count ?? 0} кодов, ожидалось 2" : "Ожидание",
                VerificationCamera2ReadingResult = cameraName == "Camera2" ? $"Ошибка: получено {codes?.Count ?? 0} кодов, ожидалось 2" : "Ожидание"
            });
            return;
        }

        var boxLabelCode = codes[0];
        var productCode = codes[1];

        var isValid = await VirtualBoxService.VerifyBoxAsync(boxLabelCode, productCode);
        if (!isValid)
        {
            ReportTaskService.Statistic.CameraReadingResults.Insert(0, new CameraReadingResult()
            {
                StatisticCameraReadingResult = "Ожидание",
                ProductCamera1ReadingResult = "Ожидание",
                ProductCamera2ReadingResult = "Ожидание",
                VerificationCamera1ReadingResult = cameraName == "Camera1" ? "Ошибка верификации" : "Ожидание",
                VerificationCamera2ReadingResult = cameraName == "Camera2" ? "Ошибка верификации" : "Ожидание"
            });
            return;
        }

        var virtualBox = VirtualBoxService.FindVirtualBoxByLabelCode(boxLabelCode);
        if (virtualBox == null)
        {
            return;
        }

        if (virtualBox.Status == VirtualBoxStatus.Verified)
        {
            ReportTaskService.Statistic.CameraReadingResults.Insert(0, new CameraReadingResult()
            {
                StatisticCameraReadingResult = "Ожидание",
                ProductCamera1ReadingResult = "Ожидание",
                ProductCamera2ReadingResult = "Ожидание",
                VerificationCamera1ReadingResult = cameraName == "Camera1" ? "Уже верифицирован" : "Ожидание",
                VerificationCamera2ReadingResult = cameraName == "Camera2" ? "Уже верифицирован" : "Ожидание"
            });
            return;
        }

        var currentPallet = ReportTaskService.Statistic.PalletCodes.LastOrDefault();
        if (currentPallet == null || currentPallet.IsFulled)
        {
            ReportTaskService.GeneratePalletCode();
            currentPallet = ReportTaskService.Statistic.PalletCodes.LastOrDefault();
        }

        if (currentPallet == null)
        {
            return;
        }

        var box = await VirtualBoxService.ConvertToRealBoxAsync(virtualBox, currentPallet.Id);
        
        ReportTaskService.Statistic.BoxCodes.Add(box);
        ReportTaskService.Statistic.CountBoxes++;
        currentPallet.Boxes.Add(box);

        foreach (var product in box.Products)
        {
            ReportTaskService.Statistic.ProductCodes.Add(product);
            ReportTaskService.Statistic.CountProducts++;
        }

        ReportTaskService.Statistic.CountBoxInCurrentPallet = currentPallet.Boxes.Count;

        ReportTaskService.Statistic.CameraReadingResults.Insert(0, new CameraReadingResult()
        {
            StatisticCameraReadingResult = "Ожидание",
            ProductCamera1ReadingResult = "Ожидание",
            ProductCamera2ReadingResult = "Ожидание",
            VerificationCamera1ReadingResult = cameraName == "Camera1" ? "Короб верифицирован" : "Ожидание",
            VerificationCamera2ReadingResult = cameraName == "Camera2" ? "Короб верифицирован" : "Ожидание"
        });
    }

    private string ParseCodeFromMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return string.Empty;

        var code = message.Trim();
        code = code.Replace("\0", "");
        code = code.Replace("\r", "");
        code = code.Replace("\n", "");
        code = code.Replace("<GS>", "\u001d");
        code = code.Replace(",gs.", "\u001d");

        return code;
    }

    private List<string> ParseCodesFromMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return new List<string>();

        var codes = new List<string>();
        var lines = message.Split(new[] { "\r\n", "\n", "\r", " " }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var code = ParseCodeFromMessage(line);
            if (!string.IsNullOrEmpty(code))
            {
                codes.Add(code);
            }
        }

        return codes;
    }

    private void LocalDBService_ConnectionChanged(DataBaseContext db, DateTime datetime,
        DbConnectionState connectionState)
    {
        if (db.IsConnected)
        {
            FindedDevice();
        }
        else
        {
            LostedDevice();
        }
    }

    private void CameraService_ConnectionChanged(Client client, DateTime datetime,
        SocketConnectionState connectionState)
    {
        if (client.IsConnected)
        {
            FindedDevice();
        }
        else
        {
            LostedDevice();
        }
    }

    private void PrinterDeviceService_ConnectionChanged(Client client, DateTime datetime,
        SocketConnectionState connectionState)
    {
        if (client.IsConnected)
        {
            FindedDevice();
        }
        else
        {
            LostedDevice();
        }
    }
}