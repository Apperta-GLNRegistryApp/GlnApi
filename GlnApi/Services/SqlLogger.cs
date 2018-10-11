//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Security.Principal;
using gln_registry_aspNet.Migrations;
using GlnApi.Helpers;
using GlnApi.Models;
using GlnApi.Repository;
using GlnApi.ViewModels;

namespace GlnApi.Services
{
    public class SqlLogger : ILoggerHelper
    {
        private IUnitOfWork _unitOfWork;
        private const string SYSTEM_NAME = "gln-registry";
        public SqlLogger(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void FailedServerLog(string message, IPrincipal user)
        {
            var log = new Log()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Error,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = SYSTEM_NAME,
                SamAccountName = user.Identity.Name,
            };

            AddLog(log);
            
        }

        public void SuccessfulServerLog<TBefore, TAfter>(string message, IPrincipal user, TBefore beforeUpdate = default(TBefore),
            TAfter afterUpdate = default(TAfter))
        {
            var log = new Log()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Success,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = SYSTEM_NAME,
                SamAccountName = user.Identity.Name

            };

            if (!Equals(beforeUpdate, null))
                log.BeforeUpdate = beforeUpdate.ToString();

            if (!Equals(afterUpdate, null))
                log.AfterUpdate = afterUpdate.ToString();

            AddLog(log);
        }

        public void SuccessfullyAddedServerLog<TAfter>(IPrincipal user, TAfter beforeUpdate = default(TAfter))
        {
            string message;

            if (!Equals(beforeUpdate, null))
            {
                message = $"{beforeUpdate.ToString()} was successfully added";
            }
            else
            {
                message = "Successful addition to db occured";
            }

            var log = new Log()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Success,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = SYSTEM_NAME,
                SamAccountName = user.Identity.Name
            };

            if (!Equals(beforeUpdate, null))
                log.BeforeUpdate = beforeUpdate.ToString();

            AddLog(log);
        }

        public void SuccessfulUpdateServerLog<TBefore, TAfter>(IPrincipal user, TBefore beforeUpdate = default(TBefore),
            TAfter afterUpdate = default(TAfter))
        {
            string message;

            if (!Equals(beforeUpdate, null))
            {
                message = $"{beforeUpdate.ToString()} was successfully updated";
            }
            else
            {
                message = "Successful update occured";
            }

            var log = new Log()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Success,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = SYSTEM_NAME,
                SamAccountName = user.Identity.Name

            };

            if (!Equals(beforeUpdate, null))
                log.BeforeUpdate = beforeUpdate.ToString();

            if (!Equals(afterUpdate, null))
                log.AfterUpdate = afterUpdate.ToString();

            AddLog(log);
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

            var log = new Log()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Replacement,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = SYSTEM_NAME,
                SamAccountName = user.Identity.Name

            };

            AddLog(log);
        }

        public void FailedUpdateServerLog<TInnerExc, TBefore, TAfter>(IPrincipal user, string generatedException,
            TInnerExc innerException = default(TInnerExc), TBefore beforeUpdate = default(TBefore),
            TAfter afterUpdate = default(TAfter))
        {
            string message;

            if (!Equals(beforeUpdate, null))
            {
                message = $"{beforeUpdate.ToString()} failed to update.";
            }
            else
            {
                message = "An update failed.";
            }

            var log = new Log()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Error,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = SYSTEM_NAME,
                SamAccountName = user.Identity.Name,
                GeneratedExceptionMessage = generatedException

            };

            if (!Equals(beforeUpdate, null))
                log.BeforeUpdate = beforeUpdate.ToString();

            if (!Equals(afterUpdate, null))
                log.AfterUpdate = afterUpdate.ToString();

            if (!Equals(innerException, null))
                log.GeneratedInnerExceptionMessage = innerException.ToString();

            AddLog(log);
        }

        public void FailedToCreateServerLog<TInnerExc, TBefore>(IPrincipal user, string generatedException,
            TInnerExc innerException = default(TInnerExc), TBefore beforeUpdate = default(TBefore))
        {
            string message;

            if (!Equals(beforeUpdate, null))
            {
                message = $"{beforeUpdate.ToString()} failed to create.";
            }
            else
            {
                message = "An creation failed.";
            }

            var log = new Log()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Error,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = SYSTEM_NAME,
                SamAccountName = user.Identity.Name,
                GeneratedExceptionMessage = generatedException


            };

            if (!Equals(beforeUpdate, null))
                log.BeforeUpdate = beforeUpdate.ToString();

            if (!Equals(innerException, null))
                log.GeneratedInnerExceptionMessage = innerException.ToString();

            AddLog(log);
        }

        public void ConcurrenyServerLog<TBefore, TAfter>(IPrincipal user, TBefore beforeUpdate = default(TBefore),
            TAfter afterUpdate = default(TAfter))
        {
            string message;

            if (!Equals(beforeUpdate, null) && !Equals(afterUpdate, null))
            {
                message = $"Falied to update {beforeUpdate.ToString()} to {afterUpdate.ToString()} because versions are out of sync.";
            }
            else
            {
                message = "An update failed.";
            }

            var log = new Log()
            {
                LogType = Enums.LogType.Server,
                MessageType = Enums.MessageType.Concurrency,
                Message = message,
                CreatedDateTime = DateTime.Now,
                SystemName = SYSTEM_NAME,
                SamAccountName = user.Identity.Name

            };

            if (!Equals(beforeUpdate, null))
                log.BeforeUpdate = beforeUpdate.ToString();

            if (!Equals(afterUpdate, null))
                log.AfterUpdate = afterUpdate.ToString();

            AddLog(log);
        }

        public void SiteVisitedLog<TGln>(IPrincipal user, TGln gln)
        {
            var log = new Log()
            {
                LogType = Enums.LogType.Visit,
                MessageType = Enums.MessageType.Concurrency,
                Message = $"Site visited - {gln.ToString()}",
                CreatedDateTime = DateTime.Now,
                SystemName = SYSTEM_NAME,
                SamAccountName = user.Identity.Name
            };

            log.BeforeUpdate = gln.ToString();

            AddLog(log);
        }

        private void AddLog(Log log)
        {
            try
            {
                _unitOfWork.Logs.Add(log);
                _unitOfWork.Complete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}