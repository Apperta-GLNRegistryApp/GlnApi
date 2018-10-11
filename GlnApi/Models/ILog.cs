//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using GlnApi.ViewModels;

namespace GlnApi.Models
{
    public interface ILog
    {
        string SystemName { get; set; }
        Enums.LogType LogType { get; set; }
        Enums.MessageType MessageType { get; set; }
        string Message { get; set; }
        string SamAccountName { get; set; }
        DateTime CreatedDateTime { get; set; }
        string GeneratedExceptionMessage { get; set; }
        string GeneratedInnerExceptionMessage { get; set; }
    }
}
