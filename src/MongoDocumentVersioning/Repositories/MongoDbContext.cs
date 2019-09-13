using MongoDB.Driver;

namespace MongoDocumentVersioning.Repositories
{
    public class MongoDbContext
    {
        private readonly MongoClient _mongodbClient;
        private readonly IMongoDatabase _mongodb;
        private readonly string _collectionName;

        public MongoDbContext(string connStr, string databaseName, string collectionName)
        {
            _mongodbClient = new MongoClient(connStr);
            _mongodb = _mongodbClient.GetDatabase(databaseName);
            _collectionName = collectionName;
        }

        public IMongoCollection<T> MongoDbCollection<T>()
        {
            return Mongodb.GetCollection<T>(_collectionName);
        }

        public MongoClient MongodbClient => _mongodbClient;

        public IMongoDatabase Mongodb => _mongodb;
    }

    public class MongoDbContext<T>
    {
        private readonly MongoClient _mongodbClient;
        private readonly IMongoDatabase _mongodb;
        private readonly string _collectionName;
        private readonly IMongoCollection<T> _mongoDbCollection;

        public MongoDbContext(string connStr, string databaseName, string collectionName)
        {
            _mongodbClient = new MongoClient(connStr);
            _mongodb = _mongodbClient.GetDatabase(databaseName);
            _collectionName = collectionName;
            _mongoDbCollection = _mongodb.GetCollection<T>(_collectionName);
        }

        public IMongoCollection<T> MongoDbCollection => _mongoDbCollection;

        public MongoClient MongodbClient => _mongodbClient;

        public IMongoDatabase Mongodb => _mongodb;
    }

}
