using Atlas.Template.Core.Interfaces.ISpecificationParams;

namespace Atlas.Template.Services.SpecificationParams
{
    public class UserSpecParams : BaseSpecParams, IUserSpecParams
    {
        public string? UserName { get; set; }
    }
}
