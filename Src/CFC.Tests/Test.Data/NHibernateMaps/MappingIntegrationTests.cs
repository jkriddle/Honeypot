namespace CFC.Tests.CFC.Data.NHibernateMaps
{
    using System.IO;

    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;

    using NUnit.Framework;

    using SharpArch.NHibernate;
    using SharpArch.Testing.NUnit.NHibernate;

    using global::CFC.Infrastructure.NHibernateMaps;

    /// <summary>
    /// Provides a means to verify that the target database is in compliance with all mappings.
    /// Taken from http://ayende.com/Blog/archive/2006/08/09/NHibernateMappingCreatingSanityChecks.aspx.
    /// 
    /// If this is failing, the error will likely inform you that there is a missing table or column
    /// which needs to be added to your database.
    /// </summary>
    [TestFixture]
    [Category("DB Tests")]
    public class MappingIntegrationTests
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
        [Explicit]
        public void CanConfirmDatabaseMatchesMappings()
        {
            var allClassMetadata = NHibernateSession.GetDefaultSessionFactory().GetAllClassMetadata();

            foreach (var entry in allClassMetadata)
            {
                NHibernateSession.Current.CreateCriteria(entry.Value.GetMappedClass(EntityMode.Poco))
                     .SetMaxResults(0).List();
            }
        }

        /// <summary>
        /// Generates and outputs the database schema SQL to the console
        /// </summary>
        [Test]
        [Explicit]
        public void CanGenerateDatabaseSchema()
        {
            var session = NHibernateSession.GetDefaultSessionFactory().OpenSession();

            using (TextWriter stringWriter = new StreamWriter("../../../../Database/UnitTestGeneratedSchema.sql"))
            {
                new SchemaExport(this._configuration).Execute(true, false, false, session.Connection, stringWriter);
            }
        }

        /// <summary>
        /// Creates/Updates database schema, this runs on database configured in 
        /// Mvc project and is marked as Explicit because it changes the database.
        /// </summary>
        [Test]
        [Explicit]
        public void CanCreateDatabase()
        {
            new SchemaExport(this._configuration).Execute(false, true, false);
        }
    }
}
