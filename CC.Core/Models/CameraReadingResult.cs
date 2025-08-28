using System;

namespace CC.Core.Models;

public class CameraReadingResult
{
    public DateTime DateTime { get; set; } = DateTime.Now;
    public string BoxCameraReadingResult { get; set; } = "---";

    public string ProductCameraSlaveReadingResult { get; set; } = "---";

    public string ProductCameraMasterReadingResult { get; set; } = "---";   
}