using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using ListenManager.Database.Handlers;
using ListenManager.src.Database.Model;
using OfficeOpenXml;

namespace ListenManager.IO
{
    public class ExcelReader
    {
        
        private readonly ExcelPackage _package;
        public int RowCount { get; set; }
        public int CurrentSheet { get; set; }
        public string StartPos { get; set; }
        public FileInfo FilePath
        {
            get => _package.File;
            set => _package.File = value;
        }
        public ExcelReader()
        {
            CurrentSheet = 0;
            _package = new ExcelPackage();
        }

        public ExcelReader(FileInfo inputFileInfo)
        {
            _package = new ExcelPackage(inputFileInfo);
        }

        public void ImportFile()
        {
            ReadFileFromExcel();
        }

        public void ImportFileAsync(BackgroundWorker worker)
        {
            worker.DoWork += delegate
            {
                ReadFileFromExcel(worker);
            };
            worker.RunWorkerAsync();
        }

        private void ReadFileFromExcel(BackgroundWorker worker = null)
        {
            var handler = VerzeichnisHandler.Instance;
            using (var workbook = _package.Workbook)
            {
                using (var sheet = workbook.Worksheets[CurrentSheet])
                {
                    if (StartPos == null)
                    {
                        StartPos = "A1";
                    }

                    var start = sheet.Cells[StartPos].Start;
                    
                    for (int i = start.Row, count = 0; count < RowCount; i++, count++)
                    {
                        if (worker != null && worker.CancellationPending)
                        {
                            break;
                        }
                        var ortId = handler.GetOrtForPlz(sheet.Cells[i, start.Column + 5].GetValue<long>());
                        var m = new Mitglied
                        {
                            Mitgliedsnr = sheet.Cells[i, start.Column].GetValue<long>(),
                            Anrede = sheet.Cells[i, start.Column + 1].GetValue<string>(),
                            Vorname = sheet.Cells[i, start.Column + 2].GetValue<string>(),
                            Name = sheet.Cells[i, start.Column + 3].GetValue<string>(),
                            Straße = sheet.Cells[i, start.Column + 4].GetValue<string>(),
                            ID_Ort = ortId.First().SourceOrt.ID,
                            eMail = sheet.Cells[i, start.Column + 8].GetValue<string>(),
                            Telefon = sheet.Cells[i, start.Column + 9].GetValue<string>(),
                            Mobil = sheet.Cells[i, start.Column + 10].GetValue<string>(),
                            Eintrittsdatum = sheet.Cells[i, start.Column + 11].GetValue<DateTime>(),
                            Geburtsdatum = sheet.Cells[i, start.Column + 12].GetValue<DateTime>(),
                            IBAN = sheet.Cells[i, start.Column + 13].GetValue<string>(),
                            BIC = sheet.Cells[i, start.Column + 14].GetValue<string>(),
                            Kreditinstitut = sheet.Cells[i, start.Column + 15].GetValue<string>()
                        };
                        handler.AddMitglied(m);
                        if (worker == null || !worker.WorkerReportsProgress) continue;

                        if (i == 0)
                        {
                            worker.ReportProgress(1 * 100 / RowCount);
                        }
                        else
                        {
                            worker.ReportProgress(count * 100 / RowCount);
                        }
                    }
                }
            }
        }
    }
}
