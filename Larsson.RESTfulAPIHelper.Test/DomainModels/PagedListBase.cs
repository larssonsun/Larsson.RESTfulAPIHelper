using System.Collections.Generic;
using Larsson.RESTfulAPIHelper.Pagination;

namespace Larsson.RESTfulAPIHelper.Test.DomainModel
{
    public class PagedListBase<T> : PaginatedList<T> where T : class
    {
        public PagedListBase(int pageIndex, int pageSize, int totalItemsCount, IEnumerable<T> data) : base(pageIndex, pageSize, totalItemsCount, data)
        {
        }
    }
}