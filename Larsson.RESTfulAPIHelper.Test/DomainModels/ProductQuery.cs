using Larsson.RESTfulAPIHelper.Pagination;

namespace Larsson.RESTfulAPIHelper.Test.DomainModel
{
    public class ProductQuery : PaginationBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}