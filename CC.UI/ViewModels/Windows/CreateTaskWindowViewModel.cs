using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CC.Core.Commands.Base;
using CC.Core.Services.Impl;
using CC.Data.Entities.Tasks;
using CC.UI.ViewModels.Base;
using CC.UI.Views.Windows;

namespace CC.UI.ViewModels.Windows;

public class CreateTaskWindowViewModel:ViewModel
{
       private ReportTask reportTask;
        public ReportTask ReportTask
        {
            get => reportTask;
            set
            {
                reportTask = value;
                OnPropertyChanged(nameof(ReportTask));
            }
        }

        private Nomenclature selectedNomenclature;
        public Nomenclature SelectedNomenclature
        {
            get => selectedNomenclature;
            set
            {
                selectedNomenclature = value;
                OnPropertyChanged(nameof(SelectedNomenclature));

                if (selectedNomenclature != null)
                {
                    ReportTask.Nomenclature = SelectedNomenclature;
                    ReportTask.NomenclatureId = SelectedNomenclature.Id;

                    ReportTask.ExpiryDateInDays = Convert.ToInt32(selectedNomenclature.Attributes.FirstOrDefault(c=> c.Code == 11).Value);
                    ReportTask.ExpiryDate = ReportTask.ManufactureDate.AddDays(ReportTask.ExpiryDateInDays);
                }
            }
        }


        private ICommand closeWindowCommand;
        public ICommand CloseWindowCommand => closeWindowCommand;
        private bool CanCloseWindowCommandExecute(object p) => true;
        private void OnCloseWindowCommandExecuted(object p)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(CreateTaskWindow))
                {
                    window.IsVisibleChanged += Window_IsVisibleChanged;
                    window.Hide();
                }
            }
        }

        private ICommand createTaskCommand;
        public ICommand CreateTaskCommand => createTaskCommand;
        private bool CanCreateTaskCommandExecute(object p) =>
            SelectedNomenclature != null && ReportTask != null &&
            !string.IsNullOrEmpty(ReportTask.LotNumber) && Convert.ToInt32(ReportTask.LotNumber) != 0 &&
            ReportTask.ManufactureDate != DateTime.MinValue &&
            ReportTask.ExpiryDate != DateTime.MinValue &&
            !string.IsNullOrEmpty(ReportTask.CountProductInBox) && Convert.ToInt32(ReportTask.CountProductInBox) != 0 &&
            !string.IsNullOrEmpty(ReportTask.CountBoxInPallet) && Convert.ToInt32(ReportTask.CountBoxInPallet) != 0;
        private void OnCreateTaskCommandExecuted(object p)
        {
            bool isAddedReportTask = ReportTaskService.CreateReportTask(ReportTask);
            if (isAddedReportTask)
            {
                CloseWindowCommand.Execute(null);
            }
        }


        public NomenclatureService NomenclatureService { get; set; }
        public ReportTaskService ReportTaskService { get; set; }

        private bool _isCountProductInBoxReadOnly;
        public bool IsCountProductInBoxReadOnly
        {
            get => _isCountProductInBoxReadOnly;
            set
            {
                _isCountProductInBoxReadOnly = value;
                OnPropertyChanged(nameof(IsCountProductInBoxReadOnly));

                // persist user preference between sessions
                Application.Current.Properties["IsCountProductInBoxReadOnly"] = value;
            }
        }

        public CreateTaskWindowViewModel(NomenclatureService nomenclatureService,
                                               ReportTaskService? reportTaskService)
        {
            NomenclatureService = nomenclatureService;
            ReportTaskService = reportTaskService;

            ReportTask = new ReportTask();
            ReportTask.ManufactureDate = DateTime.Now.Date.AddDays(1);
            ReportTask.CountProductInBox = "12";

            if (Application.Current.Properties.Contains("IsCountProductInBoxReadOnly") &&
                Application.Current.Properties["IsCountProductInBoxReadOnly"] is bool savedReadonly)
            {
                IsCountProductInBoxReadOnly = savedReadonly;
            }
            else
            {
                IsCountProductInBoxReadOnly = false;
            }

            closeWindowCommand = new RelayCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecute);
            createTaskCommand = new RelayCommand(OnCreateTaskCommandExecuted, CanCreateTaskCommandExecute);
        }


        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ReportTask = new ReportTask();
            ReportTask.ManufactureDate = DateTime.Now.Date;
            ReportTask.CountProductInBox = "12";

            if (Application.Current.Properties.Contains("IsCountProductInBoxReadOnly") &&
                Application.Current.Properties["IsCountProductInBoxReadOnly"] is bool savedReadonly)
            {
                IsCountProductInBoxReadOnly = savedReadonly;
            }
            else
            {
                IsCountProductInBoxReadOnly = false;
            }
            SelectedNomenclature = SelectedNomenclature;
        }
}