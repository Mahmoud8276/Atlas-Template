
namespace Atlas.Template.Core.Dtos
{
    public class Pagination
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public object Data { get; set; }

        public Pagination(int pageIndex, int pageSize, int count, object data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;
        }
    }
}
