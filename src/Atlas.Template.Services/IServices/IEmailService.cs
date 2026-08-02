using Atlas.Template.Core.Interfaces;
using System.Threading.Tasks;

namespace Atlas.Template.Services.IServices
{
    public interface IEmailService
    {
        public Task SendAsync(IEmailStructure email);
    }
}
