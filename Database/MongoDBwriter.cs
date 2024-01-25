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
using EscapeFromTheWoods.Database.exceptions;

namespace EscapeFromTheWoods.Database
{
    public class MongoDBwriter
    {
        public MongoDBwriter(string connectionString)
        {
            dbClient = new MongoClient(connectionString);
            database = dbClient.GetDatabase("EscapeTheWoods");
        }
        private IMongoClient dbClient;
        private IMongoDatabase database;

        public void WriteWood(WoodModel wood)
        {
            Console.Out.WriteLine($"writeing wood id {wood.Id}");
            var woodCollection = database.GetCollection<BsonDocument>("FullWoods");
            woodCollection.InsertOne(wood.GenerateBson());
            Console.Out.WriteLine($"finiched wood id {wood.Id}");
        }
        public async Task WriteRouteAsync(RouteModel route)
        {
            Console.Out.WriteLine($"writing wood id {route.Id}");
            var pathCollection = database.GetCollection<BsonDocument>("Route");
            pathCollection.InsertOne(route.GenerateBson());
            Console.Out.WriteLine($"finiched wood id {route.Id}");
        }



        //public async Task WritePathAsync(PathModel path)
        //{
        //    var pathCollection = database.GetCollection<BsonDocument>("FullPath");
        //    pathCollection.InsertOne(path.GenerateBson());
        //}
        
        //public void WriteMonkey(MonkeyModel monkey)
        //{
        //    var monkeyCollection = database.GetCollection<BsonDocument>("monkey");
        //    monkeyCollection.InsertOne(monkey.GenerateBson());
        //}
        //public async void WriteTree(TreeModel tree)
        //{
        //    try
        //    {
        //        var treeCollection = database.GetCollection<BsonDocument>("tree");
        //        treeCollection.InsertOne(tree.GenerateBson());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new GeneralException("WriteTree", ex);
        //    }
        //}
        //public async void WriteTrees(List<TreeModel> trees) 
        //{
        //    try
        //    {
        //        var treeCollection = database.GetCollection<BsonDocument>("tree");
        //        treeCollection.InsertMany(await GeneratBsonTreesAsync(trees));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new GeneralException("WriteTrees", ex);
        //    }
        //}
        //private async Task<List<BsonDocument>> GeneratBsonTreesAsync(List<TreeModel> trees)
        //{
        //    try
        //    {
        //        List<Task> tasks = new List<Task>();
        //        List<BsonDocument> bsons = new List<BsonDocument>();
        //        foreach (TreeModel tree in trees)
        //        {
        //            tasks.Add(AddAndMakeBsonTreeAsync(bsons, tree));
        //        }
        //        await Task.WhenAll(tasks);
        //        return bsons;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        //private async Task AddAndMakeBsonTreeAsync(List<BsonDocument> bsons, TreeModel tree)
        //{
        //    try
        //    {
        //        bsons.Add(tree.GenerateBson());
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
    }
}
