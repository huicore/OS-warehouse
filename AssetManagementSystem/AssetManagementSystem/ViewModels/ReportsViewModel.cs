using AssetManagementSystem.Data;
using AssetManagementSystem.Models;
using AssetManagementSystem.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows;

namespace AssetManagementSystem.ViewModels
{
    public class ReportsViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;
        private readonly ExportService _exportService;

        public ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; }

        public ICommand GenerateReportCommand { get; }
        public ICommand ExportReportCommand { get; }

        public ReportsViewModel(AppDbContext context, ExportService exportService)
        {
            _context = context;
            _exportService = exportService;

            GenerateReportCommand = new RelayCommand(_ => GenerateReport());
            ExportReportCommand = new RelayCommand(_ => ExportReport());

            GenerateReport();
        }

        private void GenerateReport()
        {
            var reportData = _context.Assets
                .Include(a => a.AssetType)
                .GroupBy(a => a.AssetType.Name)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToList();

            Series = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = reportData.Select(x => x.Count).ToArray(),
                    Name = "Количество ОС"
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = reportData.Select(x => x.Type).ToArray(),
                    LabelsRotation = 15
                }
            };
        }

        private void ExportReport()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = "Отчет по типам ОС.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var data = _context.Assets
                    .Include(a => a.AssetType)
                    .GroupBy(a => a.AssetType.Name)
                    .Select(g => new ReportItem
                    {
                        AssetType = g.Key,
                        Count = g.Count(),
                        Percentage = (double)g.Count() / _context.Assets.Count() * 100
                    })
                    .ToList();

                _exportService.ExportReport(data, saveDialog.FileName);
                MessageBox.Show("Отчет успешно экспортирован", "Успех");
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