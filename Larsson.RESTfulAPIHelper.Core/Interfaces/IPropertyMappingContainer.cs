namespace Larsson.RESTfulAPIHelper.Interface
{
    public interface IPropertyMappingContainer
    {
        IPropertyMapping Resolve<TSource, TDestination>();
        bool ValidMappingExistsFor<TSource, TDestination>(string fields);
    }
}