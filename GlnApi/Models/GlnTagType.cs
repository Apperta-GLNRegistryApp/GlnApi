//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlnApi.Models
{
    [Table("GLNREGISTRY.GlnTagTypes")]
    public class GlnTagType
    {
        public int GlnTagTypeId { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string UserCreated { get; set; }
        public string UserModified { get; set; }
        public bool Active { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }

    }
}