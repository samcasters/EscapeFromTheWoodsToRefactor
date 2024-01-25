using EscapeFromTheWoods.Database.exceptions;
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
        public TreeModel(int locX, int locY, string woodId)
        {
            LocX = locX;
            LocY = locY;
            WoodId = woodId;
        }
        public TreeModel(string id, int locX, int locY, string woodId)
        {
            Id = id;
            LocX = locX;
            LocY = locY;
            WoodId = woodId;
        }

        public string Id {  get; set; }
        public int LocX {  get; set; }
        public int LocY { get; set; }
        public string WoodId { get; set; }

        public BsonDocument GenerateBson()
        {
            try
            {
                var document = new BsonDocument()
                {

                {"_id",new BsonObjectId(Id)},
                {"locationX",LocX},
                {"locationY",LocY},
                    {"woodId",WoodId}

                };
                return document;
            }
            catch (Exception ex)
            {
                throw new ModelException($"id:{Id} locationX:{LocX} locationY:{LocY} woodId{WoodId},",ex);
            }
        }
    }
}
