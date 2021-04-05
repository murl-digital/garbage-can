using Swashbuckle.AspNetCore.Annotations;

namespace GarbageCan.Web.Filters
{
    public class PaginationFilter
    {
        [SwaggerParameter("Which page to display. 1-based, with a default value of 1")]
        public int PageNumber { get; set; }
        [SwaggerParameter("How many member entries per page. The default is 10 and the maximum is 20.")]
        public int PageSize { get; set; }
        
        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        
        public PaginationFilter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 20 ? 20 : pageSize;
        }

        public void Deconstruct(out int pageNumber, out int pageSize)
        {
            pageNumber = PageNumber;
            pageSize = PageSize;
        }
    }
}