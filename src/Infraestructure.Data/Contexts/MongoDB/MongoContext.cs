using MongoDB.Driver;

namespace Infraestructure.Data.Contexts.MongoDB
{
    sealed class MongoContext : IMongoContext
    {
        public IMongoDatabase Database { get; }

        public IMongoClient Client { get; }

        public MongoContext(IMongoClient mongoClient)
        {
            Client = mongoClient;
            Database = Client.GetDatabase("Observability");
        }
    }
}
