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
    public interface IGlnRegistryDb
    {
        DbSet<AdditionalContact> AdditionalContacts { get; set; }
        DbSet<Address> Addresses { get; set; }
        DbSet<Extension> Extensions { get; set; }
        DbSet<GlnAssociation> GlnAssociations { get; set; }
        DbSet<Gln> Glns { get; set; }
        DbSet<Ipr> Iprs { get; set; }
        DbSet<PrimaryContact> PrimaryContacts { get; set; }
        DbSet<GlnTag> Tags { get; set; }
        DbSet<GlnTagType> TagTypes { get; set; }
        int SaveChanges();
    }
}