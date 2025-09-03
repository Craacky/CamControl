using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Windows;
using CC.Core.Devices.Impl;
using CC.Core.Models;
using CC.Core.Models.Base;
using CC.Data.Entities.Codes;
using CC.Data.Entities.Settings;
using CC.Data.Entities.Tasks;
using JsonParser.Services;
using Attribute = CC.Data.Entities.Tasks.Attribute;

namespace CC.Core.Services.Impl;

public class ReportTaskService : ObservableObject
{
    private readonly string defaultReportTasksDirectory = Environment.CurrentDirectory + "\\Reports\\";


    private List<ReportTask> reportTasks;

    public List<ReportTask> ReportTasks
    {
        get => reportTasks;
        set
        {
            reportTasks = value;
            OnPropertyChanged(nameof(ReportTasks));
        }
    }

    private List<ReportTask> allReportTasks;

    public List<ReportTask> AllReportTasks
    {
        get => allReportTasks;
        set
        {
            allReportTasks = value;
            OnPropertyChanged(nameof(AllReportTasks));
        }
    }

    private ReportTask currentReportTask;

    public ReportTask CurrentReportTask
    {
        get => currentReportTask;
        set
        {
            currentReportTask = value;
            OnPropertyChanged(nameof(CurrentReportTask));

            if (currentReportTask != null)
            {
                Statistic = new Statistic();
                LoadCodesReportTask(currentReportTask);
                Statistic.CountBoxInCurrentPallet = 
                    Statistic.PalletCodes.Count > 0 ? Statistic.PalletCodes[^1].Boxes.Count : 0;
                Statistic.CountProducts = Statistic.ProductCodes.Count;
                Statistic.CountBoxes = Statistic.BoxCodes.Count;
            }
        }
    }

    public Statistic Statistic { get; set; }


    public LineSettings LineSettings { get; set; }
    public LocalDb LocalDb { get; set; }
    public ErrorsService ErrorsService { get; set; }


    public ReportTaskService(LocalDb localDbService,
        LineSettings lineSettings,
        ErrorsService errorsService)
    {
        LocalDb = localDbService;
        LineSettings = lineSettings;
        ErrorsService = errorsService;

        if (!Directory.Exists(defaultReportTasksDirectory))
        {
            Directory.CreateDirectory(defaultReportTasksDirectory);
        }

        Statistic = new Statistic();
        ReportTasks = new List<ReportTask>();

        LoadAllReportTaskFromDB();
    }


    private void LoadAllReportTaskFromDB()
    {
        Expression<Func<ReportTask, Nomenclature>> includePredicate = p => p.Nomenclature!;
        Expression<Func<Nomenclature, IEnumerable<Attribute>>> thenIncludePredicate = b => b.Attributes;
        var dbAllReportTasks = LocalDb.ReportTaskDataService.GetAllWithInclude(includePredicate)!;
        var dbReportTasks = LocalDb.ReportTaskDataService.GetAllWithInclude(
            rt => rt.Status != "Сохранено",
            includePredicate,
            thenIncludePredicate)!;
        foreach (var dbReportTask in dbAllReportTasks)
        {
            if (DateTime.Now.Subtract(dbReportTask.DateTime).Days <= LineSettings.JobStoragePeriodInDays) continue;
            LocalDb.ReportTaskDataService.Delete(dbReportTask.Id);
        }

        foreach (ReportTask dbReportTask in dbReportTasks)
        {
            if (DateTime.Now.Subtract(dbReportTask.DateTime).Days > LineSettings.JobStoragePeriodInDays)
            {
                LocalDb.ReportTaskDataService.Delete(dbReportTask.Id);
                continue;
            }

            if (DateTime.Now.Subtract(dbReportTask.DateTime).Days > 1)
            {
                dbReportTask.Status = "Устаревшее";
                LocalDb.ReportTaskDataService.Update(dbReportTask.Id, dbReportTask);
            }

            if (dbReportTask.Status == "Запущено")
            {
                dbReportTask.Status = "Остановлено";
                LocalDb.ReportTaskDataService.Update(dbReportTask.Id, dbReportTask);
            }

            if (DateTime.Now.Subtract(dbReportTask.DateTime).Days > 2)
            {
                continue;
            }

            ReportTasks.Insert(0, dbReportTask);
        }

        ReportTasks = new List<ReportTask>(ReportTasks);
    }

    private void LoadCodesReportTask(ReportTask reportTask)
    {
        Expression<Func<Pallet, IEnumerable<Box>>> includePredicate = p => p.Boxes;
        Expression<Func<Box, IEnumerable<Product>>> thenIncludePredicate = b => b.Products;
        List<Pallet> pallets =
            LocalDb.PalletDataService.GetAll(p => p.ReportTaskGuid == currentReportTask.Guid).ToList();

        foreach (Pallet pallet in pallets)
        {
            pallet.Boxes = LocalDb.BoxDataService.GetAllWithInclude(b => b.PalletId == pallet.Id, thenIncludePredicate)!
                .ToList();
            Statistic.PalletCodes.Add(pallet);

            foreach (Box box in pallet.Boxes)
            {
                Statistic.BoxCodes.Add(box);
                foreach (Product product in box.Products)
                {
                    Statistic.ProductCodes.Add(product);
                }
            }
        }
    }


    public void GeneratePalletCode()
    {
        string patternPalletCode = $"01{CurrentReportTask.Nomenclature.Gtin}" +
                                   $"11{CurrentReportTask.ManufactureDate.ToString("yyMMdd")}" +
                                   $"17{CurrentReportTask.ExpiryDate.ToString("yyMMdd")}" +
                                   $"10{CurrentReportTask.LotNumber}" +
                                   $"\u001d21";

        string currentPalletCode = patternPalletCode + $"{Statistic.PalletCodes.Count + 1}" +
                                   $"\u001d37{Convert.ToInt32(CurrentReportTask.CountBoxInPallet) * Convert.ToInt32(CurrentReportTask.CountProductInBox)}";
        Pallet newPallet = new Pallet
        {
            MarkingCode = currentPalletCode,
            LineId = LineSettings.LineId,
            ReportTaskGuid = CurrentReportTask.Guid,
            ReportTaskId = CurrentReportTask.Id
        };

        newPallet = LocalDb.PalletDataService.Create(newPallet);

        Statistic.PalletCodes.Add(newPallet);
        Statistic.PalletCodes = new List<Pallet>(Statistic.PalletCodes);
        Statistic.CountBoxInCurrentPallet = 0;
    }

    public void ClosePallet()
    {
        string patternPalletCode = $"01{CurrentReportTask.Nomenclature.Gtin}" +
                                   $"11{CurrentReportTask.ManufactureDate.ToString("yyMMdd")}" +
                                   $"17{CurrentReportTask.ExpiryDate.ToString("yyMMdd")}" +
                                   $"10{CurrentReportTask.LotNumber}" +
                                   $"\u001d21";
        if (Statistic.PalletCodes.Count > 0)
        {
            Pallet pallet = Statistic.PalletCodes[^1];
            Statistic.PalletCodes.Remove(pallet);

            string currentPalletCode = patternPalletCode + $"{Statistic.PalletCodes.Count + 1}" +
                                       $"\u001d37{Convert.ToInt32(Statistic.CountBoxInCurrentPallet) * Convert.ToInt32(CurrentReportTask.CountProductInBox)}";
            pallet.MarkingCode = currentPalletCode;
            pallet.IsFulled = true;

            List<Box> boxes = pallet.Boxes;
            pallet.Boxes = null;

            pallet = LocalDb.PalletDataService.Update(pallet.Id, pallet);
            pallet.Boxes = boxes;

            Statistic.PalletCodes.Add(pallet);
        }
    }

    public void UpdateReportTask(ReportTask reportTask)
    {
        ReportTasks.Remove(reportTask);
        LocalDb.ReportTaskDataService.Update(reportTask.Id, reportTask);
        ReportTasks.Insert(0, reportTask);
        ReportTasks = new List<ReportTask>(ReportTasks);
    }

    public bool SaveReportTask(ReportTask currentReportTask)
    {
        JsonParser.Objects.Write.ReportTask reportTask = new()
        {
            TaskMarks = new List<JsonParser.Objects.Write.TaskMarks>()
        };

        JsonParser.Objects.Write.TaskMarks taskMarks = new()
        {
            ExporterCode = currentReportTask.Nomenclature.ExporterCode,
            Code = 0,
            ClaimNumber = currentReportTask.ManufactureDate.ToString("dd.MM.yyyy") + "-" +
                          String.Format("{0:0000}", currentReportTask.LotNumber),
            PartnerCode = null,
            MarkTemplateCode = 0,
            LineCode = LineSettings.LineId,
            StartTime = currentReportTask.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
            CloseTime = currentReportTask.StopTime.ToString("yyyy-MM-ddTHH:mm:ss"),
            Attributes = new List<JsonParser.Objects.Write.Attributes>()
            {
                new JsonParser.Objects.Write.Attributes()
                {
                    IsMarketReport = false,
                    Code = 1,
                    Value = String.Format("{0:0000}", currentReportTask.LotNumber)
                },
                new JsonParser.Objects.Write.Attributes()
                {
                    IsMarketReport = false,
                    Code = 2,
                    Value = currentReportTask.ManufactureDate.ToString("yyyy-MM-dd")
                },
                new JsonParser.Objects.Write.Attributes()
                {
                    IsMarketReport = false,
                    Code = 3,
                    Value = currentReportTask.ExpiryDate.ToString("yyyy-MM-dd")
                },
                new JsonParser.Objects.Write.Attributes()
                {
                    IsMarketReport = false,
                    Code = 4,
                    Value = currentReportTask.Nomenclature.Gtin
                },
                new JsonParser.Objects.Write.Attributes()
                {
                    IsMarketReport = false,
                    Code = 5,
                    Value = currentReportTask.Nomenclature.Attributes[11].Value
                },
            },
            BarCodes = new List<JsonParser.Objects.Write.BarCodes>()
        };

        Expression<Func<Pallet, IEnumerable<Box>>> includePredicate = p => p.Boxes;
        Expression<Func<Box, IEnumerable<Product>>> thenIncludePredicate = b => b.Products;
        var pallets =
            LocalDb.PalletDataService.GetAll(p => p.ReportTaskGuid == currentReportTask.Guid).ToList();
        List<JsonParser.Objects.Write.BarCodes> barcodes = new();

        var countPallet = 0;
        var countBox = 0;
        var countProduct = 0;

        List<string> productCodes = new();
        List<string> boxCodes = new();
        ErrorsService.Errors = new List<Error>();
        foreach (var pallet in pallets)
        {
            pallet.Boxes = LocalDb.BoxDataService.GetAllWithInclude(b => b.PalletId == pallet.Id, thenIncludePredicate)
                .ToList();
            if (pallet.Boxes == null || pallet.Boxes.Count == 0)
            {
                continue;
            }

            JsonParser.Objects.Write.BarCodes palletBarcode = new()
            {
                BarCode = pallet.MarkingCode,
                Level = 2,
                NumberInTask = ++countPallet,
                Weight = null,
                ChildBarCodes = new List<JsonParser.Objects.Write.BarCodes>()
            };

            foreach (var box in pallet.Boxes)
            {
                if (boxCodes.Contains(box.MarkingCode))
                {
                    Error error = new Error { TypeError = "Повтор кода короба" };
                    error.BoxCodes.Add(box.MarkingCode);
                    var boxes = LocalDb.BoxDataService.GetAllWithInclude(b => b.MarkingCode == box.MarkingCode,
                        p => p.Pallet);
                    foreach (Box repeatBox in boxes)
                    {
                        error.PalletCodes.Add(repeatBox.Pallet.MarkingCode);
                    }

                    ErrorsService.Errors.Add(error);
                    continue;
                }
                else
                {
                    if (box.Products.Count == 12)
                    {
                        JsonParser.Objects.Write.BarCodes boxBarcode = new()
                        {
                            BarCode = box.MarkingCode,
                            Level = 1,
                            NumberInTask = ++countBox,
                            Weight = null,
                            ChildBarCodes = new List<JsonParser.Objects.Write.BarCodes>()
                        };


                        foreach (var product in box.Products)
                        {
                            if (productCodes.Contains(product.MarkingCode))
                            {
                                Error error = new Error { TypeError = "Повтор кода продукта" };
                                error.ProductCode = product.MarkingCode;
                                var products = LocalDb.ProductDataService.GetAllWithInclude(
                                    b => b.MarkingCode == product.MarkingCode, p => p.Box, p => p.Pallet);
                                foreach (Product repeatProduct in products)
                                {
                                    error.BoxCodes.Add(repeatProduct.Box.MarkingCode);
                                    error.PalletCodes.Add(repeatProduct.Box.Pallet.MarkingCode);
                                }

                                ErrorsService.Errors.Add(error);

                                boxBarcode = null;
                                break;
                            }

                            JsonParser.Objects.Write.BarCodes productBarcode = new()
                            {
                                BarCode = product.MarkingCode,
                                Level = 0,
                                NumberInTask = ++countProduct,
                                Weight = null,
                                ChildBarCodes = new List<JsonParser.Objects.Write.BarCodes>()
                            };

                            productCodes.Add(product.MarkingCode);
                            boxBarcode.ChildBarCodes.Add(productBarcode);
                        }


                        if (boxBarcode != null && boxBarcode.ChildBarCodes != null &&
                            boxBarcode.ChildBarCodes.Count != 0)
                        {
                            boxCodes.Add(box.MarkingCode);
                            palletBarcode.ChildBarCodes.Add(boxBarcode);
                        }
                        else
                        {
                            if (boxBarcode != null &&
                                (boxBarcode.ChildBarCodes == null || boxBarcode.ChildBarCodes.Count == 0))
                            {
                                Console.WriteLine("Пустой короб");
                            }
                        }
                    }
                    else
                    {
                        Error error = new Error { TypeError = "Неполный короб" };
                        error.BoxCodes.Add(box.MarkingCode);
                        var boxes = LocalDb.BoxDataService.GetAllWithInclude(b => b.MarkingCode == box.MarkingCode,
                            p => p.Pallet);
                        foreach (Box repeatBox in boxes)
                        {
                            error.PalletCodes.Add(repeatBox.Pallet.MarkingCode);
                        }

                        ErrorsService.Errors.Add(error);

                        foreach (var product in box.Products)
                        {
                            if (productCodes.Contains(product.MarkingCode))
                            {
                                error = new Error { TypeError = "Повтор кода продукта" };
                                error.ProductCode = product.MarkingCode;
                                var products = LocalDb.ProductDataService.GetAllWithInclude(
                                    b => b.MarkingCode == product.MarkingCode, p => p.Box, p => p.Pallet);
                                foreach (Product repeatProduct in products)
                                {
                                    error.BoxCodes.Add(repeatProduct.Box.MarkingCode);
                                    error.PalletCodes.Add(repeatProduct.Box.Pallet.MarkingCode);
                                }

                                ErrorsService.Errors.Add(error);

                                Console.WriteLine("Повтор продукта:" + product.MarkingCode);
                                break;
                            }
                        }
                    }
                }
            }

            taskMarks.BarCodes.Add(palletBarcode);
        }

        if (countBox == 0 || countPallet == 0 || countProduct == 0)
        {
            MessageBox.Show($"В ходе работы задания произогла ошибка.\nОтчёт пустой.\nОтчёт не будет выгружен.");
            return false;
        }

        reportTask.TaskMarks.Add(taskMarks);


        if (ErrorsService.Errors.Count > 0)
        {
            ErrorsService.Errors = new List<Error>(ErrorsService.Errors);
            var result = MessageBox.Show("Отчёт содержит ошибки. Сохранить отчёт?", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return false;
            }
        }


        string jsonFileName = currentReportTask.Nomenclature.Gtin + "_" +
                              currentReportTask.ManufactureDate.ToString("dd.MM.yyyy") + "_" +
                              string.Format("{0:0000}", currentReportTask.LotNumber) + "_" + currentReportTask.Guid +
                              "_" + LineSettings.LineId + ".json";
        NetworkCredential nc = new(@"BELMILK\mark_brgtu", "869651");
        CredentialCache credentialCache = new();
        credentialCache.Add(new Uri(@"\\172.25.0.30\apk\In\BrGTU"), "Basic", nc);

        try
        {
            JsonService.Write(reportTask, LineSettings.PathSaveReportTaskFiles + jsonFileName);
            JsonService.Write(null, LineSettings.PathSaveReportTaskFiles + jsonFileName.Replace(".json", ".in"));
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не удалось сохранить отчёт в папку {LineSettings.PathSaveReportTaskFiles + jsonFileName}");
        }

        if (File.Exists(LineSettings.PathSaveReportTaskFiles + jsonFileName))
        {
            currentReportTask.Status = "Сохранено";

            LocalDb.ReportTaskDataService.Update(currentReportTask.Id, currentReportTask);
            MessageBox.Show("Отчёт сохранён");
        }

        JsonService.Write(reportTask, defaultReportTasksDirectory + "\\" + jsonFileName);
        JsonService.Write(null, defaultReportTasksDirectory + "\\" + jsonFileName.Replace(".json", ".in"));

        return true;
    }

    public bool CreateReportTask(ReportTask reportTask)
    {
        if (ReportTasks.Any(rt => rt.Nomenclature.Gtin == reportTask.Nomenclature.Gtin &&
                                  rt.LotNumber == reportTask.LotNumber &&
                                  rt.CountProductInBox == reportTask.CountProductInBox &&
                                  rt.CountBoxInPallet == reportTask.CountBoxInPallet &&
                                  rt.ManufactureDate == reportTask.ManufactureDate &&
                                  rt.ExpiryDate == reportTask.ExpiryDate &&
                                  rt.IsUsedCap == reportTask.IsUsedCap))
        {
            MessageBox.Show("Данное задание уже существует");
            return false;
        }

        Nomenclature nomenclature = reportTask.Nomenclature;
        reportTask.Nomenclature = null;
        reportTask.LineId = LineSettings.LineId;
        reportTask = LocalDb.ReportTaskDataService.Create(reportTask);

        if (reportTask != null)
        {
            reportTask.Nomenclature = nomenclature;

            ReportTasks.Insert(0, reportTask);
            ReportTasks = new List<ReportTask>(ReportTasks);
        }

        return true;
    }

    public void DeleteReportTask(ReportTask reportTask)
    {
        LocalDb.ReportTaskDataService.Delete(reportTask.Id);

        ReportTasks.Remove(reportTask);
        ReportTasks = new List<ReportTask>(ReportTasks);
    }

    public void AddBoxesToPreviousPallet()
    {
        Pallet pallet = Statistic.PalletCodes[^1];
        if (Statistic.PalletCodes.Count > 1)
        {
            Pallet priviousPallet = Statistic.PalletCodes[^2];

            foreach (var box in pallet.Boxes)
            {
                Statistic.BoxCodes.Remove(box);
                box.PalletId = priviousPallet.Id;
                Box newBox = LocalDb.BoxDataService.Update(box.Id, box);
                Statistic.BoxCodes.Add(newBox);
                Statistic.PalletCodes[^2].Boxes.Add(newBox);
            }

            Statistic.CountBoxInCurrentPallet = priviousPallet.Boxes.Count();

            Statistic.PalletCodes.Remove(pallet);
            Statistic.PalletCodes = new List<Pallet>(Statistic.PalletCodes);

            LocalDb.PalletDataService.Delete(pallet.Id);
            MessageBox.Show("Продукт добавлен к предыдущей паллете");
            ClosePallet();
        }
        else
        {
            MessageBox.Show("Не существует предыдущей паллеты", "Ошибка");
        }
    }
}