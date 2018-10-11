//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Collections.Generic;

namespace GlnApi.DTOs
{
    public class GlnTagTypeDto
    {
        public int GlnTagTypeId { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
        public override string ToString()
        {
            return $"GLN Tag Type Id: {GlnTagTypeId}, Description: {Description}, Code: {Code}.";
        }
    }
}