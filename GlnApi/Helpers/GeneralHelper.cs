//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System.Web;
using GlnApi.ViewModels;

namespace GlnApi.Helpers
{

    public static class GeneralHelper
    {
        public static string[] SplitStringByWhiteSpace(string initialValue)
        {
            return initialValue.Split(null);
        }

        public static string GetJobTitleFromDisplayName(string displayName)
        {
            var splitDisplayName = displayName.Split(',');

            return splitDisplayName.Length < 2 ? "" : splitDisplayName[1].Trim();
        }
    }
}