
using Larsson.RESTfulAPIHelper.Pagination;

namespace Larsson.RESTfulAPIHelper.Test.DTO
{
    public class ProductQueryDTO: PaginationBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}