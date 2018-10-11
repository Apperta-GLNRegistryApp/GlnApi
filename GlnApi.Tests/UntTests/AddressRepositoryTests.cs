//Global Location Number (GLN) Registry API
//Copyright (C) 2018  University Hospitals Plymouth NHS Trust
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlnApi.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xunit;
using Assert = Xunit.Assert;


namespace GlnApi.Tests.UntTests
{
    [TestClass]
    public class AddressRepositoryTests
    {
        private readonly Mock<IAddressRepository> _mockAddressRepository;

        public AddressRepositoryTests()
        {
            _mockAddressRepository = new Mock<IAddressRepository>();
        }

        [Fact]
        [Trait("Category", "AddressRepositoryConstructor")]
        public void AddressRepositoryConstructorTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                IAddressRepository newAddressRepository = new AddressRepository(null);
            });
        }
    }
}
