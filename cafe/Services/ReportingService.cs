using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel; // библиотека для работы с Excel
using Npgsql;
using RestaurantApp.Models;

namespace RestaurantApp.Services
{
    public class ReportingService
    {
        private readonly string _connectionString;

        public ReportingService(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Генерация отчета по финансовому состоянию за определенный временной промежуток.
        /// </summary>
        /// <param name="startDate">Начальная дата временного промежутка.</param>
        /// <param name="endDate">Конечная дата временного промежутка.</param>
        /// <returns>Список финансовых отчетов за указанный период.</returns>
        public async Task<List<FinancialReport>> GenerateReportAsync(DateTime startDate, DateTime endDate)
        {
            var reports = new List<FinancialReport>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmd = new NpgsqlCommand(
                    @"SELECT * FROM financial_reports 
                      WHERE data_range_start >= @StartDate AND data_range_end <= @EndDate",
                    conn
                );

                cmd.Parameters.AddWithValue("@StartDate", startDate.Date);
                cmd.Parameters.AddWithValue("@EndDate", endDate.Date);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        reports.Add(new FinancialReport
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ReportType = reader.GetString(reader.GetOrdinal("report_type")),
                            ShiftId = reader.IsDBNull(reader.GetOrdinal("shift_id")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("shift_id")),
                            GeneratedBy = reader.IsDBNull(reader.GetOrdinal("generated_by")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("generated_by")),
                            DataRangeStart = reader.GetDateTime(reader.GetOrdinal("data_range_start")),
                            DataRangeEnd = reader.GetDateTime(reader.GetOrdinal("data_range_end")),
                            TotalRevenue = reader.GetDecimal(reader.GetOrdinal("total_revenue")),
                            CashRevenue = reader.GetDecimal(reader.GetOrdinal("cash_revenue")),
                            CardRevenue = reader.GetDecimal(reader.GetOrdinal("card_revenue")),
                            TotalOrders = reader.GetInt32(reader.GetOrdinal("total_orders"))
                        });
                    }
                }
            }

            return reports;
        }

        /// <summary>
        /// Экспортирует список финансовых отчетов в файл формата Excel.
        /// </summary>
        /// <param name="reports">Список отчетов для экспорта.</param>
        /// <param name="outputFilePath">Путь к выходному файлу Excel.</param>
        public async Task ExportToExcelAsync(List<FinancialReport> reports, string outputFilePath)
        {
            if (reports.Count == 0 || string.IsNullOrEmpty(outputFilePath))
            {
                throw new ArgumentException("Недостаточно данных для экспорта.");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Финансовые отчёты");

                // Создаем шапку таблицы
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Тип отчета";
                worksheet.Cell(1, 3).Value = "Смена";
                worksheet.Cell(1, 4).Value = "Сформировал";
                worksheet.Cell(1, 5).Value = "Начало периода";
                worksheet.Cell(1, 6).Value = "Окончание периода";
                worksheet.Cell(1, 7).Value = "Общая выручка";
                worksheet.Cell(1, 8).Value = "Наличные";
                worksheet.Cell(1, 9).Value = "Безналичные";
                worksheet.Cell(1, 10).Value = "Всего заказов";

                // Заполняем строки
                for (int i = 0; i < reports.Count; i++)
                {
                    var rowIndex = i + 2; // Начинаем с второй строки
                    var report = reports[i];

                    worksheet.Cell(rowIndex, 1).Value = report.Id;
                    worksheet.Cell(rowIndex, 2).Value = report.ReportType;
                    worksheet.Cell(rowIndex, 3).Value = report.ShiftId.HasValue ? report.ShiftId.Value.ToString() : "";
                    worksheet.Cell(rowIndex, 4).Value = report.GeneratedBy.HasValue ? report.GeneratedBy.Value.ToString() : "";
                    worksheet.Cell(rowIndex, 5).Value = report.DataRangeStart;
                    worksheet.Cell(rowIndex, 6).Value = report.DataRangeEnd;
                    worksheet.Cell(rowIndex, 7).Value = report.TotalRevenue;
                    worksheet.Cell(rowIndex, 8).Value = report.CashRevenue;
                    worksheet.Cell(rowIndex, 9).Value = report.CardRevenue;
                    worksheet.Cell(rowIndex, 10).Value = report.TotalOrders;
                }

                // Авторазмер колонок
                foreach (var column in worksheet.Columns())
                {
                    column.AdjustToContents();
                }

                // Сохраняем файл
                await workbook.SaveAs(outputFilePath);
            }
        }
    }
}