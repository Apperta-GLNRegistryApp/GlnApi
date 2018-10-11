//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Data.Entity;
using System.Linq;

namespace GlnApi.Repository
{
    public class IprRepository : Repository<Ipr>, IIprRepository
    {
        public IprRepository(DbContext context) : base(context)
        {

        }
        public GlnRegistryDb GLNdbDiagramContainer
        {
            get { return _context as GlnRegistryDb; }
        }

        public Ipr UpdateIpr(Ipr ipr)
        {
            var updateIpr = this.GLNdbDiagramContainer.Iprs.SingleOrDefault(i => i.Id == ipr.Id);

            updateIpr.Active = ipr.Active;
            updateIpr.IprName = ipr.IprName;
            updateIpr.IprImageAddress = ipr.IprImageAddress;

            return updateIpr;
        }

    }
}