using System.Linq;
using System.Text.RegularExpressions;
using CC.Core.Devices.Impl;

namespace CC.Core.Services.Impl;

public class ProcessingCodeService
{
    private readonly string _patternOnPalletCode = @"^01\d{14}" +
                                                   @"11\d{6}" +
                                                   @"17\d{6}" +
                                                   @"10\d{4}" +
                                                   @"(\u001d|)?21\d{1,5}" +
                                                   @"(\u001d|)?37\d{1,10}$";

    private string _patternOnPalletCodeTheCurrentReportTask = null!;

    private readonly string _patternOnBoxCode = @"^01\d{14}" +
                                                @"11\d{6}" +
                                                @"17\d{6}" +
                                                @"10\d{4}" +
                                                @"(\u001d|)?21\d{1,5}$";

    private string _patternOnBoxCodeTheCurrentReportTask = null!;

    private readonly string _patternOnProductCode = @"^01\d{14}21" +
                                                    @".{1,10}" +
                                                    @"(\u001d|)?93.{4}$";

    private string _patternOnProductCodeTheCurrentReportTask = null!;

    private readonly Regex _regexOnPalletCode;
    private Regex _regexOnPalletCodeTheCurrentReportTask = null!;
    private readonly Regex _regexOnBoxCode;
    private Regex _regexOnBoxCodeTheCurrentReportTask = null!;
    private readonly Regex _regexOnProductCode;
    private Regex _regexOnProductCodeTheCurrentReportTask = null!;


    public ReportTaskService ReportTaskService { get; set; }
    public LocalDb LocalDbService { get; set; }

    public ProcessingCodeService(ReportTaskService? reportTaskService,
        LocalDb localDbService)
    {
        ReportTaskService = reportTaskService;
        LocalDbService = localDbService;

        _regexOnPalletCode = new Regex(_patternOnPalletCode);
        _regexOnBoxCode = new Regex(_patternOnBoxCode);
        _regexOnProductCode = new Regex(_patternOnProductCode);
    }


    public bool IsPalletCode(string code)
    {
        bool isCode = _regexOnPalletCode.Match(code).Success;
        return isCode;
    }

    public bool IsPalletCodeTheCurrentTask(string code)
    {
        _patternOnPalletCodeTheCurrentReportTask = $@"^01{ReportTaskService.CurrentReportTask.Nomenclature!.Gtin}" +
                                                   $@"11{ReportTaskService.CurrentReportTask.ManufactureDate:yyMMdd}" +
                                                   $@"17{ReportTaskService.CurrentReportTask.ExpiryDate:yyMMdd}" +
                                                   $@"10{ReportTaskService.CurrentReportTask.LotNumber}" +
                                                   @"(\u001d|)?21\d{1,5}" +
                                                   @"(\u001d|)?37\d{1,10}$";


        _regexOnPalletCodeTheCurrentReportTask = new Regex(_patternOnPalletCodeTheCurrentReportTask);

        bool isCodeTheCurrentTask = _regexOnPalletCodeTheCurrentReportTask.Match(code).Success;
        return isCodeTheCurrentTask;
    }

    public bool IsRepeatPalletCode(string code)
    {
        bool isRepeatCode;
        var pallets = ReportTaskService.Statistic.PalletCodes.FirstOrDefault(p =>
            p.MarkingCode!.Replace("\u001d", "") == code.Replace("\u001d", ""));
        if (pallets == null)
        {
            isRepeatCode = false;
        }
        else
        {
            isRepeatCode = true;
        }

        return isRepeatCode;
    }

    public bool IsBoxCode(string code)
    {
        bool isCode = _regexOnBoxCode.Match(code).Success;
        return isCode;
    }

    public bool IsBoxCodeTheCurrentTask(string code)
    {
        _patternOnBoxCodeTheCurrentReportTask =
            $@"^01{ReportTaskService.CurrentReportTask.Nomenclature!.Attributes.FirstOrDefault(c => c.Code == 12)!.Value}" +
            $@"11{ReportTaskService.CurrentReportTask.ManufactureDate.ToString("yyMMdd")}" +
            $@"17{ReportTaskService.CurrentReportTask.ExpiryDate.ToString("yyMMdd")}" +
            $@"10{ReportTaskService.CurrentReportTask.LotNumber}" +
            @"(\u001d|)?21\d{1,5}";
        _regexOnBoxCodeTheCurrentReportTask = new Regex(_patternOnBoxCodeTheCurrentReportTask);

        bool isCodeTheCurrentTask = _regexOnBoxCodeTheCurrentReportTask.Match(code).Success;
        return isCodeTheCurrentTask;
    }

    public bool IsRepeatBoxCode(string code)
    {
        bool isRepeatCode;
        var boxes = ReportTaskService.Statistic.BoxCodes.FirstOrDefault(p =>
            p.MarkingCode!.Replace("\u001d", "") == code.Replace("\u001d", ""));
        if (boxes == null)
        {
            isRepeatCode = false;
        }
        else
        {
            isRepeatCode = true;
        }

        return isRepeatCode;
    }

    public bool IsProductCode(string code)
    {
        bool isProductCode = _regexOnProductCode.Match(code).Success;
        return isProductCode;
    }

    public bool IsProductCodeTheCurrentTask(string code)
    {
        _patternOnProductCodeTheCurrentReportTask = @$"01{ReportTaskService.CurrentReportTask.Nomenclature!.Gtin}21" +
                                                    @".{1,10}" +
                                                    @"(\u001d|)?93.{4}$";
        _regexOnProductCodeTheCurrentReportTask = new Regex(_patternOnProductCodeTheCurrentReportTask);

        bool isCodeTheCurrentTask = _regexOnProductCodeTheCurrentReportTask.Match(code).Success;
        return isCodeTheCurrentTask;
    }

    public bool IsRepeatProductCode(string code)
    {
        bool isRepeatCode;
        var products = ReportTaskService.Statistic.ProductCodes.FirstOrDefault(p =>
            p.MarkingCode!.Replace("\u001d", "") == code.Replace("\u001d", ""));
        if (products == null)
        {
            isRepeatCode = false;
        }
        else
        {
            isRepeatCode = true;
        }

        return isRepeatCode;
    }
}