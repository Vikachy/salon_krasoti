using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.Entity;
using System.Collections.Generic;

namespace salon_krasoti.Pages
{
    public partial class SalesChartPage : System.Windows.Controls.Page
    {
        private Entities context = Entities.GetContext();
        private string currentGroupBy = "Employee";
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;

        public SalesChartPage()
        {
            InitializeComponent();
            InitializeChart();
            LoadData();
        }

        private void InitializeChart()
        {
            chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            wfhChart.Child = chart;

            var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("MainArea");
            chartArea.AxisX.LabelStyle.Format = "dd.MM.yyyy";
            chartArea.AxisY.LabelStyle.Format = "#,##0 руб";
            chart.ChartAreas.Add(chartArea);

            var legend = new System.Windows.Forms.DataVisualization.Charting.Legend("MainLegend");
            legend.Docking = Docking.Bottom;
            chart.Legends.Add(legend);
        }

        private void LoadData()
        {
            try
            {
                var salesData = context.Sales
                    .Include(s => s.Employees)  
                    .Include(s => s.Services)   
                    .OrderBy(s => s.SaleDate)
                    .ToList();

                if (!salesData.Any())
                {
                    MessageBox.Show("Нет данных для отображения", "Информация", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                UpdateChart(salesData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateChart(List<Sales> salesData)
        {
            if (chart == null || cbChartType == null || cbGroupBy == null) return;

            chart.Series.Clear();

            string chartType = (cbChartType.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Column";
            currentGroupBy = (cbGroupBy.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Employee";

            var series = new System.Windows.Forms.DataVisualization.Charting.Series("SalesData")
            {
                ChartType = GetSeriesChartType(chartType),
                IsValueShownAsLabel = true,
                LabelFormat = "#,##0 руб" 
            };

            var groupedData = currentGroupBy == "Employee"
                ? salesData.GroupBy(s => s.Employees.FirstName + " " + s.Employees.LastName)
                          .Select(g => new {
                              Key = g.Key,
                              Value = g.Sum(s => s.Services.Price * s.QuantitySold)
                          })
                : salesData.GroupBy(s => s.Services.ServiceName)
                          .Select(g => new {
                              Key = g.Key,
                              Value = g.Sum(s => s.Services.Price * s.QuantitySold)
                          });

            foreach (var item in groupedData)
            {
                DataPoint point = new DataPoint();
                point.SetValueXY(item.Key, item.Value);
                point.LegendText = $"{item.Key}: {item.Value:#,##0} руб";
                series.Points.Add(point);
            }

            chart.Series.Add(series);
            chart.Titles.Clear();
            chart.Titles.Add($"Продажи по {(currentGroupBy == "Employee" ? "сотрудникам" : "услугам")}");
        }

        private SeriesChartType GetSeriesChartType(string chartType)
        {
            switch (chartType)
            {
                case "Column": return SeriesChartType.Column;
                case "Pie": return SeriesChartType.Pie;
                case "Line": return SeriesChartType.Line;
                default: return SeriesChartType.Column;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void btnExportToWord_Click(object sender, RoutedEventArgs e)
        {
            ExportToWord();
        }

        private void btnExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel();
        }

        private void ExportToWord()
        {
            try
            {
                string reportsDir = @"D:\Reports";
                Directory.CreateDirectory(reportsDir);
                string filePath = Path.Combine(reportsDir, $"SalesReport_{DateTime.Now:yyyyMMdd_HHmmss}.docx");

                var data = GetExportData();
                if (data.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта", "Информация",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var wordApp = new Microsoft.Office.Interop.Word.Application();
                var document = wordApp.Documents.Add();

                // Добавление заголовка
                var paragraph = document.Paragraphs.Add();
                paragraph.Range.Text = "Отчет о продажах салона красоты";
                paragraph.Range.Font.Bold = 1;
                paragraph.Range.Font.Size = 16;
                paragraph.Format.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                paragraph.Range.InsertParagraphAfter();

                // Добавление таблицы с данными
                var table = document.Tables.Add(paragraph.Range, data.Rows.Count + 1, data.Columns.Count);
                table.Borders.Enable = 1;

                // Заголовки столбцов
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    table.Cell(1, i + 1).Range.Text = data.Columns[i].ColumnName;
                    table.Cell(1, i + 1).Range.Font.Bold = 1;
                }

                // Данные
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        table.Cell(i + 2, j + 1).Range.Text = data.Rows[i][j].ToString();
                    }
                }

                // Сохранение и закрытие
                document.SaveAs2(filePath);
                document.Close();
                wordApp.Quit();

                MessageBox.Show($"Отчет успешно сохранен: {filePath}", "Успешно",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта в Word: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToExcel()
        {
            try
            {
                string reportsDir = @"D:\Reports";
                Directory.CreateDirectory(reportsDir);
                string filePath = Path.Combine(reportsDir, $"SalesReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

                var excelApp = new Excel.Application();
                var workbook = excelApp.Workbooks.Add();
                var worksheet = (Excel.Worksheet)workbook.Sheets[1];

                // Заголовок
                worksheet.Cells[1, 1] = "Отчет о продажах салона красоты";
                var headerRange = worksheet.Range["A1:D1"];
                headerRange.Merge();
                headerRange.Font.Bold = true;
                headerRange.Font.Size = 14;
                headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                // Таблица с данными
                var data = GetExportData();
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    worksheet.Cells[3, i + 1] = data.Columns[i].ColumnName;
                    ((Excel.Range)worksheet.Cells[3, i + 1]).Font.Bold = true;
                }

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 4, j + 1] = data.Rows[i][j];
                    }
                }

                // Диаграмма
                var chartObjects = (Excel.ChartObjects)worksheet.ChartObjects();
                var chartObject = chartObjects.Add(100, 100, 400, 300);
                var chartExcel = chartObject.Chart;
                chartExcel.ChartType = GetExcelChartType((cbChartType.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Column");
                chartExcel.SetSourceData(worksheet.Range[$"A3:D{data.Rows.Count + 3}"]);
                chartExcel.HasTitle = true;
                chartExcel.ChartTitle.Text = $"Продажи по {(currentGroupBy == "Employee" ? "сотрудникам" : "услугам")}";

                workbook.SaveAs(filePath);
                workbook.Close();
                excelApp.Quit();

                MessageBox.Show($"Отчет сохранен: {filePath}", "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта в Excel: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private System.Data.DataTable GetExportData()
        {
            var data = new System.Data.DataTable();
            data.Columns.Add("Дата");
            data.Columns.Add(currentGroupBy == "Employee" ? "Сотрудник" : "Услуга");
            data.Columns.Add("Количество");
            data.Columns.Add("Сумма", typeof(decimal));

            try
            {
                
                var query = context.Sales
                    .Include(s => s.Employees)  
                    .Include(s => s.Services)   
                    .OrderBy(s => s.SaleDate)
                    .AsNoTracking()
                    .ToList();

                foreach (var sale in query)
                {
                    data.Rows.Add(
                        sale.SaleDate.ToString("dd.MM.yyyy HH:mm"),
                        currentGroupBy == "Employee"
                            ? sale.Employees?.FirstName + " " + sale.Employees?.LastName
                            : sale.Services?.ServiceName,
                        sale.QuantitySold,
                        sale.Services?.Price * sale.QuantitySold ?? 0
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подготовки данных для экспорта: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return data;
        }

        private Excel.XlChartType GetExcelChartType(string chartType)
        {
            switch (chartType)
            {
                case "Column": return Excel.XlChartType.xlColumnClustered;
                case "Pie": return Excel.XlChartType.xlPie;
                case "Line": return Excel.XlChartType.xlLine;
                default: return Excel.XlChartType.xlColumnClustered;
            }
        }
    }
}