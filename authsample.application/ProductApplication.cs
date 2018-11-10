using AuthSample.Domain;
using AuthSample.MongoDBProvider;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthSample.Repository
{
    public class ProductApplication
    {
        IDbContext<Product> _products;
       

        
        public ProductApplication()
        {
            _products = new DbContext<Product>();
        }

        public IList<Product> GetAll()
        {
            return _products.GetAll().ToList();
        }

        public Product Save(Product product)
        {
           return _products.Save(product);
        }
    }
}
