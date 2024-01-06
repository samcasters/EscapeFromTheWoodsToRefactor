using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromTheWoods.Database.Model
{
    public class PathModel
    {
        public PathModel(int seqNr, string monkeyId, string treeId)
        {
            SeqNr = seqNr;
            MonkeyId = monkeyId;
            TreeId = treeId;
        }

        public PathModel(string id, int seqNr, string monkeyId, string treeId)
        {
            Id = id;
            SeqNr = seqNr;
            MonkeyId = monkeyId;
            TreeId = treeId;
        }

        public string Id { get; set; }
        public int SeqNr { get; set; }
        public string MonkeyId { get; set; }
        public string TreeId { get; set; }


        public BsonDocument GenerateBson()
        {
            var document = new BsonDocument()
            {
                {"_id",new BsonObjectId(Id)},
                {"seqNr",SeqNr },
                {"monkeyId",MonkeyId},
                {"treeId",TreeId}
            };
            return document;
        }
    }
}