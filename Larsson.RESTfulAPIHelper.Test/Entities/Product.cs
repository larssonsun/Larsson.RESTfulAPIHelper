using System;

namespace Larsson.RESTfulAPIHelper.Test.Entity
{

    public class Product : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsOnSale { get; set; }
        public DateTime CreateTime { get; set; }
    }
}