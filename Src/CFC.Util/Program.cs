using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using CFC.Infrastructure.PageRepository;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using NHibernate.Tool.hbm2ddl;
using CFC.Domain;
using CFC.Infrastructure.NHibernateMaps;
using CFC.Services.UserService;
using SharpArch.Domain.PersistenceSupport;
using SharpArch.NHibernate;
using Microsoft.Practices.ServiceLocation;


namespace CFC.Util
{
    class Program
    {
        private static User demoCompanyAdmin1 = null;
        private static User demoCompanyAdmin2 = null;
           
        private static NHibernate.Cfg.Configuration _configuration;
        private static IUserService _userService;
        private static IPageRepository _pageRepository;
        private static Random _randomInt;
        static void Main(string[] args)
        {
            _randomInt = new Random(DateTime.Now.Second);
            _pageRepository = new PageRepository();
            _configuration = NHibernateSession.Init(
                new SimpleSessionStorage(),
                new[] { "CFC.Infrastructure.dll" },
                new AutoPersistenceModelGenerator().Generate(),
                "../../../../Src/CFC.Web.Mvc/NHibernate.config");
            
            IWindsorContainer container = new WindsorContainer();

            container.Register(
                    Component
                        .For(typeof(IEntityDuplicateChecker))
                        .ImplementedBy(typeof(EntityDuplicateChecker))
                        .Named("entityDuplicateChecker"));

            container.Register(
                    Component
                        .For(typeof(ISessionFactoryKeyProvider))
                        .ImplementedBy(typeof(DefaultSessionFactoryKeyProvider))
                        .Named("sessionFactoryKeyProvider"));

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
           
            Console.WriteLine("Connections initialized. What would you like to do?");
            Console.WriteLine("[D]emo Environment Setup");
            Console.WriteLine("[Q]uit");
            Console.WriteLine("Press a key...");
            bool validKey = false;
            string key = "";

            if (args.Count() > 0)
            {
                key = args[0].ToString();
                validKey = true;
            }

            while (!validKey)
            {
                key = Console.ReadKey().Key.ToString().ToLower();
                switch(key)
                {
                    case "d":
                        validKey = true;
                        SetupDemoSite();
                        break;
                    case "q":
                        validKey = true;
                        break;
                    default:
                        Console.Write("Invalid choice. Press a key...");
                        break;
                }
            }

            NHibernateSession.CloseAllSessions();
            NHibernateSession.Reset();
        }

        private static void CreateDb()
        {
            new SchemaExport(_configuration).Execute(false, true, false);
            
            /*//Run SQL scripts if needed
            RunScript(@"Install\####.sql");
             */
            Thread.Sleep(2000);
        }

        public static void RunScript(string filename)
        {
            // Get path to item in Build/Scripts folder. 
            // Relative to executable path of compiled TPR.Deploy executable.
            var a = Assembly.GetEntryAssembly();
            var baseDir = System.IO.Path.GetDirectoryName(a.Location);
            var fullPath = System.IO.Path.Combine("../../../../Build/Scripts/", filename);
            var script = System.IO.File.ReadAllText(fullPath);
            var stringSeparators = new string[] { "\nGO" };
            var lines = script.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var query in lines)
            {
                var command = NHibernateSession.Current.Connection.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }



        private static void SetupDemoSite()
        {
            CreateDb();

            #region Content Creation

            Console.WriteLine("Creating content...");
            CreateContent();

            #endregion

            /*
            CheckHostUrls();
             */
        }

        private static void CheckHostUrls()
        {
            bool success = false;
            try
            {
                PingReply pr = new Ping().Send("sandbox.####.com");
                success = (pr.Status == IPStatus.Success);
            } catch(Exception)
            {
                
            }

            if (!success)
            {
                Console.WriteLine("====================================WARNING=====================================");
                Console.WriteLine("Could not reach sandbox.####.com. In order to use the Facebook API");
                Console.WriteLine("you will need to add this line to your hosts file:\n");
                Console.WriteLine("127.0.0.1\t\tsandbox.####.com\n");
                Console.WriteLine("Hit any key to continue...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        private static string RandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            var random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        private static void CreateContent()
        {
            Page page = null;

            #region Home

            page = new Page
            {
                Name = "home-main",
                Content = @"<h1>Hello World</h1>
<p>Welcome</p>
<blockquote> Home-Main</blockquote>
<p>Thank You!.</p>"
            };
            _pageRepository.Save(page);

            page = new Page
            {
                Name = "home-sidebar",
                Content = @"<p></p>"
            };
            _pageRepository.Save(page);

            #endregion
        }
    }
}
