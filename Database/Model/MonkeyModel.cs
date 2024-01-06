using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace EscapeFromTheWoods.Database.Model
{
    public class MonkeyModel
    {
        public MonkeyModel(string name)
        {
            Name = name;
        }

        public MonkeyModel(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public BsonDocument GenerateBson()
        {
            var document = new BsonDocument()
            {

                {"_id",new BsonObjectId(Id)},
                {"name",Name }
                
            };
            return document;
        }
    }
}
