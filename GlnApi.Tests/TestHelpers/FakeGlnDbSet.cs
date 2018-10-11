//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Linq;

namespace GlnApi.Tests.TestHelpers
{
    class FakeGlnDbSet : FakeDbSet<Gln>
    {
        public override Gln Find(params object[] keyValues)
        {
            var id = (int)keyValues.Single();
            return this.SingleOrDefault(g => g.Id == id);
            //return new Gln {Id = 1, OwnGln = "MATCH"};
        }

    }
}
