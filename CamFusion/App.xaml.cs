using System;
using System.ComponentModel;
using System.Windows;
using System.Xaml;
using CamFusion.Services;
using CC.Core.Devices.Impl;
using CC.Core.Services;
using CC.Core.Services.Impl;
using CC.Data.Entities.Settings;
using CC.UI.Navigators;
using CC.UI.ViewModels.MainWindow;
using CC.UI.ViewModels.Windows;
using CC.UI.Views.Windows;

namespace CamFusion;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public new MainWindow MainWindow { get; set; }
    public CreateTaskWindow CreateReportTaskWindow { get; set; }


    public MainWindowViewModel MainWindowViewModel { get; set; }
    public CreateTaskWindowViewModel CreateReportTaskWindowViewModel { get; set; }


    public MainViewModel MainViewModel { get; set; }
    public TasksViewModel ReportTasksViewModel { get; set; }
    public AggregationViewModel HandleAggregationViewModel { get; set; }
    public EventsViewModel EventsViewModel { get; set; }
    public PrinterViewModel PrinterViewModel { get; set; }
    public ErrorsViewModel ErrorsViewModel { get; set; }
    public SettingsViewModel SettingsViewModel { get; set; }
    public LoginViewModel LoginViewModel { get; set; }


    public MainWindowNavigator MainWindowNavigator { get; set; }


    public ISettingsService SettingsService { get; set; }
    public LocalDb LocalDbService { get; set; }
    public NomenclatureService NomenclatureService { get; set; }
    public ReportTaskService ReportTaskService { get; set; }
    public IDeviceService DeviceService { get; set; }
    public ProcessingCodeService ProcessingCodeService { get; set; }
    public ErrorsService ErrorsService { get; set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {

            // Settings.LocalDb = new DbSettings()
            // {
            //     Name = "База данных (локальная)",
            //     ServerName = "localhost",
            //     DatabaseName = "camfusion",
            //     IsAuthentification = false,
            //     Login = "",
            //     Password = "",
            //     IsUsed = true,
            // };
            LocalDbService = new LocalDb();

            SettingsService = new SettingsService(LocalDbService);
            SettingsService.LoadSettings();


            ErrorsService = new ErrorsService();

            NomenclatureService = new NomenclatureService(LocalDbService);
            NomenclatureService.StartLodingNomenclatureAsync(SettingsService.Settings.Line.PathLoadNomenclatureFiles);

            if (SettingsService.Settings != null)
                ReportTaskService = new ReportTaskService(LocalDbService,
                    SettingsService.Settings.Line,
                    ErrorsService);

            ProcessingCodeService = new ProcessingCodeService(ReportTaskService, LocalDbService);

            DeviceService = new DeviceService(SettingsService,
                ReportTaskService,
                LocalDbService,
                ProcessingCodeService);


            CreateReportTaskWindowViewModel = new CreateTaskWindowViewModel(NomenclatureService,
                ReportTaskService);
            CreateReportTaskWindow = new CreateTaskWindow();

            CreateReportTaskWindow.DataContext = CreateReportTaskWindowViewModel;

            ErrorsViewModel = new ErrorsViewModel(ErrorsService);

            MainViewModel = new MainViewModel(ReportTaskService,
                SettingsService,
                DeviceService);
            ReportTasksViewModel = new TasksViewModel(ReportTaskService,
                CreateReportTaskWindow,
                DeviceService);

            HandleAggregationViewModel = new AggregationViewModel(ReportTaskService,
                ProcessingCodeService,
                LocalDbService,
                ErrorsService);

            EventsViewModel = new EventsViewModel();
            PrinterViewModel = new PrinterViewModel();

            SettingsViewModel = new SettingsViewModel(SettingsService,
                DeviceService,
                ReportTaskService);

            LoginViewModel = new LoginViewModel();

            MainWindowNavigator = new MainWindowNavigator(MainViewModel,
                ReportTasksViewModel,
                HandleAggregationViewModel,
                EventsViewModel,
                PrinterViewModel,
                ErrorsViewModel,
                SettingsViewModel,
                LoginViewModel);

            LoginViewModel.LoginSucceeded += () =>
            {
                MainWindowNavigator.IsSettingsAuthorized = true;
                MainWindowNavigator.UpdateCurrentViewModelCommand.Execute(MainWindowViewType.SettingsView);
            };

            MainWindowViewModel = new MainWindowViewModel(MainWindowNavigator,
                SettingsService,
                ErrorsService);

            MainWindow = new MainWindow();

            MainWindow.DataContext = MainWindowViewModel;
            MainWindow.Closing += MainWindow_Closing;
            MainWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        Current.Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        NomenclatureService.StopLoadingNomenclatures();
        DeviceService.StopDevices();
        DeviceService.DisconnectDevices();
        CreateReportTaskWindow.Close();
        CreateReportTaskWindow = null;
    }
}