//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using GlnApi.Interfaces;
using GlnApi.ViewModels;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GlnApi.Services
{
    public class MongoLogger : IMongoLogger
    {
        private IMongoCollection<BsonDocument> _mongoCollection;
        private readonly IMongoHelper _mongoHelper;

        public MongoLogger(IMongoHelper mongorHelper)
        {
            _mongoHelper = mongorHelper;
            _mongoCollection = _mongoHelper.SetCollection("log");
        }

        public void AddLog(MongoLog newMongoLog)
        {
            try
            {
                _mongoCollection.InsertOne(newMongoLog.ToBsonDocument());
            }
            catch (Exception ex)
            {
                    
                throw new Exception($"Adding log to mongdb was not successful, Exception generated: {ex}");
            }
        }

        public void SetCollection(string collectionName)
        {
            _mongoCollection = _mongoHelper.SetCollection(collectionName);
        }

    }
}