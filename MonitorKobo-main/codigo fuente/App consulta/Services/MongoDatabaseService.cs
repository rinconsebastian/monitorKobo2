using App_consulta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksApi.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace App_consulta.Services
{
    public class MongoDatabaseService
    {

        private readonly IMongoDatabase database;

        public MongoDatabaseService(IMongoDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            database = client.GetDatabase(settings.DatabaseName);
        }

        public async Task<KoGenericData> Find(String collectionName, String id)
        {
            var collection = database.GetCollection<KoGenericData>(collectionName);
            return await collection.Find(n => n.Id == id).FirstOrDefaultAsync();
        }


        public async Task<KoExtendData> FindExt(String collectionName, String id)
        {
            var collection = database.GetCollection<KoExtendData>(collectionName);
            return await collection.Find(n => n.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<KoGenericData>> FindMany(String collectionName, string[] ids)
        {
            var collection = database.GetCollection<KoGenericData>(collectionName);
            return await collection.Find(n => ids.Contains(n.Id)).ToListAsync();
        }

       
        public async Task<KoDataViewModel> FindViewModel(String collectionName, String id)
        {
            var collection = database.GetCollection<KoDataViewModel>(collectionName);
            return await collection.Find(n => n.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<KoDataViewModel>> FindManyViewModel(String collectionName, string[] ids)
        {
            var collection = database.GetCollection<KoDataViewModel>(collectionName);
            return await collection.Find(n => ids.Contains(n.Id)).ToListAsync();
        }

        public async Task<List<KoGenericData>> Get(String collectionName)
        {
            var collection = database.GetCollection<KoGenericData>(collectionName);
            var list = await collection.Find(n => true).ToListAsync();
            return list;
        }

        public async Task<List<BsonDocument>> GetWithFilter(String collectionName, List<string> fieldList,  FilterDefinition<BsonDocument> filter, bool ExcludeId = false)
        {
            var result = new List<BsonDocument>();

            if(fieldList.Count == 0) { return result; }

            var collection = database.GetCollection<BsonDocument>(collectionName);

            var projection = Builders<BsonDocument>.Projection.Include(fieldList.First());
            foreach (var field in fieldList.Skip(1))
            {
                projection = projection.Include(field);
            }
            if (ExcludeId) { projection = projection.Exclude("_id"); }
            result = await collection.Find(filter).Project(projection).ToListAsync();
            return result;
        }

        public async Task<List<BsonDocument>> CountByUser(String collectionName)
        {
            var collection = database.GetCollection<BsonDocument>(collectionName);
            var result = await collection.Aggregate().Group(new BsonDocument
              {
                   { "_id", "$user" },
                   {"count", new BsonDocument("$sum", 1)}
              }
            ).ToListAsync();
            return result;
        }

        public async Task<string> MaxIdKobo(String collectionName)
        {
            string max = null;
            var collection = database.GetCollection<KoGenericData>(collectionName);
            var result = await collection.Find(n => true).SortByDescending(n => n.IdKobo).Limit(1).FirstOrDefaultAsync();
            if(result != null) { max = result.IdKobo; }
            return max;
        }

        public async void InsertMany(String collectionName, List<KoGenericData> data)
        {
            if (data.Count > 0)
            {
                var collection = database.GetCollection<KoGenericData>(collectionName);
                await collection.InsertManyAsync(data);
            }
        }


        public async Task<bool> Replace(String collectionName,  KoExtendData dataIn)
        {
            var collection = database.GetCollection<KoExtendData>(collectionName);
            var actionResult = await collection.ReplaceOneAsync(n => n.Id == dataIn.Id, dataIn);
            return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
        }

        public async Task<bool> ReplaceGeneric(String collectionName, KoGenericData dataIn)
        {
            var collection = database.GetCollection<KoGenericData>(collectionName);
            var actionResult = await collection.ReplaceOneAsync(n => n.Id == dataIn.Id, dataIn);
            return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
        }

        public async Task<bool> Update(String collectionName, UpdateDefinition<KoExtendData> update, FilterDefinition<KoExtendData> filter)
        {
            var collection = database.GetCollection<KoExtendData>(collectionName);
            var actionResult = await collection.UpdateOneAsync(filter, update);
            return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
        }

        public async Task<int> DeleteMany (String collectionName, string[] ids)
        {
            var idsFilter = Builders<KoGenericData>.Filter.In(d => d.Id, ids);
            var collection = database.GetCollection<KoGenericData>(collectionName);
            var actionResult = await collection.DeleteManyAsync(idsFilter);
            return (int)actionResult.DeletedCount;
        }

    }
}
