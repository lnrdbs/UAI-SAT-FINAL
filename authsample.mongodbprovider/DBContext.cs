using AuthSample.Domain;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace AuthSample.MongoDBProvider
{
    public interface IDbContext<T> where T : PersistentEntity
    {

        IEnumerable<T> GetAll();
        T Get(Guid id);
        T Save(T entity);
        void Remove(Guid id);

    }


    public class DbContext<T> : IDbContext<T> where T : PersistentEntity
    {
        IMongoDatabase _db;


        MongoClient _client;

        string _name;
        public DbContext()
        {
            _client = new MongoClient("mongodb://admin:AIeR8E7IfNhWQivP@cluster0-shard-00-00-ghu5n.gcp.mongodb.net:27017,cluster0-shard-00-01-ghu5n.gcp.mongodb.net:27017,cluster0-shard-00-02-ghu5n.gcp.mongodb.net:27017/test?ssl=true&replicaSet=Cluster0-shard-0&authSource=admin&retryWrites=true");
            _db = _client.GetDatabase("mydb");
     
            _name = typeof(T).Name.ToString();


        }

        public T Get(Guid id)
        {
            var filter = Builders<T>.Filter.Eq(s => s.Id, id);
            var result = _db.GetCollection<T>(_name).Find<T>(filter);

            return result.FirstOrDefault();

        }

        public IEnumerable<T> GetAll()
        {
            return _db.GetCollection<T>(_name).AsQueryable<T>();
        }

        public void Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        public T Save(T entity)
        {
            if (entity.Id.Equals(Guid.Empty))
            {
                _db.GetCollection<T>(_name).InsertOne(entity);
            }
            else
            {

                var filter = Builders<T>.Filter.Eq(s => s.Id, entity.Id);
                var result = _db.GetCollection<T>(_name).ReplaceOne(filter, entity);


            }

            return entity;
        }
    }
}
