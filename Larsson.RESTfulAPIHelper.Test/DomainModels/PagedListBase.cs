using System.Collections.Generic;
using Larsson.RESTfulAPIHelper.Pagination;
using MessagePack;

namespace Larsson.RESTfulAPIHelper.Test.DomainModel
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class PagedListBase<T> : PaginatedList<T> where T : class
    {
        public PagedListBase(int pageIndex, int pageSize, int totalItemsCount, IEnumerable<T> data) : base(pageIndex, pageSize, totalItemsCount, data)
        {
        }
    }
}