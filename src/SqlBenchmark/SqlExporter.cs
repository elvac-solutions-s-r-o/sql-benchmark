using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Extensions;
using System.Linq;
using System;
using System.Configuration;

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
            var table = summary.Table;
            //foreach(var row in table.SeparateLogicalGroups)

            //string realSeparator = separator.ToRealSeparator();
            //foreach (var line in summary.GetTable(style).FullContentWithHeader)
            //{
            //    for (int i = 0; i < line.Length;)
            //    {
            //        logger.Write(CsvHelper.Escape(line[i], realSeparator));

            //        if (++i < line.Length)
            //        {
            //            logger.Write(realSeparator);
            //        }
            //    }

            //    logger.WriteLine();
            //}
        }
    }
}
