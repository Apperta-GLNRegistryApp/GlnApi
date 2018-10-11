//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using GlnApi.Models;
using GlnApi.Repository;

namespace GlnApi.Tests.TestHelpers
{
    public class FakeGlnContext : DbContext, IGlnRegistryDb
    {
        public FakeGlnContext()
        {
            AdditionalContacts = new FakeDbSet<AdditionalContact>();
            Addresses = new FakeDbSet<Address>();
            Extensions = new FakeDbSet<Extension>();
            GlnAssociations = new FakeGlnAssociationDbSet();
            Glns = new FakeGlnDbSet();
            Iprs = new FakeDbSet<Ipr>();
            PrimaryContacts = new FakeDbSet<PrimaryContact>();
        }
        public DbSet<AdditionalContact> AdditionalContacts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Extension> Extensions { get; set; }
        public DbSet<GlnAssociation> GlnAssociations { get; set; }
        public DbSet<Gln> Glns { get; set; }
        public DbSet<Ipr> Iprs { get; set; }
        public DbSet<PrimaryContact> PrimaryContacts { get; set; }
        public DbSet<GlnTag> Tags { get; set; }
        public DbSet<GlnTagType> TagTypes { get; set; }

        public IDbSet<T> Set<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public DbEntityEntry<T> Entry<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            return 0;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}