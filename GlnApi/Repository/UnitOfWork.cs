//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Data.Entity;
using GlnApi.Models;

namespace GlnApi.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        public IGlnRepository Glns { get; private set; }

        public IAddressRepository Addresses { get; private set; }
        public IPrimaryContactRepository PrimaryContacts { get; }

        public IAdditionalContactRepository AdditionalContacts { get; }
        public IIprRepository Ipr { get; }
        public IGlnTagRepository GlnTag { get; }
        public IGlnTagTypeRepository GlnTagType { get; }
        public ILogRepository Logs { get; }

        public UnitOfWork(DbContext context)
        {
            _context = context;
            Glns = new GlnRepository(_context);
            Addresses = new AddressRepository(_context);
            PrimaryContacts = new PrimaryContactRepository(_context);
            AdditionalContacts = new AdditionalContactRepository(_context);
            Ipr = new IprRepository(_context);
            GlnTag = new GlnTagRepository(_context);
            GlnTagType = new GlnTagTypeRepository(_context);
            Logs = new LogRepository(_context);
        }
        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}