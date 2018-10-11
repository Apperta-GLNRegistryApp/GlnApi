//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using GlnApi.ViewModels;

namespace GlnApi.Services
{
    public interface IMongoLogger
    {
        void AddLog(MongoLog newMongoLog);
        void SetCollection(string collectionName);
    }
}
