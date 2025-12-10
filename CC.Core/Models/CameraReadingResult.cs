using System;

namespace CC.Core.Models;

public class CameraReadingResult
{
    public DateTime DateTime { get; set; } = DateTime.Now;
    public string StatisticCameraReadingResult { get; set; } = "---";
    public string ProductCamera1ReadingResult { get; set; } = "---";
    public string ProductCamera2ReadingResult { get; set; } = "---";
    public string VerificationCamera1ReadingResult { get; set; } = "---";
    public string VerificationCamera2ReadingResult { get; set; } = "---";
}