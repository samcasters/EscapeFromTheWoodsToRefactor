using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace EscapeFromTheWoods.Database.Model
{
    public class WoodModel
    {
        public WoodModel(int sizeX, int sizeY, List<TreeModel> trees)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            Trees = trees;
        }
        public WoodModel(string id, int sizeX, int sizeY, List<TreeModel> trees)
        {
            Id = id;
            SizeX = sizeX;
            SizeY = sizeY;
            Trees = trees;
        }

        public string Id { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public List<TreeModel> Trees { get; set; }

        public BsonDocument GenerateBson()
        {
            var bsonTrees = new BsonArray();
            foreach (var tree in Trees)
            {
                bsonTrees.Add(tree.GenerateBson());
            }
            var document = new BsonDocument()
            {
                {"_id",new BsonObjectId(Id)},
                {"siseX",SizeX},
                {"siseY",SizeY},
                {"trees",bsonTrees}
                 
            };
            return document;
        }
    }
}
