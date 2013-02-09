using System;
using System.Configuration;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using Honeypot.Domain;
using Honeypot.Infrastructure;
using Honeypot.Infrastructure.Mapping;
using Honeypot.Services;
using NHibernate.Tool.hbm2ddl;
using SharpArch.Domain.PersistenceSupport;
using SharpArch.NHibernate;
using Microsoft.Practices.ServiceLocation;
using Configuration = NHibernate.Cfg.Configuration;

namespace Honeypot.Build
{
    class Program
    {
        #region Fields

        private static Configuration _configuration;
        private static IUserService _userService;
        private static IPermissionRepository _permissionRepository;
        private static ILogRepository _logRepository;
        private static IResourceRepository _resourceRepository;
        private static IWindsorContainer _container;

        #endregion

        #region Main Routine

        static void Main()
        {
            InitNHibernate();
            InitServiceLocator();

            Console.WriteLine("Connections initialized. What would you like to do?");
            Console.WriteLine("[D]emo Environment Setup");
            Console.WriteLine("[U]ser Interface Testing Setup");
            Console.WriteLine("[Q]uit");
            Console.WriteLine("Press a key...");
            bool validKey = false;

            while (!validKey)
            {
                string key = Console.ReadKey().Key.ToString().ToLower();
                switch(key)
                {
                    case "d":
                        validKey = true;
                        SetupDemoSite();
                        break;
                    case "u":
                        validKey = true;
                        SetupInterfaceTestSite();
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

        #endregion

        #region Setup Methods

        private static void SetupDemoSite()
        {
            CreateDb();
            CreatePermissions();
            CreateUsers(1);
        }

        private static void SetupInterfaceTestSite()
        {
            CreateDb();
            CreatePermissions();
            CreateUsers(300);
            CreateResources();
            CreateLogs(100);
        }

        private static void CreateResources()
        {
            for(int i = 1; i <= 100; i++)
            {
                var category = ResourceCategory.Content;
                if (i <= 30) category = ResourceCategory.Content;
                else if (i <= 70) category = ResourceCategory.Settings;

                var type = ResourceType.Html;
                if (i <= 50) type = ResourceType.Html;
                else if (i <= 100) type = ResourceType.String;

                _resourceRepository.Save(new Resource
                    {
                        Category = category,
                        IsReadOnly = i > 50,
                        Name = "resource-" + i,
                        Value = i.ToString(),
                        Type = type
                    });
            }
        }

        private static void CreateLogs(int numLogs)
        {
            for (int i = 1; i <= numLogs; i++)
            {
                User user = _userService.GetUserById(1);
                var level = LogLevel.Debug;
                if (i <= 20) level = LogLevel.Debug;
                else if (i <= 40) level = LogLevel.Info;
                else if (i <= 80) level = LogLevel.Warning;
                else if (i <= 100) level = LogLevel.Error;

                var category = LogCategory.Application;
                if (i <= 30) category = LogCategory.Application;
                if (i <= 60) category = LogCategory.Security;
                else if (i <= 90) category = LogCategory.System;

                _logRepository.Save(new Log
                    {
                        Category = category,
                        IpAddress = "127.0.0." + i,
                        Level = level,
                        Details = "Log details " + i,
                        LogDate = DateTime.Now.AddDays(-1 * i),
                        Message = "Sample log " + i,
                        User = (i % 2 == 0) ? null : user
                    });
            }
        }

        private static void CreatePermissions()
        {
            _permissionRepository.Save(new Permission
            {
                Name = Permission.EditUsers,
                Description = "Edit users"
            });

            _permissionRepository.Save(new Permission
            {
                Name = Permission.EditResources,
                Description = "Edit resources"
            });
        }

        private static void CreateUsers(int numMembers)
        {
            for (int i = 1; i < numMembers; i++)
            {
                var member = new User
                {
                    Email = "member" + i + "@honeypot.com",
                    Role = Role.Member,
                    ResetPasswordToken = "member" + i
                };
                _userService.GenerateUserPassword(member, "test");
                _userService.CreateUser(member);
            }

            var admin = new User
            {
                Email = "admin@honeypot.com",
                Role = Role.Administrator
            };
            _userService.GenerateUserPassword(admin, "test");
            _userService.CreateUser(admin);
        }

        #endregion

        #region Helper Methods

        private static void InitNHibernate()
        {
            _configuration = NHibernateSession.Init(
                new SimpleSessionStorage(),
                new[] { "Honeypot.Infrastructure.dll" },
                new AutoPersistenceModelGenerator().Generate(),
                "../../../../Src/Honeypot.Web/NHibernate.config");
        }

        private static void InitServiceLocator()
        {
            // Setup Windsor for SharpArchitecture
            _container = new WindsorContainer();
            ComponentRegistrar.AddComponentsTo(_container);
            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(_container));
            InitServices();
        }

        private static void InitServices()
        {
            _userService = _container.Resolve<IUserService>();
            _permissionRepository = _container.Resolve<IPermissionRepository>();
            _resourceRepository = _container.Resolve<IResourceRepository>();
            _logRepository = _container.Resolve<ILogRepository>();
        }

        private static void CreateDb()
        {
            new SchemaExport(_configuration).Execute(false, true, false);
        }

        private static void RunScript(string filename)
        {
            // Get path to item in Build/Scripts folder. 
            // Relative to executable path of compiled executable.
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

        #endregion
    }
}
