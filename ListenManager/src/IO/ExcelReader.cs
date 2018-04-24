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
                    for (var i = 0; i < RowCount; i++)
                    {
                        if (worker != null && worker.CancellationPending)
                        {
                            break;
                        }
                        var ortId = handler.GetOrtForPlz(sheet.Cells[i + 2, 6].GetValue<long>());
                        var m = new Mitglied
                        {
                            Mitgliedsnr = sheet.Cells[i + 2, 1].GetValue<long>(),
                            Anrede = sheet.Cells[i + 2, 2].GetValue<string>(),
                            Vorname = sheet.Cells[i + 2, 3].GetValue<string>(),
                            Name = sheet.Cells[i + 2, 4].GetValue<string>(),
                            Straße = sheet.Cells[i + 2, 5].GetValue<string>(),
                            ID_Ort = ortId.First().SourceOrt.ID,
                            eMail = sheet.Cells[i + 2, 9].GetValue<string>(),
                            Telefon = sheet.Cells[i + 2, 10].GetValue<string>(),
                            Mobil = sheet.Cells[i + 2, 11].GetValue<string>(),
                            Eintrittsdatum = sheet.Cells[i + 2, 12].GetValue<DateTime>(),
                            Geburtsdatum = sheet.Cells[i + 2, 13].GetValue<DateTime>(),
                            IBAN = sheet.Cells[i + 2, 14].GetValue<string>(),
                            BIC = sheet.Cells[i + 2, 15].GetValue<string>(),
                            Kreditinstitut = sheet.Cells[i + 2, 16].GetValue<string>()
                        };
                        handler.AddMitglied(m);
                        if (worker == null || !worker.WorkerReportsProgress) continue;

                        if (i == 0)
                        {
                            worker.ReportProgress(1 * 100 / RowCount);
                        }
                        else
                        {
                            worker.ReportProgress(i * 100 / RowCount);
                        }
                    }
                }
            }
        }
    }
}
