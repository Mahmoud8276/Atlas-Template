using Atlas.Template.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Atlas.Template.Infrastructure.DataSeeders
{
    public class DataSeedersRunner
    {
        private readonly IEnumerable<IDataSeeder> _seeders;
        private readonly ILogger<DataSeedersRunner> _logger;

        public DataSeedersRunner(IEnumerable<IDataSeeder> seeders, ILogger<DataSeedersRunner> logger)
        {
            _seeders = seeders;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            foreach (var seeder in _seeders.OrderBy(s => s.Order))
            {
                _logger.LogInformation("Running seeder: {Seeder}", seeder.GetType().Name);
                await seeder.SeedAsync(cancellationToken);
            }
        }
    }
}
