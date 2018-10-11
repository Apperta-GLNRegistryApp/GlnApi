//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;

namespace GlnApi.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IGlnRepository Glns { get; }
        IAddressRepository Addresses { get; }
        IPrimaryContactRepository PrimaryContacts { get; }
        IAdditionalContactRepository AdditionalContacts { get; }
        IIprRepository Ipr { get; }
        IGlnTagRepository GlnTag { get; }
        IGlnTagTypeRepository GlnTagType { get; }
        ILogRepository Logs { get; }
        int Complete();

    }
}
