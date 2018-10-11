//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using GlnApi.DTOs;

namespace GlnApi.DTOs
{
    public class GlnTagDto
    {
        public int GlnTagId { get; set; }
        public int GlnTagTypeId { get; set; }
        public int GlnId { get; set; }
        public string TypeKey { get; set; }
        public bool Active { get; set; }

        public virtual GlnTagTypeDto GlnTagType { get; set; }
        public override string ToString()
        {
            return $"GLN Tag Id: {GlnTagId}, GLN Tag Type Id: {GlnTagTypeId}, GLN Id: {GlnId}, TypeKey: {TypeKey}.";
        }
    }
}