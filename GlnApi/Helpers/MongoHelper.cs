//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using GlnApi.Interfaces;

namespace GlnApi.Helpers
{
    public class MongoHelper : IMongoHelper
    {
        private readonly string _mongoServer = ConfigurationManager.AppSettings["customise:mongoConnectionString"];
        private readonly string _mongoDatabaseName = ConfigurationManager.AppSettings["customise:mongoDatabaseName"];
        private readonly string _mongoSystemName = ConfigurationManager.AppSettings["customise:mongoSystemName"];

        private readonly IMongoDatabase _mongoDatabase;

        public MongoHelper()
        {
            var mongoClient = new MongoClient(_mongoServer);
            _mongoDatabase = mongoClient.GetDatabase(_mongoDatabaseName);
        }

        public string GetMongoSystemName()
        {
            return _mongoSystemName;
        }

        public IMongoCollection<BsonDocument> SetCollection(string collectionName)
        {
            return _mongoDatabase.GetCollection<BsonDocument>(collectionName);
        }

        public T DeserializeBsonDocument<T>(BsonDocument document)
        {
            return BsonSerializer.Deserialize<T>(document);
        }

    }
}