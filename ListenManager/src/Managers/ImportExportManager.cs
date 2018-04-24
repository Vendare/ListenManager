using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using ListenManager.Database.DataObjects;
using ListenManager.Database.Handlers;
using ListenManager.Enums;
using ListenManager.IO;
using ListenManager.src.Database.Model;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace ListenManager.Managers
{
    public class ImportExportManager : BaseManager
    {
        private readonly IDialogCoordinator _coordinator;
        private readonly VerzeichnisHandler _handler;

        private int _inputRowCount;
        private int _sheetIndex;
        private string _arbeitsblattName;

        private FileInfo _inputFileInfo;
        private FileInfo _outputFileInfo;

        private ICommand _importCommand;
        private ICommand _exportCommand;
        private ICommand _inputFileCommand;
        private ICommand _outputFileCommand;

        private MitgliedsListe _selectedListe;
        private ObservableCollection<MitgliedsListe> _listen;

        public ImportExportManager()
        {
            _coordinator = DialogCoordinator.Instance;
            _handler = VerzeichnisHandler.Instance;

            var alle = new MitgliedsListe() { Name = "Alle", SourceVerzeichnis = null, Type = ListType.Alle };
            var erwa = new MitgliedsListe() { Name = "Erwachsene", SourceVerzeichnis = null, Type = ListType.Erwachsene };
            var jugd = new MitgliedsListe() { Name = "Jugend", SourceVerzeichnis = null, Type = ListType.Jugend };

            Listen = _handler.GetAllVerzeichnisse();
            Listen.Insert(0, alle);
            Listen.Insert(1, erwa);
            Listen.Insert(2, jugd);

            SelectedListe = alle;
        }


        public FileInfo InputFileInfo
        {
            get => _inputFileInfo;
            set
            {
                _inputFileInfo = value;
                OnPropertyChanged(nameof(InputFileInfo));
            }
        }

        public FileInfo OutFileInfo
        {
            get => _outputFileInfo;
            set
            {
                _outputFileInfo = value;
                OnPropertyChanged(nameof(OutFileInfo));
            } 
        }

        public int InputRowCount
        {
            get => _inputRowCount;
            set
            {
                _inputRowCount = value;
                OnPropertyChanged(nameof(InputRowCount));
            }
        }

        public int SheetIndex
        {
            get => _sheetIndex;
            set
            {
                _sheetIndex = value;
                OnPropertyChanged(nameof(SheetIndex));
            }
        }

        public string ArbeitsblattName
        {
            get => _arbeitsblattName;
            set
            {
                _arbeitsblattName = value;
                OnPropertyChanged(nameof(ArbeitsblattName));
            }
        }

        public ObservableCollection<MitgliedsListe> Listen
        {
            get => _listen;
            set
            {
                _listen = value;
                OnPropertyChanged(nameof(Listen));
            }
        }

        public MitgliedsListe SelectedListe
        {
            get => _selectedListe;
            set
            {
                _selectedListe = value;
                OnPropertyChanged(nameof(SelectedListe));
            }
        }

        public ICommand SelectInputFileCommand => _inputFileCommand ?? (_inputFileCommand = new RelayCommand(SelectInputFile));

        public ICommand SelectOutputFileCommand => _outputFileCommand ?? (_outputFileCommand = new RelayCommand(SelectOutputFile));

        public ICommand ImportCommand => _importCommand ?? (_importCommand = new RelayCommand(ImportData));

        public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new RelayCommand(ExportData));

        private void SelectInputFile()
        {
            var dialog = new OpenFileDialog()
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".xlsx",
                Filter = "Excel Dateien(*.xls, *.xlsx, *.xlsm)|*.xlsx;*.xls;*.xlsm"  
            };

            if (dialog.ShowDialog() == true)
            {
                InputFileInfo = new FileInfo(dialog.FileName);
            }
        }

        private void SelectOutputFile()
        {
            var dialog = new SaveFileDialog()
            {
                AddExtension = true,
                CheckFileExists = false,
                CheckPathExists = false,
                CreatePrompt = false,
                OverwritePrompt = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".xlsx",
                Filter = "Excel Dateien(*.xls, *.xlsx, *.xlsm)|*.xlsx;*.xls;*.xlsm"
            };

            if (dialog.ShowDialog() == true)
            {
                OutFileInfo = new FileInfo(dialog.FileName);
            }
        }

        private async void ImportData()
        {
            var controller = await _coordinator.ShowProgressAsync(this, "Lese Daten", "Initialisiere", true);
            controller.SetIndeterminate();

            var worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            worker.RunWorkerCompleted += async delegate
            {
                await controller.CloseAsync();
            };

            worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs args)
            {
                var percent = args.ProgressPercentage;
                var message = args.UserState as string;
                controller.SetProgress(percent/100.0);
                controller.SetMessage(message);
            };

            controller.Canceled += delegate
            {
                worker.CancelAsync();
                worker.Dispose();
            };

            var reader = new ExcelReader(InputFileInfo)
            {
                RowCount = InputRowCount,
                CurrentSheet = SheetIndex - 1
            };

            reader.ImportFileAsync(worker);
        }

        private async void ExportData()
        {
            var controller = await _coordinator.ShowProgressAsync(this, "Lese Daten", "Initialisiere", true);
            controller.SetIndeterminate();

            var worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            worker.RunWorkerCompleted += async delegate
            {
                await controller.CloseAsync();
            };

            worker.ProgressChanged += delegate (object sender, ProgressChangedEventArgs args)
            {
                var percent = args.ProgressPercentage;
                var message = args.UserState as string;
                controller.SetProgress(percent / 100.0);
                controller.SetMessage(message);
            };

            controller.Canceled += delegate
            {
                worker.CancelAsync();
                worker.Dispose();
            };


            var visibilities = SelectedListe.Type == ListType.UserCreated
                ? _handler.GetFieldVisibilitiesForGivenList(SelectedListe.SourceVerzeichnis.ID)
                : new Visibilities() { SourceFieldvisibility = new Fieldvisibility() };

            var writer = new ExcelWriter(OutFileInfo)
            {
                MitgliedListe = SelectedListe,
                MitgliedsDaten = LoadMitglieder(),
                FieldsToExport = visibilities,
                SheetName = ArbeitsblattName != null && ArbeitsblattName.Trim().Equals(string.Empty) ? null : ArbeitsblattName
            };

            writer.ExportToExcelAsync(worker);
        }

        private ObservableCollection<VereinsMitglied> LoadMitglieder()
        {
            if (SelectedListe.Type == ListType.UserCreated)
            {
                return _handler.GetMitgliederInVerzeichnis(SelectedListe.SourceVerzeichnis.ID);
               
            }
            ObservableCollection<VereinsMitglied> tmp = null;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (SelectedListe.Type)
            {
                case ListType.Jugend:
                    tmp = _handler.GetJugend();
                    break;
                case ListType.Erwachsene:
                    tmp = _handler.GetErwachsene();
                    break;
                case ListType.Alle:
                    tmp = _handler.GetAllMitglieder();
                    break;
            }
            return tmp;
        }
    }
}
