using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleQueryBenchmark
{
    //[HtmlExporter]
    //[CsvExporter]
    [SqlExporter]
    //[SimpleJob(RunStrategy.ColdStart, launchCount: 1, warmupCount: 5, targetCount: 5, id: "SqlBenchmarkJob")]
    [SimpleJob(RunStrategy.Monitoring, targetCount: 10, id: "MonitoringJob")]
    [MinColumn, Q1Column, Q3Column, MaxColumn]
    public class QueryBenchmark
    {
        public IEnumerable<string> ConnectionStringNames()
        {
            return ConfigurationManager.ConnectionStrings
                .OfType<ConnectionStringSettings>()
                //.Where(i => i.Name.StartsWith("HRP"))
                .Select(i => i.Name)
                ;
        }

        public IEnumerable<string> SqlQueries()
        {
            return Directory.GetFiles("SqlQueries", "*.sql").Select(i => Path.GetFileName(i));

            //return new string[]
            //    {
            //        "SELECT TOP (1000) *  FROM [dbo].[Activity]"
            //    };
        }


        [ParamsSource(nameof(ConnectionStringNames))]
        public string Database;

        [ParamsSource(nameof(SqlQueries))]
        public string SqlQuery;



        [Benchmark]
        public void ExecuteQueryBenchmark()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[Database]
                .ConnectionString;

            var filePath = Path.Combine("SqlQueries", SqlQuery);
            string sql = File.ReadAllText(filePath);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    //Console.WriteLine(SqlQuery);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //Console.Write(".");
                        }
                    }
                }
                connection.Close();
            }
        }
    }
}