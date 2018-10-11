//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using GlnApi.Helpers;
using GlnApi.Repository;

namespace GlnApi.Services
{
    public class Export
    {

        private readonly IUnitOfWork _unitOfWork;
        private StringBuilder _sb;
        private IEnumerable<Gln> _glns;

        private readonly string HeaderList =
                $"GLN, Parent GLN, Name, Active, GLN Type - Functional, GLN Type - Physical, GLN Type - Legal, GLN Type - Digital, Use parent address, Address - Line 1, Address - Line 2, Address - City, Address - Region/County, Address - Postcode, Address - Country, Address - Delivery notes, Contact - Salutation, Contact - First Name, Contact - Surname, Contact - Function, Contact - Email Address, Contact - Telephone"
            ;

        public Export( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
 
        }

        public string CreateCsv()
        {
            try
            {
                var csvExport = new StringBuilder();

                csvExport.AppendLine(HeaderList);

                var filePath = Path.GetFullPath(Path.Combine(HostingEnvironment.MapPath("~/GLNexport/"), "GLNcsvRegistryExport.csv"));

                using (var sw = new StreamWriter(filePath, false))
                {
                    sw.WriteLine(HeaderList);

                    _glns = _unitOfWork.Glns.Find(gln => gln.Assigned && gln.Public).OrderByDescending(gln => gln.LastUpdate);

                    foreach (var gln in _glns)
                    {
                        sw.WriteLine(TransformGlnIntoCsvLine(gln));
                        csvExport.AppendLine(TransformGlnIntoCsvLine(gln));
                    }

                    sw.Close();
                }

                File.WriteAllText(filePath, csvExport.ToString());

                return csvExport.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private string TransformGlnIntoCsvLine(Gln gln)
        {
            var sb = new StringBuilder();

            sb.Append(TransfomGlnIntoCsv(gln));
            sb.Append(TransformAddressIntoCsv(gln));
            sb.Append(TransformPrimaryContactIntoCsv(gln));

            return sb.ToString();
        }

        private string TransfomGlnIntoCsv(Gln gln)
        {
            var sb = new StringBuilder();

            sb.Append(gln.OwnGln);
            sb.Append(", ");
            sb.Append(gln.ParentGln);
            sb.Append(", ");
            sb.Append(gln.FriendlyDescriptionPurpose = gln.FriendlyDescriptionPurpose.Replace(",", " - "));
            sb.Append(", ");
            sb.Append(gln.Active);
            sb.Append(", ");
            sb.Append(gln.FunctionalType);
            sb.Append(", ");
            sb.Append(gln.PhysicalType);
            sb.Append(", ");
            sb.Append(gln.LegalType);
            sb.Append(", ");
            sb.Append(gln.DigitalType);
            sb.Append(", ");
            sb.Append(gln.UseParentAddress);
            sb.Append(", ");

            return sb.ToString();
        }

        private string TransformAddressIntoCsv(Gln gln)
        {
            var sb = new StringBuilder();

            sb.Append(gln.Address.AddressLineOne);
            sb.Append(", ");
            sb.Append(gln.Address.AddressLineTwo);
            sb.Append(", ");
            sb.Append(gln.Address.City);
            sb.Append(", ");
            sb.Append(gln.Address.RegionCounty);
            sb.Append(", ");
            sb.Append(gln.Address.Postcode);
            sb.Append(", ");
            sb.Append(gln.Address.Country);
            sb.Append(", ");
            sb.Append(gln.DeliveryNote);
            sb.Append(", ");

            return sb.ToString();
        }
        private string TransformPrimaryContactIntoCsv(Gln gln)
        {
            var sb = new StringBuilder();
            var name = gln.PrimaryContact.Name.Split();

            sb.Append(gln.PrimaryContact.Salutation);
            sb.Append(", ");
            sb.Append(name.First());
            sb.Append(", ");

            if (name.Length > 1)
                sb.Append(name[1]);

            sb.Append(", ");
            sb.Append(gln.PrimaryContact.Function);
            sb.Append(", ");
            sb.Append(gln.PrimaryContact.Email);
            sb.Append(", ");
            sb.Append(gln.PrimaryContact.Telephone);

            return sb.ToString();
        }
    }
}