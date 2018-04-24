using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using ListenManager.Database.DataObjects;
using ListenManager.Database.Handlers;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ListenManager.IO
{
    public class ExcelWriter
    {
        private readonly ExcelPackage _package;
        public string SheetName { get; set; }

        public FileInfo FilePath
        {
            get => _package.File;
            set => _package.File = value;
        }
        public ExcelWriter()
        {
            _package = new ExcelPackage();
        }

        public ExcelWriter(FileInfo excelFileInfo)
        {
            _package = new ExcelPackage(excelFileInfo);
        }

        public MitgliedsListe MitgliedListe { get; set; }

        public Visibilities FieldsToExport { get; set; }

        public ObservableCollection<VereinsMitglied> MitgliedsDaten { get; set; }

        public void ExportToExcel()
        {
            WriteToExcel();
        }

        public void ExportToExcelAsync(BackgroundWorker worker)
        {
            worker.DoWork += delegate { WriteToExcel(worker); };
            worker.RunWorkerAsync();
        }

        private void WriteToExcel(BackgroundWorker worker = null)
        {
            var name = SheetName ?? MitgliedListe.Name;

            if (_package == null) throw new IOException("Exceldatei nicht angelegt");

            if (_package.Workbook.Worksheets[name] != null)
            {
                _package.Workbook.Worksheets.Delete(name);
            }

            using (var sheet = _package.Workbook.Worksheets.Add(name))
            {
                // Creating a DataTable and filling it with the Selected Data is easier
                // Than conditionally checking what to put where
                var data = CreateDataTable(worker);
                if (worker != null && worker.CancellationPending) return;

                sheet.Cells["B2"].LoadFromDataTable(data, true);
                sheet.Cells.AutoFitColumns();

                worker?.ReportProgress(0, "Lege Styles An");

                sheet.Cells[2, 2, 2, data.Columns.Count + 1].Style.Font.Bold = true;
                sheet.Cells[2, 2, 2, data.Columns.Count + 1].Style.Font.Color.SetColor(Color.White);

                sheet.Cells[2, 2, 2, data.Columns.Count + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[2, 2, 2, data.Columns.Count + 1].Style.Fill.BackgroundColor.SetColor(Color.SteelBlue);

                for (var i = 4; i < data.Rows.Count + 2; i += 2)
                {
                    if (worker != null && worker.CancellationPending) return;
                    worker?.ReportProgress(i - 3 * 100 / data.Rows.Count - 2, "Lege Styles An");
                    sheet.Cells[i, 2, i, data.Columns.Count + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[i, 2, i, data.Columns.Count + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                sheet.Cells[2, 2, data.Rows.Count + 2, data.Columns.Count + 1].Style.Border.Bottom.Style =
                    ExcelBorderStyle.Thin;
                sheet.Cells[2, 2, data.Rows.Count + 2, data.Columns.Count + 1].Style.Border.Left.Style =
                    ExcelBorderStyle.Thin;
                sheet.Cells[2, 2, data.Rows.Count + 2, data.Columns.Count + 1].Style.Border.Right.Style =
                    ExcelBorderStyle.Thin;
                sheet.Cells[2, 2, data.Rows.Count + 2, data.Columns.Count + 1].Style.Border.Top.Style =
                    ExcelBorderStyle.Thin;

                worker?.ReportProgress(100, "Lege Styles An");
                _package.Save();
            }
        }


        private DataTable CreateDataTable(BackgroundWorker worker)
        {
            var table = new DataTable();
            var rowCount = 0;
            worker?.ReportProgress(0, "Verarbeite Daten");

            if (FieldsToExport.MitgliednrVisible)
            {
                table.Columns.Add(new DataColumn("Mitgliedsnummer"){ DataType = typeof(long) });
                rowCount++;
            }

            if (FieldsToExport.AnredeVisible)
            {
                table.Columns.Add("Anrede");
                rowCount++;
            }

            if (FieldsToExport.VornameVisible)
            {
                table.Columns.Add("Vorname");
                rowCount++;
            }

            if (FieldsToExport.NameVisible)
            {
                table.Columns.Add("Nachname");
                rowCount++;
            }

            if (FieldsToExport.StraßeVisible)
            {
                table.Columns.Add("Straße");
                rowCount++;
            }

            if (FieldsToExport.PostleitzahlVisible)
            {
                table.Columns.Add(new DataColumn("Postleitzahl") { DataType = typeof(long) });
                rowCount++;
            }

            if (FieldsToExport.OrtVisible)
            {
                table.Columns.Add("Ort");
                rowCount++;
            }

            if (FieldsToExport.BundeslandVisible)
            {
                table.Columns.Add("Bundesland");
                rowCount++;
            }

            if (FieldsToExport.EmailVisible)
            {
                table.Columns.Add("E-Mail");
                rowCount++;
            }

            if (FieldsToExport.TelefonVisible)
            {
                table.Columns.Add("Telefon");
                rowCount++;
            }

            if (FieldsToExport.MobilVisible)
            {
                table.Columns.Add("Mobil");
                rowCount++;
            }

            if (FieldsToExport.EintrittsdatumVisible)
            {
                table.Columns.Add(new DataColumn("Eintrittsdatum"));
                rowCount++;
            }

            if (FieldsToExport.GeburtsdatumVisible)
            {
                table.Columns.Add(new DataColumn("Geburtsdatum"));
                rowCount++;
            }

            if (FieldsToExport.IbanVisible)
            {
                table.Columns.Add("IBAN");
                rowCount++;
            }

            if (FieldsToExport.BicVisible)
            {
                table.Columns.Add("BIC");
                rowCount++;
            }

            if (FieldsToExport.KreditinstitutVisible)
            {
                table.Columns.Add("Kreditinstitut");
                rowCount++;
            }

            var data = MitgliedsDaten;
            var count = 1;
            foreach (var mitglied in data)
            {
                if (worker != null && worker.CancellationPending) return null;
                
                var row = new object[rowCount];
                var pos = 0;
                if (FieldsToExport.MitgliednrVisible)
                {
                    row[pos] = mitglied.Mitgliedsnr;
                    pos++;
                }

                if (FieldsToExport.AnredeVisible)
                {
                    row[pos] = mitglied.Anrede;
                    pos++;
                }

                if (FieldsToExport.VornameVisible)
                {
                    row[pos] = mitglied.Vorname;
                    pos++;
                }

                if (FieldsToExport.NameVisible)
                {
                    row[pos] = mitglied.Name;
                    pos++;
                }

                if (FieldsToExport.StraßeVisible)
                {
                    row[pos] = mitglied.Straße;
                    pos++;
                }

                if (FieldsToExport.PostleitzahlVisible)
                {
                    row[pos] = mitglied.Postleitzahl;
                    pos++;
                }

                if (FieldsToExport.OrtVisible)
                {
                    row[pos] = mitglied.Ort;
                    pos++;
                }

                if (FieldsToExport.BundeslandVisible)
                {
                    row[pos] = mitglied.Bundesland;
                    pos++;
                }

                if (FieldsToExport.EmailVisible)
                {
                    row[pos] = mitglied.Email;
                    pos++;
                }

                if (FieldsToExport.TelefonVisible)
                {
                    row[pos] = mitglied.Telefon;
                    pos++;
                }

                if (FieldsToExport.MobilVisible)
                {
                    row[pos] = mitglied.Mobil;
                    pos++;
                }

                if (FieldsToExport.EintrittsdatumVisible)
                {
                    row[pos] = mitglied.Eintrittsdatum.ToString("dd.MM.yyyy");
                    pos++;
                }

                if (FieldsToExport.GeburtsdatumVisible)
                {
                    row[pos] = mitglied.Geburtsdatum.ToString("dd.MM.yyyy");
                    pos++;
                }

                if (FieldsToExport.IbanVisible)
                {
                    row[pos] = mitglied.IBAN;
                    pos++;
                }

                if (FieldsToExport.BicVisible)
                {
                    row[pos] = mitglied.BIC;
                    pos++;
                }

                if (FieldsToExport.KreditinstitutVisible)
                {
                    row[pos] = mitglied.Kreditinstitut;
                }

                table.Rows.Add(row);
                
                worker?.ReportProgress(count*100/rowCount,"Verarbeite Daten");
                count++;
            }
            worker?.ReportProgress(100, "Verarbeite Daten");
            return table;
        }
    }
}
