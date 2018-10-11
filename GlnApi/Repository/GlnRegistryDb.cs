//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using GlnApi.Models;
using GlnApi.Repository;

namespace GlnApi
{
    public partial class GlnRegistryDb : DbContext, IGlnRegistryDb
    {
        public GlnRegistryDb()
            : base("name=GlnRegistryDb")
        {

        }
        public virtual DbSet<AdditionalContact> AdditionalContacts { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Extension> Extensions { get; set; }
        public virtual DbSet<GlnAssociation> GlnAssociations { get; set; }
        public virtual DbSet<Gln> Glns { get; set; }
        public virtual DbSet<Ipr> Iprs { get; set; }
        public virtual DbSet<PrimaryContact> PrimaryContacts { get; set; }
        public virtual DbSet<GlnTag> Tags { get; set; }
        public virtual DbSet<GlnTagType> TagTypes { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
                .HasMany(e => e.Glns)
                .WithRequired(e => e.Address)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Gln>()
                .HasMany(e => e.Extensions)
                .WithRequired(e => e.Gln)
                .HasForeignKey(e => e.BarcodeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Gln>()
                .HasMany(e => e.Children)
                .WithOptional(e => e.Parent)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<PrimaryContact>()
                .HasMany(e => e.Glns)
                .WithRequired(e => e.PrimaryContact)
                .HasForeignKey(e => e.ContactId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Gln>()
                .HasMany(e => e.Tags)
                .WithRequired(e => e.Gln)
                .HasForeignKey(e => e.GlnId)
                .WillCascadeOnDelete(false);
        }
    }
}
