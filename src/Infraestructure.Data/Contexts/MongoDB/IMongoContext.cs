using MongoDB.Driver;

namespace Infraestructure.Data.Contexts.MongoDB
{
    interface IMongoContext
    {
        IMongoDatabase Database { get; }
        IMongoClient Client { get; }
    }
}
