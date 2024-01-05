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
        public WoodModel(int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
        }

        public WoodModel(string id, int sizeX, int sizeY)
        {
            Id = id;
            SizeX = sizeX;
            SizeY = sizeY;
        }

        public string Id { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }

        public BsonDocument GenerateBson()
        {
            var document = new BsonDocument()
            {
                {"_id",Id},
                {"siseX",SizeX},
                {"siseY",SizeY}
                 
            };
            return document;
        }
    }
}
