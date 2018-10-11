//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Security.Principal;
using GlnApi.Interfaces;
using GlnApi.Services;
using GlnApi.ViewModels;
using MongoDB.Bson;

namespace GlnApi.Helpers
{
    public class MongoLoggerHelper : ILoggerHelper
    {
        private readonly IMongoHelper _mongoHelper;
        private readonly IMongoLogger _mongoLogger;

        public MongoLoggerHelper()
        {
        }

        public MongoLoggerHelper(IMongoHelper mongoHelper, IMongoLogger mongoLogger):this()
        {
            _mongoHelper = mongoHelper;
            _mongoLogger = mongoLogger;
        }

        public void FailedServerLog(string message, IPrincipal user)
        {
            var log = new MongoLog()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Error,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = _mongoHelper.GetMongoSystemName(),
                SamAccountName = user.Identity.Name,
            };

            _mongoLogger.AddLog(log);

        }
        public void SuccessfulServerLog<TBefore, TAfter>(string message, IPrincipal user, TBefore beforeUpdate = default(TBefore) , TAfter afterUpdate = default(TAfter))
        {
            var log = new MongoLog()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Success,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = _mongoHelper.GetMongoSystemName(),
                SamAccountName = user.Identity.Name
                
            };

            if (!Equals(beforeUpdate, null))
                log.BeforeUpdate = beforeUpdate;

            if (!Equals(afterUpdate, null))
                log.AfterUpdate = afterUpdate;

            _mongoLogger.AddLog(log);
        }
        public void SuccessfullyAddedServerLog<TAfter>(IPrincipal user, TAfter beforeUpdate = default(TAfter))
        {
            string message;

            if (!Equals(beforeUpdate, null))
            {
                message = $"{beforeUpdate} was successfully added";
            }
            else
            {
                message = "Successful addition to db occured";
            }

            var log = new MongoLog()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Success,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = _mongoHelper.GetMongoSystemName(),
                SamAccountName = user.Identity.Name
            };

            if (!Equals(beforeUpdate, null))
                log.BeforeUpdate = beforeUpdate;

            _mongoLogger.AddLog(log);
        }

        public void SuccessfulUpdateServerLog<TBefore, TAfter>(IPrincipal user, TBefore beforeUpdate = default(TBefore), TAfter afterUpdate = default(TAfter))
        {
            string message;

            if (!Equals(beforeUpdate, null))
            {
                message = $"{beforeUpdate} was successfully updated";
            }
            else
            {
                message = "Successful update occured";
            }

            var log = new MongoLog()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Success,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = _mongoHelper.GetMongoSystemName(),
                SamAccountName = user.Identity.Name

            };

            if (!Equals(beforeUpdate, null))
                log.BeforeUpdate = beforeUpdate;

            if (!Equals(afterUpdate, null))
                log.AfterUpdate = afterUpdate;

            _mongoLogger.AddLog(log);
        }

        public void SuccessfulReplacementServerLog<TToDeactivate, TReplacement>(IPrincipal user,
            TToDeactivate toDeactivate = default(TToDeactivate), TReplacement replacement = default(TReplacement))
        {
            string message;

            if (!Equals(toDeactivate, null))
            {
                message = $"{toDeactivate.ToString()} was successfully replaced with {replacement.ToString()}";
            }
            else
            {
                message = "Successful replacement occured";
            }

            var log = new MongoLog()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Replacement,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = _mongoHelper.GetMongoSystemName(),
                SamAccountName = user.Identity.Name

            };

            _mongoLogger.AddLog(log);
        }


        public void FailedUpdateServerLog<TInnerExc, TBefore, TAfter>(IPrincipal user, string generatedException, 
                                                                        TInnerExc innerException = default(TInnerExc), 
                                                                        TBefore beforeUpdate = default(TBefore), 
                                                                        TAfter afterUpdate = default(TAfter))
        {
            string message;

            if (!Equals(beforeUpdate, null))
            {
                message = $"{beforeUpdate} failed to update.";
            }
            else
            {
                message = "An update failed.";
            }

            var log = new MongoLog()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Error,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = _mongoHelper.GetMongoSystemName(),
                SamAccountName = user.Identity.Name,
                GeneratedExceptionMessage = generatedException

            };

            if (!Equals(beforeUpdate, null))
                log.BeforeUpdate = beforeUpdate;

            if (!Equals(afterUpdate, null))
                log.AfterUpdate = afterUpdate;

            if (!Equals(innerException, null))
                log.GeneratedInnerExceptionMessage = innerException.ToString();

            _mongoLogger.AddLog(log);

        }

        public void FailedToCreateServerLog<TInnerExc, TBefore> (IPrincipal user, string generatedException, TInnerExc innerException = default(TInnerExc), TBefore beforeUpdate = default(TBefore))
        {
            string message;

            if (!Equals(beforeUpdate, null))
            {
                message = $"{beforeUpdate} failed to create.";
            }
            else
            {
                message = "An creation failed.";
            }

            var log = new MongoLog()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Error,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = _mongoHelper.GetMongoSystemName(),
                SamAccountName = user.Identity.Name,
                GeneratedExceptionMessage = generatedException
                

            };

            if (!Equals(beforeUpdate, null))
                log.BeforeUpdate = beforeUpdate;

            if (!Equals(innerException, null))
                log.GeneratedInnerExceptionMessage = innerException.ToString();

            _mongoLogger.AddLog(log);
        }

        public void ConcurrenyServerLog<TBefore, TAfter>(IPrincipal user, TBefore beforeUpdate = default(TBefore), TAfter afterUpdate = default(TAfter))
        {
            string message;

            if (!Equals(beforeUpdate, null) && !Equals(afterUpdate, null))
            {
                message = $"Falied to update {beforeUpdate} to {afterUpdate} because versions are out of sync.";
            }
            else
            {
                message = "An update failed.";
            }

            var log = new MongoLog()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Concurrency,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = _mongoHelper.GetMongoSystemName(),
                SamAccountName = user.Identity.Name

            };

            if (!Equals(beforeUpdate, null))
                log.BeforeUpdate = beforeUpdate;

            if (!Equals(afterUpdate, null))
                log.AfterUpdate = afterUpdate;

            _mongoLogger.AddLog(log);
        }
        public void SiteVisitedLog<TGln>(IPrincipal user, TGln gln)
        {
            var log = new MongoLog()
            {
                LogType = Enums.LogType.Visit,
                MessageType = Enums.MessageType.Concurrency,
                Message = $"Site visited - {gln.ToString()}",
                CreatedDateTime = DateTime.Now,
                SystemName = _mongoHelper.GetMongoSystemName(),
                SamAccountName = user.Identity.Name
            };

            log.BeforeUpdate = gln.ToString();

            _mongoLogger.AddLog(log);
        }
    }
}