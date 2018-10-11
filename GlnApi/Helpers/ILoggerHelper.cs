//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Security.Principal;

namespace GlnApi.Helpers
{
    public interface ILoggerHelper
    {
        void FailedServerLog(string message, IPrincipal user);
        void SuccessfulServerLog<TBefore, TAfter>(string message, IPrincipal user,
            TBefore beforeUpdate = default(TBefore), TAfter afterUpdate = default(TAfter));
        void SuccessfullyAddedServerLog<TAfter>(IPrincipal user, TAfter beforeUpdate = default(TAfter));

        void SuccessfulUpdateServerLog<TBefore, TAfter>(IPrincipal user, TBefore beforeUpdate = default(TBefore),
            TAfter afterUpdate = default(TAfter));

        void SuccessfulReplacementServerLog<TToDeactivate, TReplacement>(IPrincipal user, TToDeactivate toDeactivate = default(TToDeactivate),
            TReplacement replacement = default(TReplacement));

        void FailedUpdateServerLog<TInnerExc, TBefore, TAfter>(IPrincipal user, string generatedException,
            TInnerExc innerException = default(TInnerExc),
            TBefore beforeUpdate = default(TBefore),
            TAfter afterUpdate = default(TAfter));

        void FailedToCreateServerLog<TInnerExc, TBefore>(IPrincipal user, string generatedException,
            TInnerExc innerException = default(TInnerExc), TBefore beforeUpdate = default(TBefore));

        void ConcurrenyServerLog<TBefore, TAfter>(IPrincipal user, TBefore beforeUpdate = default(TBefore), TAfter afterUpdate = default(TAfter));

        void SiteVisitedLog<TGln>(IPrincipal user, TGln gln);
    }
}
