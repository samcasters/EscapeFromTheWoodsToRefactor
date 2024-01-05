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
        public PathModel(int seqNr, MonkeyModel monkey, TreeModel tree)
        {
            SeqNr = seqNr;
            Monkey = monkey;
            Tree = tree;
        }

        public PathModel(string id, int seqNr, MonkeyModel monkey, TreeModel tree)
        {
            Id = id;
            SeqNr = seqNr;
            Monkey = monkey;
            Tree = tree;
        }

        public string Id { get; set; }
        public int SeqNr { get; set; }
        public MonkeyModel Monkey { get; set; }
        public TreeModel Tree { get; set; }


        public BsonDocument GenerateBson()
        {
            var document = new BsonDocument()
            {
                {"_id",Id },
                {"seqNr",SeqNr },
                {"monkeyId",Monkey.Id},
                {"treeId",Tree.Id}
            };
            return document;
        }
    }
}