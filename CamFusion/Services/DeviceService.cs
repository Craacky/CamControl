using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
    }


    public override void CreateDevice()
    {
        StopDevices();
        DisconnectDevices();

        ProductMasterCameraService = null!;
        ProductSlaveCameraService = null!;
        BoxCameraService = null!;
        BoxPrinterService = null!;
        PalletPrinterService = null!;

        Devices = new ObservableCollection<Device>();

        if (Settings.LocalDb.IsUsed)
        {
            Devices.Add(LocalDbService.Device);
            LocalDbService.ConnectionChanged += LocalDBService_ConnectionChanged;
        }

        if (SettingsService.Settings.ProductCameraMaster != null)
        {
            if (SettingsService.Settings.Line != null)
                ProductMasterCameraService = new ProductMasterCameraService(
                    SettingsService.Settings.ProductCameraMaster,
                    SettingsService.Settings.Line);
            if (SettingsService.Settings.ProductCameraMaster.IsUsed)
            {
                Devices.Add(ProductMasterCameraService.Device);
                ((ProductMasterCameraService)ProductMasterCameraService).ConnectionChanged +=
                    CameraService_ConnectionChanged;
                ((ProductMasterCameraService)ProductMasterCameraService).MessageReceived +=
                    ProductMasterCameraService_MessageReceived;
            }
        }

        if (SettingsService.Settings.Line != null)
        {
            if (SettingsService.Settings.ProductCameraSlave != null)
            {
                ProductSlaveCameraService = new ProductSlaveCameraService(SettingsService.Settings.ProductCameraSlave,
                    SettingsService.Settings.Line);
                if (SettingsService.Settings.ProductCameraSlave.IsUsed)
                {
                    Devices.Add(ProductSlaveCameraService.Device);
                    ((ProductSlaveCameraService)ProductSlaveCameraService).ConnectionChanged +=
                        CameraService_ConnectionChanged;
                    ((ProductSlaveCameraService)ProductSlaveCameraService).MessageReceived +=
                        ProductSlaveCameraService_MessageReceived;
                }
            }

            if (SettingsService.Settings.BoxCamera != null)
            {
                BoxCameraService = new BoxCameraService(SettingsService.Settings.BoxCamera,
                    SettingsService.Settings.Line);
                if (SettingsService.Settings.BoxCamera.IsUsed)
                {
                    Devices.Add(BoxCameraService.Device);
                    ((BoxCameraService)BoxCameraService).ConnectionChanged += CameraService_ConnectionChanged;
                    ((BoxCameraService)BoxCameraService).MessageReceived += BoxCameraService_MessageReceived;
                }
            }

            if (SettingsService.Settings.PalletPrinter != null)
            {
                BoxPrinterService = new BoxPrinterService(SettingsService.Settings.PalletPrinter,
                    SettingsService.Settings.Line,
                    LocalDbService,
                    ReportTaskService);
                if (SettingsService.Settings.BoxPrinter != null && SettingsService.Settings.BoxPrinter.IsUsed)
                {
                    Devices.Add(BoxPrinterService.Device);
                    ((BoxPrinterService)BoxPrinterService).ConnectionChanged += PrinterDeviceService_ConnectionChanged;
                }

                PalletPrinterService = new PalletPrinterService(SettingsService.Settings.PalletPrinter,
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

        ConnectDevices();
        FindedDevice();
    }

    private void BoxCameraService_MessageReceived(Client client, DateTime datetime, string message)
    {
    }

    private void ProductSlaveCameraService_MessageReceived(Client client, DateTime datetime, string message)
    {
    }

    private void ProductMasterCameraService_MessageReceived(Client client, DateTime datetime, string message)
    {
        Task.Run(() =>
        {
            string startPattern = "<start>";
            string stopPattern = "<stop>";
            string failPattern = "fail";
            string nextPattern = "<next>";

            bool isCorrectMessage = message.Contains(startPattern) && message.Contains(stopPattern);
            if (isCorrectMessage)
            {
                string[] batches = message.Split(startPattern, StringSplitOptions.RemoveEmptyEntries);
                foreach (string batch in batches)
                {
                    if (batch.Contains(stopPattern))
                    {
                        string part = batch.Split(stopPattern)[0];

                        bool isFaiLMessage = part.Contains(failPattern);
                        if (isFaiLMessage)
                        {
                            string[] decodeResults = part.Split(nextPattern);

                            if (Convert.ToInt32(decodeResults[1]) == 1)
                            {
                                ReportTaskService.Statistic.CameraReadingResults.Insert(0, new CameraReadingResult());

                                ReportTaskService.Statistic.CameraReadingResults[0].BoxCameraReadingResult = "Считано";

                                ReportTaskService.Statistic.CameraReadingResults[0].ProductCameraMasterReadingResult =
                                    Convert.ToInt32(decodeResults[2]) ==
                                    Convert.ToInt32(ReportTaskService.CurrentReportTask.CountProductInBox) / 2
                                        ? "Считано"
                                        : "Несчитано";
                                ReportTaskService.Statistic.CameraReadingResults[0].ProductCameraSlaveReadingResult =
                                    Convert.ToInt32(decodeResults[3].Split(stopPattern)[0]) ==
                                    Convert.ToInt32(ReportTaskService.CurrentReportTask.CountProductInBox) / 2
                                        ? "Считано"
                                        : "Несчитано";
                            }
                            else
                            {
                                ReportTaskService.Statistic.CameraReadingResults.Insert(0,
                                    new CameraReadingResult() { BoxCameraReadingResult = "Несчитано" });
                            }

                            return;
                        }


                        string[] markingCodes = part.Split(nextPattern);

                        if (ReportTaskService.Statistic.PalletCodes.Count == 0 ||
                            ReportTaskService.Statistic.PalletCodes[^1].IsFulled)
                        {
                            ReportTaskService.GeneratePalletCode();
                        }

                        if (SettingsService.Settings.Line != null)
                        {
                            Box? newBox = new Box
                            {
                                LineId = SettingsService.Settings.Line.LineId,
                                MarkingCode = markingCodes[0],
                                ReportTaskGuid = ReportTaskService.CurrentReportTask.Guid,
                                PalletId = ReportTaskService.Statistic.PalletCodes[^1].Id
                            };


                            bool isBoxCodeTheCurrentTask =
                                ProcessingCodeService.IsBoxCodeTheCurrentTask(markingCodes[0]);
                            if (!isBoxCodeTheCurrentTask)
                            {
                                LostedDevice();
                                StopDevices();

                                MessageBox.Show(
                                    "Код короба не соответствует коду короба текущего задания.\n" +
                                    $"{markingCodes[0]}\n" +
                                    "Короб не будет добавлен.\n" +
                                    "Уберите с паллеты последний прошедший короб!!!", "Ошибка", MessageBoxButton.OK);

                                ReportTaskService.Statistic.CameraReadingResults.Insert(0,
                                    new CameraReadingResult()
                                        { BoxCameraReadingResult = $"Несоответсвие кода {markingCodes[0]}" });

                                FindedDevice();
                                StartDevices();

                                return;
                            }

                            bool isRepeateBoxCode = ProcessingCodeService.IsRepeatBoxCode(markingCodes[0]);
                            if (isRepeateBoxCode)
                            {
                                LostedDevice();
                                StopDevices();

                                MessageBox.Show("Повтор кода короба.\n" +
                                                $"{markingCodes[0]}\n" +
                                                "Короб не будет добавлен.\n" +
                                                "Уберите с паллеты последний прошедший короб!!!",
                                    "Ошибка", MessageBoxButton.OK);


                                ReportTaskService.Statistic.CameraReadingResults.Insert(0,
                                    new CameraReadingResult()
                                        { BoxCameraReadingResult = $"Повтор кода {markingCodes[0]}" });

                                FindedDevice();
                                StartDevices();

                                return;
                            }

                            List<Product> products = new List<Product>();

                            for (int i = 1; i < markingCodes.Length; i++)
                            {
                                Product newProduct = new()
                                {
                                    MarkingCode = markingCodes[i],
                                    BoxId = newBox.Id,
                                    LineId = SettingsService.Settings.Line.LineId,
                                    ReportTaskGuid = ReportTaskService.CurrentReportTask.Guid
                                };

                                bool isProductCodeTheCurrentTask =
                                    ProcessingCodeService.IsProductCodeTheCurrentTask(markingCodes[i]);
                                if (!isProductCodeTheCurrentTask)
                                {
                                    LostedDevice();
                                    StopDevices();

                                    MessageBox.Show(
                                        "Код продукта не соответствует коду продукта текущего задания.\n" +
                                        $"{markingCodes[i]}\n" +
                                        "Короб не будет добавлен.\n" +
                                        "Уберите с паллеты последний прошедший короб!!!", "Ошибка",
                                        MessageBoxButton.OK);

                                    ReportTaskService.Statistic.CameraReadingResults.Insert(0,
                                        new CameraReadingResult()
                                        {
                                            BoxCameraReadingResult = $"Считано {newBox.MarkingCode}",
                                            ProductCameraMasterReadingResult = $"Несоответствие кода {markingCodes[i]}"
                                        });

                                    StartDevices();
                                    FindedDevice();

                                    return;
                                }

                                bool isRepeatProductCode = ProcessingCodeService.IsRepeatProductCode(markingCodes[i]);
                                bool isRepeatInCurrentBox = products.Any(p =>
                                    p.MarkingCode?.Replace("\u001d", "") == markingCodes[i].Replace("\u001d", ""));
                                if (isRepeatProductCode || isRepeatInCurrentBox)
                                {
                                    LostedDevice();
                                    StopDevices();

                                    MessageBox.Show("Повтор кода продукта.\n" +
                                                    $"{markingCodes[i]}\n" +
                                                    "Короб не будет добавлен.\n" +
                                                    "Уберите с паллеты последний прошедший короб!!!",
                                        "Ошибка", MessageBoxButton.OK);

                                    ReportTaskService.Statistic.CameraReadingResults.Insert(0,
                                        new CameraReadingResult()
                                        {
                                            BoxCameraReadingResult = $"Считано {newBox.MarkingCode}",
                                            ProductCameraMasterReadingResult = $"Повтор кода {markingCodes[i]}"
                                        });

                                    StartDevices();
                                    FindedDevice();

                                    return;
                                }

                                products.Add(newProduct);
                            }

                            newBox = LocalDbService.BoxDataService.Create(newBox);

                            for (int i = 0; i < products.Count; i++)
                            {
                                products[i].BoxId = newBox?.Id;
                                var newProduct = LocalDbService.ProductDataService.Create(products[i]);
                                ReportTaskService.Statistic.ProductCodes.Add(newProduct!);
                            }

                            ReportTaskService.Statistic.CountProducts += products.Count;
                            if (newBox != null)
                            {
                                ReportTaskService.Statistic.BoxCodes.Add(newBox);
                                ReportTaskService.Statistic.CountBoxes++;
                                ReportTaskService.Statistic.CountBoxInCurrentPallet++;


                                ReportTaskService.Statistic.CameraReadingResults.Insert(0,
                                    new CameraReadingResult()
                                    {
                                        BoxCameraReadingResult = $"Считано {newBox.MarkingCode}",
                                        ProductCameraMasterReadingResult = "Cчитано",
                                        ProductCameraSlaveReadingResult = "Cчитано"
                                    });
                            }
                        }

                        if (ReportTaskService.Statistic.CountBoxInCurrentPallet % countBoxInPallet == 0)
                        {
                            ReportTaskService.ClosePallet();
                            PalletPrinterService?.PrintCode();
                        }
                    }
                }
            }
        });
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