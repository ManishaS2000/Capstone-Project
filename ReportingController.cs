using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using OfficeOpenXml;

namespace recyclecollection.Controllers
{
    public class ReportingController : Controller
    {
        private readonly IConfiguration _configuration;

        public ReportingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Action to fetch waste data for Pie Chart
        public async Task<IActionResult> GetWasteByType()
        {
            var wasteData = new List<WasteByType>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("dbo.Sp_WasteByType", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            wasteData.Add(new WasteByType
                            {
                                WasteType = reader["WasteType"]?.ToString(), // Safely get string
                                TotalWeight = reader.IsDBNull(reader.GetOrdinal("TotalWeight"))
                                    ? 0 // Default to 0 if null
                                    : Convert.ToInt32(reader["TotalWeight"]) // Convert only if not null
                            });
                        }
                    }
                }
            }

            return Json(wasteData);
        }
        public IActionResult PieChart()
        {
            return View();
        }
        // Action to fetch recyclables weight by type per month
        public async Task<IActionResult> GetRecyclablesWeightByMonth()
        {
            var recyclablesData = new List<RecyclableWeightByMonth>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("Sp_RecyclablesWeightByMonth", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            recyclablesData.Add(new RecyclableWeightByMonth
                            {
                                Month = reader["Month"].ToString(),
                                Category = reader["Category"].ToString(),
                                TotalWeight = Convert.ToInt32(reader["TotalWeight"])
                            });
                        }
                    }
                }
            }

            return Json(recyclablesData);
        }
        public IActionResult ReportGraphs()
        {
            return View(); // This will look for a "ReportGraphs.cshtml" view in the Views/Reporting folder
        }

        // Action to fetch monthly landfill, food waste, and recycling weights
        public async Task<IActionResult> GetMonthlyTotalWeights()
        {
            var totalWeightsData = new List<MonthlyTotalWeights>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("Sp_TotalWeightsByMonth", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            totalWeightsData.Add(new MonthlyTotalWeights
                            {
                                Month = reader["MonthName"].ToString(),
                                LandfillWeight = Convert.ToInt32(reader["LandfillWeight"]),
                                FoodWasteWeight = Convert.ToInt32(reader["FoodWasteWeight"]),
                                RecyclingWeight = Convert.ToInt32(reader["RecyclingWeight"])
                            });
                        }
                    }
                }
            }

            return Json(totalWeightsData);
        }

        // Action to fetch diversion rate per month
        public async Task<IActionResult> GetDiversionRate()
        {
            var diversionRateData = new List<DiversionRateData>();

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("dbo.Sp_DiversionRate", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int monthNumber = Convert.ToInt32(reader["Month"]);
                                string monthName = GetMonthName(monthNumber); // Convert month number to month name

                                diversionRateData.Add(new DiversionRateData
                                {
                                    Month = monthNumber, // Keep it as an integer
                                    DiversionRate = reader.GetDecimal(reader.GetOrdinal("DiversionRate"))
                                });
                            }
                        }
                    }
                }

                return Json(diversionRateData);
            }
            catch (Exception ex)
            {
                // Handle the error, possibly log it
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GetMonthName(int monthNumber)
        {
            var months = new[]
            {
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    };

            return months[monthNumber - 1]; // Convert month number (1-12) to month name
        }



        // Action to fetch revenue per month
        public async Task<IActionResult> GetRevenueByMonth()
        {
            var revenueData = new List<RevenueData>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("dbo.Sp_RevenueByMonth", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            revenueData.Add(new RevenueData
                            {
                                Month = Convert.ToInt32(reader["Month"]),
                                TotalRevenue = Convert.ToDecimal(reader["TotalRevenue"])
                            });
                        }
                    }
                }
            }

            return Json(revenueData);
        }
        public async Task<IActionResult> ExportWasteDiversionToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var diversionSummary = new List<WasteDiversionSummary>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("Sp_WasteDiversionSummary", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            diversionSummary.Add(new WasteDiversionSummary
                            {
                                MonthYear = reader["MonthYear"].ToString(),
                                LandfillLbs = Convert.ToInt32(reader["LandfillLbs"]),
                                CompostLbs = Convert.ToInt32(reader["CompostLbs"]),
                                CardboardLbs = Convert.ToInt32(reader["CardboardLbs"]),
                                PaperLbs = Convert.ToInt32(reader["PaperLbs"]),
                                PlasticLbs = Convert.ToInt32(reader["PlasticLbs"]),
                                AluminumLbs = Convert.ToInt32(reader["AluminumLbs"]),
                                MetalLbs = Convert.ToInt32(reader["MetalLbs"]),
                                GlassLbs = Convert.ToInt32(reader["GlassLbs"]),
                                TotalRecycledLbs = Convert.ToInt32(reader["TotalRecycledLbs"]),
                                TotalWasteGeneratedLbs = Convert.ToInt32(reader["TotalWasteGeneratedLbs"]),
                                TotalWasteDivertedLbs = Convert.ToInt32(reader["TotalWasteDivertedLbs"]),
                                WasteDiversionRate = Convert.ToDecimal(reader["WasteDiversionRate"])
                            });
                        }
                    }
                }
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Waste Diversion Summary");

                // Add headers
                worksheet.Cells[1, 1].Value = "Month-Year";
                worksheet.Cells[1, 2].Value = "Landfill (lbs)";
                worksheet.Cells[1, 3].Value = "Compost (lbs)";
                worksheet.Cells[1, 4].Value = "Cardboard (lbs)";
                worksheet.Cells[1, 5].Value = "Paper (lbs)";
                worksheet.Cells[1, 6].Value = "Plastic (lbs)";
                worksheet.Cells[1, 7].Value = "Aluminum (lbs)";
                worksheet.Cells[1, 8].Value = "Metal (lbs)";
                worksheet.Cells[1, 9].Value = "Glass (lbs)";
                worksheet.Cells[1, 10].Value = "Total Recycled (lbs)";
                worksheet.Cells[1, 11].Value = "Total Waste Generated (lbs)";
                worksheet.Cells[1, 12].Value = "Total Waste Diverted (lbs)";
                worksheet.Cells[1, 13].Value = "Waste Diversion Rate (%)";

                // Add data rows
                for (int i = 0; i < diversionSummary.Count; i++)
                {
                    var row = i + 2;
                    var record = diversionSummary[i];

                    worksheet.Cells[row, 1].Value = record.MonthYear;
                    worksheet.Cells[row, 2].Value = record.LandfillLbs;
                    worksheet.Cells[row, 3].Value = record.CompostLbs;
                    worksheet.Cells[row, 4].Value = record.CardboardLbs;
                    worksheet.Cells[row, 5].Value = record.PaperLbs;
                    worksheet.Cells[row, 6].Value = record.PlasticLbs;
                    worksheet.Cells[row, 7].Value = record.AluminumLbs;
                    worksheet.Cells[row, 8].Value = record.MetalLbs;
                    worksheet.Cells[row, 9].Value = record.GlassLbs;
                    worksheet.Cells[row, 10].Value = record.TotalRecycledLbs;
                    worksheet.Cells[row, 11].Value = record.TotalWasteGeneratedLbs;
                    worksheet.Cells[row, 12].Value = record.TotalWasteDivertedLbs;
                    worksheet.Cells[row, 13].Value = record.WasteDiversionRate;
                }

                // Format as table
                var range = worksheet.Cells[1, 1, diversionSummary.Count + 1, 13];
                var table = worksheet.Tables.Add(range, "WasteDiversionTable");
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Return file
                var excelData = package.GetAsByteArray();
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "WasteDiversionSummary.xlsx");
            }
        }
        public IActionResult AnnualSummary()
        {
            return View(); // Redirects to Views/Reporting/AnnualSummary.cshtml
        }
    }
}
   




// Data model for waste type chart
public class WasteByType
    {
        public string WasteType { get; set; }
        public int TotalWeight { get; set; }
    }

    // Data model for recyclables by month
    public class RecyclableWeightByMonth
    {
        public string Month { get; set; }
        public string Category { get; set; }
        public int TotalWeight { get; set; }
    }

    // Data model for monthly waste weights
    public class MonthlyTotalWeights
    {
        public string Month { get; set; }
        public int LandfillWeight { get; set; }
        public int FoodWasteWeight { get; set; }
        public int RecyclingWeight { get; set; }
    }

    // Data model for diversion rate
    public class DiversionRateData
    {
        public int Month { get; set; }
        public decimal DiversionRate { get; set; }
    }

    // Data model for revenue data
    public class RevenueData
    {
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
    }
public class WasteDiversionSummary
{
    public string MonthYear { get; set; }
    public int LandfillLbs { get; set; }
    public int CompostLbs { get; set; }
    public int CardboardLbs { get; set; }
    public int PaperLbs { get; set; }
    public int PlasticLbs { get; set; }
    public int AluminumLbs { get; set; }
    public int MetalLbs { get; set; }
    public int GlassLbs { get; set; }
    public int TotalRecycledLbs { get; set; }
    public int TotalWasteGeneratedLbs { get; set; }
    public int TotalWasteDivertedLbs { get; set; }
    public decimal WasteDiversionRate { get; set; }
}


