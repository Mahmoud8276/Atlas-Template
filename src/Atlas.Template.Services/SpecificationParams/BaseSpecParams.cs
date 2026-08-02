using Atlas.Template.Core.Interfaces.ISpecificationParams;

namespace Atlas.Template.Services.SpecificationParams
{
    public class BaseSpecParams : IBaseSpecParams
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
