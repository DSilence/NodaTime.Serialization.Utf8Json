using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<OffsetDateTimeBenchmark>(DefaultConfig.Instance
                .With(Job.Default.With(CsProjCoreToolchain.NetCoreApp22)));
        }
    }
}
