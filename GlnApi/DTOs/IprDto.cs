//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿namespace GlnApi.DTOs
{
    public class IprDto
    {
        public int Id { get; set; }
        public string IprName { get; set; }
        public string IprImageAddress { get; set; }
        public bool Active { get; set; }
        public override string ToString()
        {
            return $"IPR Id: {Id}, Name: {IprName}, Active: {Active}, ImageAddress: {IprImageAddress}.";
        }
    }
}