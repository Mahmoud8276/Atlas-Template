using Atlas.Template.Core.Interfaces.ISpecificationParams;
using System;

namespace Atlas.Template.Services.SpecificationParams
{
    public class BaseSpecParams : IBaseSpecParams
    {
        private const int MaxPageSize = 15;

        private int _pageSize = 10;

        public int PageIndex { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value <= 0
                    ? 10
                    : Math.Min(value, MaxPageSize);
            }
        }
    }
}
