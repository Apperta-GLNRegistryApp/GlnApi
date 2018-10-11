//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Collections.Generic;
using System.Linq;
using GlnApi.DTOs;
using GlnApi.Models;

namespace GlnApi.Repository
{
    public interface IGlnRepository : IRepository<Gln>
    {
        Gln GetGlnsByGln(string gln);
        IEnumerable<Gln> GetGlnAssociations(int glnId);
        void AssignNewGlnAssociation(int glnId1, int glnId2);
        void RemoveGlnAssociation(int glnId1, int glnId2);
        void RemoveAllGlnAssociation(int glnId);
        QueryResult<GlnDto> GetGlnsByQuery(GlnQuery queryObj);
        Gln UpdateGln(Gln sentGln);
        bool PrimaryContactExists(int pcId);
        bool AdditionalContactExists(int acId);
        AdditionalContact GetAdditionalContact(int id);
        IGlnRegistryDb IGlnRegistryDb { get; }
    }
}