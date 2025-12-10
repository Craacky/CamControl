using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CC.Core.Devices.Impl;
using CC.Core.Services;
using CC.Data.Entities.Codes;

namespace CC.Core.Services.Impl;

public class VirtualBoxService : IVirtualBoxService
{
    private readonly ConcurrentDictionary<string, VirtualBox> _activeVirtualBoxes;
    private readonly LocalDb _localDb;
    private readonly ReportTaskService _reportTaskService;
    private readonly ProcessingCodeService _processingCodeService;
    private int _boxCounter = 0;
    private CancellationTokenSource? _timeoutCts;
    private TimeSpan _timeout = TimeSpan.FromMinutes(10);
    private TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public VirtualBoxService(LocalDb localDb, ReportTaskService reportTaskService, ProcessingCodeService processingCodeService)
    {
        _localDb = localDb;
        _reportTaskService = reportTaskService;
        _processingCodeService = processingCodeService;
        _activeVirtualBoxes = new ConcurrentDictionary<string, VirtualBox>();
    }

    public VirtualBox CreateVirtualBox(Guid taskGuid, int taskId, List<string> productCodes)
    {
        var virtualBox = new VirtualBox
        {
            ReportTaskGuid = taskGuid,
            ReportTaskId = taskId,
            Status = VirtualBoxStatus.Created,
            IsInMemory = true,
            ProductCodesJson = JsonSerializer.Serialize(productCodes)
        };

        _boxCounter++;
        virtualBox.BoxLabelCode = GenerateBoxLabelCode(taskGuid, _boxCounter);
        virtualBox.Status = VirtualBoxStatus.LabelGenerated;

        _activeVirtualBoxes.TryAdd(virtualBox.BoxLabelCode!, virtualBox);

        _ = SaveVirtualBoxAsync(virtualBox);

        return virtualBox;
    }

    public string GenerateBoxLabelCode(Guid taskGuid, int boxNumber)
    {
        var currentTask = _reportTaskService.CurrentReportTask;
        if (currentTask == null || currentTask.Nomenclature == null)
            throw new InvalidOperationException("Current report task is not set");

        var boxGtin = currentTask.Nomenclature.Attributes.FirstOrDefault(c => c.Code == 12)?.Value;
        if (string.IsNullOrEmpty(boxGtin))
            throw new InvalidOperationException("Box GTIN (attr code 12) not found in nomenclature");

        string patternBoxCode = $"01{boxGtin}" +
                               $"11{currentTask.ManufactureDate:yyMMdd}" +
                               $"17{currentTask.ExpiryDate:yyMMdd}" +
                               $"10{currentTask.LotNumber}" +
                               $"\u001d21{boxNumber}";

        return patternBoxCode;
    }

    public async Task<VirtualBox> SaveVirtualBoxAsync(VirtualBox virtualBox)
    {
        if (virtualBox.Id == 0)
        {
            var saved = await _localDb.VirtualBoxDataService.CreateAsync(virtualBox);
            return saved ?? virtualBox;
        }
        else
        {
            var updated = await _localDb.VirtualBoxDataService.UpdateAsync(virtualBox.Id, virtualBox);
            return updated ?? virtualBox;
        }
    }

    public VirtualBox? FindVirtualBoxByLabelCode(string labelCode)
    {
        var normalizedLabelCode = labelCode.Replace("\u001d", "");
        
        if (_activeVirtualBoxes.TryGetValue(normalizedLabelCode, out var virtualBox))
        {
            return virtualBox;
        }

        var fromDb = _localDb.VirtualBoxDataService.Get(vb => 
            vb.BoxLabelCode != null && vb.BoxLabelCode.Replace("\u001d", "") == normalizedLabelCode);
        
        return fromDb;
    }

    public VirtualBox? FindVirtualBoxByProductCode(string productCode)
    {
        var normalizedProductCode = productCode.Replace("\u001d", "");

        foreach (var kvp in _activeVirtualBoxes)
        {
            var codes = JsonSerializer.Deserialize<List<string>>(kvp.Value.ProductCodesJson ?? "[]");
            if (codes != null && codes.Any(c => c.Replace("\u001d", "") == normalizedProductCode))
            {
                return kvp.Value;
            }
        }

        var allVirtualBoxes = _localDb.VirtualBoxDataService.GetAll(vb => vb.ReportTaskGuid == _reportTaskService.CurrentReportTask.Guid);
        foreach (var vb in allVirtualBoxes)
        {
            var codes = JsonSerializer.Deserialize<List<string>>(vb.ProductCodesJson ?? "[]");
            if (codes != null && codes.Any(c => c.Replace("\u001d", "") == normalizedProductCode))
            {
                return vb;
            }
        }

        return null;
    }

    public async Task<bool> VerifyBoxAsync(string boxLabelCode, string productCode)
    {
        var normalizedBoxCode = boxLabelCode.Replace("\u001d", "");
        var normalizedProductCode = productCode.Replace("\u001d", "");

        if (!_processingCodeService.IsBoxCode(boxLabelCode))
        {
            return false;
        }

        if (!_processingCodeService.IsBoxCodeTheCurrentTask(boxLabelCode))
        {
            return false;
        }

        var virtualBox = FindVirtualBoxByLabelCode(boxLabelCode);
        if (virtualBox == null)
        {
            return false;
        }

        if (virtualBox.Status == VirtualBoxStatus.Verified)
        {
            return false;
        }

        if (virtualBox.Status != VirtualBoxStatus.LabelGenerated && virtualBox.Status != VirtualBoxStatus.Expired)
        {
            return false;
        }

        var codes = JsonSerializer.Deserialize<List<string>>(virtualBox.ProductCodesJson ?? "[]");
        if (codes == null || !codes.Any(c => c.Replace("\u001d", "") == normalizedProductCode))
        {
            return false;
        }

        return true;
    }

    public async Task<Box> ConvertToRealBoxAsync(VirtualBox virtualBox, int palletId)
    {
        var codes = JsonSerializer.Deserialize<List<string>>(virtualBox.ProductCodesJson ?? "[]");
        if (codes == null)
        {
            throw new InvalidOperationException("Product codes are missing");
        }

        if (_processingCodeService.IsRepeatBoxCode(virtualBox.BoxLabelCode))
        {
            throw new InvalidOperationException("Box label already exists");
        }

        var box = new Box
        {
            MarkingCode = virtualBox.BoxLabelCode,
            ReportTaskGuid = virtualBox.ReportTaskGuid,
            LineId = _reportTaskService.CurrentReportTask.LineId,
            PalletId = palletId
        };

        box = _localDb.BoxDataService.Create(box);

        foreach (var code in codes)
        {
            var product = new Product
            {
                MarkingCode = code,
                ReportTaskGuid = virtualBox.ReportTaskGuid,
                LineId = _reportTaskService.CurrentReportTask.LineId,
                BoxId = box.Id
            };

            _localDb.ProductDataService.Create(product);
            box.Products.Add(product);
        }

        virtualBox.Status = VirtualBoxStatus.Verified;
        virtualBox.VerifiedAt = DateTime.Now;
        virtualBox.IsInMemory = false;

        RemoveFromMemory(virtualBox);
        await SaveVirtualBoxAsync(virtualBox);

        return box;
    }

    public List<VirtualBox> GetActiveVirtualBoxes()
    {
        return _activeVirtualBoxes.Values.ToList();
    }

    public void RemoveFromMemory(VirtualBox virtualBox)
    {
        if (virtualBox.BoxLabelCode != null)
        {
            _activeVirtualBoxes.TryRemove(virtualBox.BoxLabelCode, out _);
        }
    }

    public void StartTimeoutWatcher(TimeSpan? timeout = null, TimeSpan? checkInterval = null)
    {
        _timeout = timeout ?? _timeout;
        _checkInterval = checkInterval ?? _checkInterval;

        _timeoutCts?.Cancel();
        _timeoutCts = new CancellationTokenSource();
        _ = WatchTimeoutAsync(_timeoutCts.Token);
    }

    public void StopTimeoutWatcher()
    {
        _timeoutCts?.Cancel();
    }

    private async Task WatchTimeoutAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;
                foreach (var vb in _activeVirtualBoxes.Values.ToList())
                {
                    if (vb.Status == VirtualBoxStatus.LabelGenerated &&
                        now - vb.DateTime > _timeout)
                    {
                        vb.Status = VirtualBoxStatus.Expired;
                        vb.ExpiredAt = now;
                        vb.IsInMemory = false;
                        RemoveFromMemory(vb);
                        await SaveVirtualBoxAsync(vb);
                    }
                }
            }
            catch
            {
                // ignore watcher errors
            }

            await Task.Delay(_checkInterval, token);
        }
    }
}

