using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CC.Data.Entities.Codes;

namespace CC.Core.Services;

public interface IVirtualBoxService
{
    VirtualBox CreateVirtualBox(Guid taskGuid, int taskId, List<string> productCodes);
    string GenerateBoxLabelCode(Guid taskGuid, int boxNumber);
    Task<VirtualBox> SaveVirtualBoxAsync(VirtualBox virtualBox);
    VirtualBox? FindVirtualBoxByLabelCode(string labelCode);
    VirtualBox? FindVirtualBoxByProductCode(string productCode);
    Task<bool> VerifyBoxAsync(string boxLabelCode, string productCode);
    Task<Box> ConvertToRealBoxAsync(VirtualBox virtualBox, int palletId);
    List<VirtualBox> GetActiveVirtualBoxes();
    void RemoveFromMemory(VirtualBox virtualBox);
    void StartTimeoutWatcher(TimeSpan? timeout = null, TimeSpan? checkInterval = null);
    void StopTimeoutWatcher();
}

