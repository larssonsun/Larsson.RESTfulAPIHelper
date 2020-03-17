using System.Threading.Tasks;
using Larsson.RESTfulAPIHelper.Test.DomainModel;
using Larsson.RESTfulAPIHelper.Test.Entity;

namespace Larsson.RESTfulAPIHelper.Test.Interface
{
    public interface IProductRepository
    {
        Task<PagedListBase<Product>> GetProducts(ProductQuery parm);
    }
}