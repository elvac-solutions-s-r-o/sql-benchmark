using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Extensions;
using System.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Data;
using DevExpress.Spreadsheet;

namespace SimpleQueryBenchmark
{
    public class SqlExporter : ExporterBase
    {
        //public static readonly IExporter Default = new SqlExporter();

        public DateTime DateCreated { get; }

        public string OutputPath
        {
            get
            {
                return ConfigurationManager.AppSettings["SqlExporter.OutputFile"];
            }
        }


        public SqlExporter()
        {
            DateCreated = DateTime.Now;
        }

        public override void ExportToLog(Summary summary, ILogger logger)
        {
            var dataTable = ToDataTable(summary.Table);
            AddToOutputFile(dataTable);
        }

        private void AddToOutputFile(DataTable dataTable)
        {
            var templateFilePath = "OutputTemplate.xlsx";
            var outputFilePath = "Output.xlsx";

            if (!File.Exists(outputFilePath))
            {
                File.Copy(templateFilePath, outputFilePath);
            }

            //var inputFilePath = File.Exists(outputFilePath) ? outputFilePath : templateFilePath;

            //var templateFile = new FileInfo(inputFilePath);
            //var outputFile = new FileInfo(outputFilePath);




            using (Workbook workbook = new Workbook())
            {
                workbook.LoadDocument(outputFilePath);
                Worksheet worksheet = workbook.Worksheets[0];

                worksheet.Columns[0].NumberFormat = "m/d/yy h:mm";

                int rowIndex = worksheet.GetDataRange().BottomRowIndex;
                if (rowIndex == 0)
                {
                    worksheet.Cells[rowIndex, 0].SetValue("Date");
                    int columnIndex = 1;
                    foreach (DataColumn dataColumn in dataTable.Columns)
                    {
                        worksheet.Cells[rowIndex, columnIndex++].SetValue(dataColumn.ColumnName);
                    }
                }
                foreach (DataRow row in dataTable.Rows)
                {
                    rowIndex++;
                    worksheet.Cells[rowIndex, 0].SetValue(DateCreated);
                    int columnIndex = 1;
                    foreach (var item in row.ItemArray)
                    {
                        worksheet.Cells[rowIndex, columnIndex++].SetValue(item);
                    }
                }

                worksheet.Columns.AutoFit(0, dataTable.Columns.Count);
                workbook.SaveDocument(outputFilePath);
            }
        }


        private DataTable ToDataTable(SummaryTable summaryTable)
        {
            DataTable result = new DataTable();

            var summaryColumns = summaryTable.Columns.ToArray();

            foreach (var summaryColumn in summaryColumns)
            {
                var column = new DataColumn();
                column.ColumnName = summaryColumn.Header;
                column.DataType = summaryColumn.OriginalColumn.IsNumeric ? typeof(decimal) : typeof(string);
                result.Columns.Add(column);
            }

            for (int i = 0; i < summaryTable.FullContent.Length; i++)
            {
                var dataRow = result.NewRow();
                var row = summaryTable.FullContent[i];
                for (int j = 0; j < summaryColumns.Length; j++)
                {
                    var summaryColumn = summaryColumns[j];
                    var value = row[j];
                    if (summaryColumn.OriginalColumn.IsNumeric)
                    {
                        //var b = summaryTable.Summary.BenchmarksCases[0];
                        //var x = summaryColumn.OriginalColumn.GetValue(summaryTable.Summary, b);
                        dataRow[summaryColumn.Header] = ToDecimal(value);
                    }
                    else
                    {
                        dataRow[summaryColumn.Header] = value.ToString();
                    }
                }
                result.Rows.Add(dataRow);

            }

            return result;
        }

        private static decimal ToDecimal(object value)
        {
            var str = value.ToString();
            if (str == "NA")
            {
                return 0;
            }

            decimal result = 0;
            var parts = str.Split(' ');

            if (!decimal.TryParse(parts[0].Replace(".", ","), out result))
            {

            }

            return result;
        }
    }
}
