using System;
using System.Configuration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Honeypot.Services;
using Honeypot.Web.Providers;
using SharpArch.Domain.Commands;
using SharpArch.Domain.PersistenceSupport;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Contracts.Repositories;
using SharpArch.Web.Mvc.Castle;


namespace Honeypot.Web.Bootstrap
{
    public class ComponentRegistrar
    {
        public static void AddComponentsTo(IWindsorContainer container)
        {
            AddGenericRepositoriesTo(container);
            AddCustomRepositoriesTo(container);
            AddServicesTo(container);
            AddMembershipTo(container);
        }

        private static void AddMembershipTo(IWindsorContainer container)
        {
            container.Register(
                    Component.For<IAuth>().ImplementedBy<FormsAuthenticationWrapper>()
                );
        }

        private static void AddServicesTo(IWindsorContainer container)
        {
            container.Register(
               Component.For<IConfigService>()
                .ImplementedBy<ConfigService>()
                .DependsOn(new
                {
                    appSettings = ConfigurationManager.AppSettings
                }));

            var minLogLevel =
                (Domain.LogLevel)Enum.Parse(typeof(Domain.LogLevel), ConfigurationManager.AppSettings["MinLogLevel"]);

            container.Register(
               Component.For<ILogService>()
                .ImplementedBy<LogService>()
                .DependsOn(new
                {
                    minLevel = minLogLevel
               }));

            container.Register(
                Component.For<IEmailService>()
                    .ImplementedBy<EmailService>());

            container.Register(
                Component.For<ITemplateService>()
                    .ImplementedBy<TemplateService>());

            container.Register(
               Component.For<INotificationService>()
                .ImplementedBy<EmailNotificationService>()
                .DependsOn(new
                {
                    fromEmail = ConfigurationManager.AppSettings["EmailFromAddress"],
                    fromName = ConfigurationManager.AppSettings["EmailFromName"]
                }));

            container.Register(
                AllTypes
                    .FromAssemblyNamed("Honeypot.Services")
                    .Pick().If(t => t.Name.EndsWith("Service"))
                    .WithService
                    .FirstNonGenericCoreInterface("Honeypot.Services"));
        }

        private static void AddCustomRepositoriesTo(IWindsorContainer container)
        {
            container.Register(
                AllTypes
                    .FromAssemblyNamed("Honeypot.Infrastructure")
                    .BasedOn(typeof(ILinqRepositoryWithTypedId<,>))
                    .WithService.FirstNonGenericCoreInterface("Honeypot.Infrastructure"));
        }

        private static void AddGenericRepositoriesTo(IWindsorContainer container)
        {
            container.Register(
                Component.For(typeof(IEntityDuplicateChecker))
                    .ImplementedBy(typeof(EntityDuplicateChecker))
                    .Named("entityDuplicateChecker"));

            container.Register(
                Component.For(typeof(INHibernateRepository<>))
                    .ImplementedBy(typeof(NHibernateRepository<>))
                    .Named("nhibernateRepositoryType")
                    .Forward(typeof(IRepository<>)));

            container.Register(
                Component.For(typeof(INHibernateRepositoryWithTypedId<,>))
                    .ImplementedBy(typeof(NHibernateRepositoryWithTypedId<,>))
                    .Named("nhibernateRepositoryWithTypedId")
                    .Forward(typeof(IRepositoryWithTypedId<,>)));

            container.Register(
                    Component.For(typeof(ISessionFactoryKeyProvider))
                        .ImplementedBy(typeof(DefaultSessionFactoryKeyProvider))
                        .Named("sessionFactoryKeyProvider"));

            container.Register(
                    Component.For(typeof(ICommandProcessor))
                        .ImplementedBy(typeof(CommandProcessor))
                        .Named("commandProcessor"));
        }

    }
}