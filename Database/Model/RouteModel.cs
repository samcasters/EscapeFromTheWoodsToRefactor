using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace EscapeFromTheWoods.Database.Model
{
    public class RouteModel
    {
        public RouteModel(string id, List<PathModel> paths)
        {
            Id = id;
            Paths = paths;
        }

        public string Id { get; set; }
        public List<PathModel> Paths { get; set; }

        public BsonDocument GenerateBson()
        {
            var bsonPaths = new BsonArray();
            foreach (var path in Paths)
            {
                bsonPaths.Add(path.GenerateBson());
            }
            var document = new BsonDocument()
            {
                {"_id",new BsonObjectId(Id)},
                {"paths",bsonPaths }
            };
            return document;
        }
    }
}
