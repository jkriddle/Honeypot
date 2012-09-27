using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFC.Domain;
using CFC.Infrastructure.CompanyRepository;
using CFC.Infrastructure.NHibernateMaps;
using CFC.Infrastructure.UserRepository;
using CFC.Services.CompanyService;
using CFC.Services.EmailService;
using CFC.Services.UserService;
using NHibernate.Cfg;
using NUnit.Framework;
using SharpArch.NHibernate;
using SharpArch.Testing.NUnit.NHibernate;


namespace CFC.Tests.ServiceTests
{
    [TestFixture]
    [Category("Service Tests")]
    public class ServiceIntegrationTest
    {
        private Configuration _configuration;

        [SetUp]
        public virtual void SetUp()
        {
            string[] mappingAssemblies = RepositoryTestsHelper.GetMappingAssemblies();
            this._configuration = NHibernateSession.Init(
                new SimpleSessionStorage(),
                mappingAssemblies,
                new AutoPersistenceModelGenerator().Generate(),
                "../../../../Src/CFC.Web.Mvc/NHibernate.config");
        }

        [TearDown]
        public virtual void TearDown()
        {
            NHibernateSession.CloseAllSessions();
            NHibernateSession.Reset();
        }

        [Test]
        public void CreateServices()
        {
            ICompanyService companyService = new CompanyService(new CompanyRepository(), new AddressRepository(),
                                                                new ZoneRepository());
            IUserService userService = new UserService(new UserRepository(), new EmailService("../"),
                                                       new DeviceRepository());
        }

        [Test]
        public void GetCompanies()
        {
            //var mockHelper = new MockHelper();
            var company = new Company();
            //var code = mockHelper.Generate(company);
            ICompanyService companyService = new CompanyService(new CompanyRepository(), new AddressRepository(),
                                                               new ZoneRepository());
            company = companyService.GetAllCompanies().FirstOrDefault();
           
        }
    }
}
