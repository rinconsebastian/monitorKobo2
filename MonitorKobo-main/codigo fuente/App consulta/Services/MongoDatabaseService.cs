using App_consulta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksApi.Models;
using MongoDB.Driver;

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

        public async Task<List<KoGenericData>> Get(String collectionName)
        {
            var collection = database.GetCollection<KoGenericData>(collectionName);
            var list = await collection.Find(n => true).ToListAsync();
            return list;
        }

        public  async Task<string> MaxIdKobo(String collectionName)
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
        /*

            public Book Get(string id) =>
                _books.Find<Book>(book => book.Id == id).FirstOrDefault();

            public Book Create(Book book)
            {
                _books.InsertOne(book);
                return book;
            }

            public void Update(string id, Book bookIn) =>
                _books.ReplaceOne(book => book.Id == id, bookIn);

            public void Remove(Book bookIn) =>
                _books.DeleteOne(book => book.Id == bookIn.Id);

            public void Remove(string id) =>
                _books.DeleteOne(book => book.Id == id);

            */
    }
}
