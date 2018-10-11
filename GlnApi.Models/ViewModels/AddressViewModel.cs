//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿namespace GlnApi.Models.ViewModels
{
    public class AddressViewModel
    {
        public int Id { get; set; }
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string AddressLineThree { get; set; }
        public string AddressLineFour { get; set; }
        public string City { get; set; }
        public string RegionCounty { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public int Level { get; set; }
        public string Directorate { get; set; }
        public string Department { get; set; }
        public string LocationCostCentre { get; set; }
        public string Room { get; set; }
        public bool Active { get; set; }
        public string DeliveryNote { get; set; }
        public int Version { get; set; }

        public override string ToString()
        {
            return $"Address Id: {Id}, Version: {Version}, {AddressLineOne}, {Postcode}.";
        }
    }

    
}