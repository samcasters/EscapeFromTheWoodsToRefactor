using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MongoDB.Driver;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using System.IO;
using EscapeFromTheWoods.Database.Model;

namespace EscapeFromTheWoods.Database
{
    public class MongoDBwriter
    {
        //collection.InsertOne(document);
        //await collection.InsertOneAsync(document);


        public MongoDBwriter(string connectionString)
        {
            this.connectionString = connectionString;
            dbClient = new MongoClient(connectionString);
            database = dbClient.GetDatabase("EscapeTheWoods");
        }
        private IMongoClient dbClient;
        private IMongoDatabase database;
        private string connectionString;
        
        //public async Task WriteWoodRecord()
        //{
        //    var pathCollection = database.GetCollection<BsonDocument>("path");
        //    var monkeyCollection = database.GetCollection<BsonDocument>("monkey");
        //    var treeCollection = database.GetCollection<BsonDocument>("tree");
        //    var woodCollection = database.GetCollection<BsonDocument>("wood");
        //}

        public void WritePath(PathModel path)
        {
            var pathCollection = database.GetCollection<BsonDocument>("path");
            pathCollection.InsertOne(path.GenerateBson());

        }
    }
}
