using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace EscapeFromTheWoods.Database.Model
{
    public class TreeModel
    {
        public TreeModel(int locX, int locY, WoodModel wood)
        {
            LocX = locX;
            LocY = locY;
            Wood = wood;
        }

        public TreeModel(string id, int locX, int locY, WoodModel wood)
        {
            Id = id;
            LocX = locX;
            LocY = locY;
            Wood = wood;
        }

        public string Id {  get; set; }
        public int LocX {  get; set; }
        public int LocY { get; set; }
        public WoodModel Wood { get; set; }

        public BsonDocument GenerateBson()
        {
            var document = new BsonDocument()
            {
                
                {"_id",Id},
                {"locationX",LocX},
                {"locationY",LocY},
                    {"woodId",Wood.Id}
                
            };
            return document;
        }
    }
}
