using System;
using System.Collections.Generic;
using System.Text;

namespace AuthSample.Domain
{
    public class Product : PersistentEntity
    {
        public string Name { get; set; }
        public int Qty { get; set; }
    }
}
