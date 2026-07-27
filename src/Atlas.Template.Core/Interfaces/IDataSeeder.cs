using System.Threading;
using System.Threading.Tasks;

namespace Atlas.Template.Core.Interfaces
{
    public interface IDataSeeder
    {
        public int Order { get; }

        public Task SeedAsync(CancellationToken token = default);
    }
}
