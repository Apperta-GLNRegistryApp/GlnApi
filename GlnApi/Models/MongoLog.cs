//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using GlnApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GlnApi.ViewModels
{
    public class MongoLog : ILog
    {
        public MongoLog()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string SystemName { get; set; }
        public Enums.LogType LogType { get; set; }
        public Enums.MessageType MessageType { get; set; }
        public string Message { get; set; }
        public string SamAccountName { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public dynamic BeforeUpdate { get; set; }
        public dynamic AfterUpdate { get; set; }
        public string GeneratedExceptionMessage { get; set; }
        public string GeneratedInnerExceptionMessage { get; set; }

    }
}