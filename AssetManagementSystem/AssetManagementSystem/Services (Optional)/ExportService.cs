using AssetManagementSystem.Models;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AssetManagementSystem.Services
{
    public class ExportService
    {
        public void ExportToExcel(IEnumerable<Asset> assets, string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Основные средства");

                    // Заголовки
                    var header = worksheet.Range("A1:J1");
                    header.Style.Font.Bold = true;
                    header.Style.Fill.BackgroundColor = XLColor.LightGray;
                    header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(1, 1).Value = "Инв. №";
                    worksheet.Cell(1, 2).Value = "Тип ОС";
                    worksheet.Cell(1, 3).Value = "Наименование";
                    worksheet.Cell(1, 4).Value = "Серийный №";
                    worksheet.Cell(1, 5).Value = "Статус";
                    worksheet.Cell(1, 6).Value = "Подразделение";
                    worksheet.Cell(1, 7).Value = "МОЛ";
                    worksheet.Cell(1, 8).Value = "Местоположение";
                    worksheet.Cell(1, 9).Value = "Дата постановки";
                    worksheet.Cell(1, 10).Value = "Примечания";

                    // Данные
                    int row = 2;
                    foreach (var asset in assets.OrderBy(a => a.InventoryNumber))
                    {
                        worksheet.Cell(row, 1).Value = asset.InventoryNumber;
                        worksheet.Cell(row, 2).Value = asset.AssetType?.Name;
                        worksheet.Cell(row, 3).Value = asset.Name;
                        worksheet.Cell(row, 4).Value = asset.SerialNumber;
                        worksheet.Cell(row, 5).Value = asset.Status;
                        worksheet.Cell(row, 6).Value = asset.Department?.Name;
                        worksheet.Cell(row, 7).Value = asset.MOL;
                        worksheet.Cell(row, 8).Value = asset.Location;

                        if (asset.PurchaseDate.HasValue)
                        {
                            worksheet.Cell(row, 9).Value = asset.PurchaseDate.Value;
                            worksheet.Cell(row, 9).Style.NumberFormat.Format = "dd.MM.yyyy";
                        }

                        worksheet.Cell(row, 10).Value = asset.Notes;
                        row++;
                    }

                    // Форматирование
                    worksheet.Columns().AdjustToContents();
                    worksheet.PageSetup.PrintAreas.Add("A1:J" + (row - 1));
                    worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                    worksheet.PageSetup.SetMargins(1, 1, 1, 1);

                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при экспорте в Excel", ex);
            }
        }

        public void ExportReport(IEnumerable<ReportItem> reportData, string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    // Лист с данными
                    var dataWorksheet = workbook.Worksheets.Add("Данные");

                    // Заголовки
                    dataWorksheet.Cell(1, 1).Value = "Тип ОС";
                    dataWorksheet.Cell(1, 2).Value = "Количество";
                    dataWorksheet.Cell(1, 3).Value = "Доля (%)";

                    // Данные
                    int row = 2;
                    foreach (var item in reportData.OrderByDescending(x => x.Count))
                    {
                        dataWorksheet.Cell(row, 1).Value = item.AssetType;
                        dataWorksheet.Cell(row, 2).Value = item.Count;
                        dataWorksheet.Cell(row, 3).Value = item.Percentage / 100;
                        dataWorksheet.Cell(row, 3).Style.NumberFormat.Format = "0.00%";
                        row++;
                    }

                    // Форматирование
                    dataWorksheet.Columns().AdjustToContents();

                    // Лист с диаграммой
                    var chartWorksheet = workbook.Worksheets.Add("Диаграмма");

                    // Создаем диаграмму
                    var pieChart = workbook.AddChart<XL.PieChart>();
                    pieChart.SetPosition(chartWorksheet.Cell(2, 2), chartWorksheet.Cell(20, 10));
                    pieChart.Title.Text = "Распределение ОС по типам";

                    var series = pieChart.AddSeries(
                        dataWorksheet.Range(2, 1, row - 1, 1),
                        dataWorksheet.Range(2, 2, row - 1, 2));

                    series.Name = "Количество ОС";
                    pieChart.Legend.Position = XL.PieChartLegendPosition.Right;

                    // Добавляем диаграмму на лист
                    chartWorksheet.AddChart(pieChart);

                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при экспорте отчета", ex);
            }
        }

        public void ExportHistory(IEnumerable<AssetHistory> history, string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("История изменений");

                    // Заголовки
                    var header = worksheet.Range("A1:E1");
                    header.Style.Font.Bold = true;
                    header.Style.Fill.BackgroundColor = XLColor.LightGray;
                    header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(1, 1).Value = "Дата изменения";
                    worksheet.Cell(1, 2).Value = "Поле";
                    worksheet.Cell(1, 3).Value = "Старое значение";
                    worksheet.Cell(1, 4).Value = "Новое значение";
                    worksheet.Cell(1, 5).Value = "Пользователь";

                    // Данные
                    int row = 2;
                    foreach (var record in history.OrderByDescending(h => h.ChangedAt))
                    {
                        worksheet.Cell(row, 1).Value = record.ChangedAt;
                        worksheet.Cell(row, 1).Style.NumberFormat.Format = "dd.MM.yyyy HH:mm";

                        worksheet.Cell(row, 2).Value = record.ChangedField;
                        worksheet.Cell(row, 3).Value = record.OldValue;
                        worksheet.Cell(row, 4).Value = record.NewValue;
                        worksheet.Cell(row, 5).Value = record.ChangedBy?.FullName;
                        row++;
                    }

                    // Форматирование
                    worksheet.Columns().AdjustToContents();
                    worksheet.PageSetup.PrintAreas.Add("A1:E" + (row - 1));
                    worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;

                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при экспорте истории", ex);
            }
        }
    }

    public class ReportItem
    {
        public string AssetType { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}