//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GlnApi.ViewModels;

namespace GlnApi.Models
{
    [Table("GLNREGISTRY.Logs")]
    public class Log : ILog
    {
        public int Id { get; set; }
        [StringLength(255)]
        public string SystemName { get; set; }
        public Enums.LogType LogType { get; set; }
        public Enums.MessageType MessageType { get; set; }
        public string Message { get; set; }
        [StringLength(255)]
        public string SamAccountName { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string BeforeUpdate { get; set; }
        public string AfterUpdate { get; set; }
        public string GeneratedExceptionMessage { get; set; }
        public string GeneratedInnerExceptionMessage { get; set; }
    }
}
