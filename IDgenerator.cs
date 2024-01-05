using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeFromTheWoods
{
    public static class IDgenerator
    {
        [BsonId]
        private static ObjectId objectID;

        public static string GetNewID()
        {
            objectID = ObjectId.GenerateNewId();
            string stringId = $"{objectID}";
            return stringId;
        }
    }
}
