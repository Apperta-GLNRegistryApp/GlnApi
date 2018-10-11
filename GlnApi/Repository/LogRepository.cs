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
    public class LogRepository : Repository<Log>, ILogRepository
    {
        public LogRepository(DbContext context) : base(context)
        {

        }
        public GlnRegistryDb GLNdbDiagramContainer
        {
            get { return _context as GlnRegistryDb; }
        }
    }
}