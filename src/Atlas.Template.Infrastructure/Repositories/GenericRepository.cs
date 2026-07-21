using Atlas.Template.Core.Interfaces.IRepositories;
using Atlas.Template.Core.Models;
using Atlas.Template.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Template.Infrastructure.Repositories
{
    public class GenericRepository<TModel, TKey> : IGenericRepository<TModel, TKey> where TModel : BaseModel<TKey>
    {
        private readonly AppDbContext _context;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<TModel>> GetAllAsync()
        {
            return await _context.Set<TModel>().ToListAsync();
        }

        public async Task<TModel?> GetByIdAsync(TKey id)
        {
            return await _context.Set<TModel>().FindAsync(id);
        }


        public async Task AddAsync(TModel model)
        {
            await _context.Set<TModel>().AddAsync(model);
        }

        public async Task AddRangeAsync(IEnumerable<TModel> models)
        {
            await _context.Set<TModel>().AddRangeAsync(models);
        }


        public void DeleteAsync(TModel model)
        {
            _context.Set<TModel>().Remove(model);
        }
        
        public void DeleteRangeAsync(IEnumerable<TModel> models)
        {
            _context.Set<TModel>().RemoveRange(models);
        }


        public void UpdateAsync(TModel model)
        {
            _context.Set<TModel>().Update(model);
        }
    }
}
