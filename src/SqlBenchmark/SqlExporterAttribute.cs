using BenchmarkDotNet.Attributes;

namespace SimpleQueryBenchmark
{
    public class SqlExporterAttribute : ExporterConfigBaseAttribute
    {
        public SqlExporterAttribute() : base(new SqlExporter())
        {
        }
    }
}
