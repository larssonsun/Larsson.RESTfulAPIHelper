﻿
using System;
using System.Linq;
using Larsson.RESTfulAPIHelper.Interface;

namespace Larsson.RESTfulAPIHelper.SortAndQuery
{
    public class PropertyMappingContainer : IPropertyMappingContainer
    {
        private RESTfulAPIHelperOptions _options;
        
        public PropertyMappingContainer(RESTfulAPIHelperOptions options)
        {
            _options = options;
        }       

        public IPropertyMapping Resolve<TSource, TDestination>()
        {
            var matchingMapping = _options.PropertyMappings.OfType<PropertyMapping<TSource, TDestination>>().ToList();
            if (matchingMapping.Count == 1)
            {
                return matchingMapping.First();
            }

            throw new Exception($"Cannot find property mapping instance for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = Resolve<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();
                var indexOfFirstSpace = trimmedField.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);
                if (!propertyMapping.MappingDictionary.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}