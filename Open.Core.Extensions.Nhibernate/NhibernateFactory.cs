using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping;
using NHibernate.Tool.hbm2ddl;

namespace Open.Core.Extensions.Nhibernate
{
    public interface INhibernateFactory
    {
        ISession GetCurrentSession();
        void CloseSession();
        void CloseSessionFactory();

        void ExposeConfiguration();
        abstract ISessionFactory FluentConfigure();
        void UseSecondLevelCache();
        void AddMappings(Assembly assembly);
        void AddMappings<TMapClass>() where TMapClass : class;
    }
    public abstract class NhibernateFactory : INhibernateFactory
    {


        protected ISessionFactory _sessionFactory;
        protected ISessionFactory SessionFactory
        {
            get
            {
                return _sessionFactory ??= FluentConfigure();

            }
            set => _sessionFactory = value;
        }

        protected bool exposeConfiguration { get; set; }
        protected bool useSecondLevelCache { get; set; }

        public ISession GetCurrentSession()
        {
            return SessionFactory.OpenSession();
        }
        public IStatelessSession GetCurrentStatelessSession()
        {
            return SessionFactory.OpenStatelessSession();
        }

        public void CloseSession()
        {
            SessionFactory.Close();
        }

        public void CloseSessionFactory()
        {
            if (SessionFactory != null)
            {
                SessionFactory.Close();
            }
        }

        public void ExposeConfiguration()
        {
            exposeConfiguration = true;
        }

        protected List<Assembly> _assemblesToRegister = new List<Assembly>();
        public void UseSecondLevelCache()
        {
            useSecondLevelCache = true;
        }

        public void AddMappings(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            _assemblesToRegister.Add(assembly);
        }
        public void AddMappings(Assembly[] assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            _assemblesToRegister.AddRange(assembly);
        }

        public void AddMappings<TMapClass>() where TMapClass : class
        {
            _assemblesToRegister.Add(typeof(TMapClass).Assembly);
        }

        public abstract ISessionFactory FluentConfigure();
    }
  
    public class NhibernateFactorySqlLite : NhibernateFactory
    {
        private string PathToDatabaseFile { get; }

        public void DeleteOnInitDatabase()
        {
            DeleteDatabase = true;
        }

        private bool DeleteDatabase { get; set; }

        public NhibernateFactorySqlLite(string pathToDatabaseFile)
        {
            if (string.IsNullOrEmpty(pathToDatabaseFile))
                throw new ArgumentException("Value cannot be null or empty.", nameof(pathToDatabaseFile));
            PathToDatabaseFile = pathToDatabaseFile;
        }
        public sealed override ISessionFactory FluentConfigure()
        {
            var config =  Fluently.Configure();
                //which database
                config = config.Database(
                    SQLiteConfiguration.Standard
                        .UsingFile(PathToDatabaseFile)
                );
                
                config = config.Cache(
                    c => c.UseQueryCache()
                        .UseSecondLevelCache()
                        .ProviderClass<NHibernate.Cache.HashtableCacheProvider>());
                if (_assemblesToRegister.Any())
                {
                    config = config.Mappings(m => _assemblesToRegister.ForEach(x=> m.FluentMappings.AddFromAssembly(x)));
                }
                if (exposeConfiguration)
                {
                    config = config.ExposeConfiguration(BuildSchema);
                }
                
                return config.BuildSessionFactory();
        }
        private void BuildSchema(Configuration config)
        {
            // delete the existing db on each run
            if (DeleteDatabase && File.Exists(PathToDatabaseFile))
                File.Delete(PathToDatabaseFile);

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
                .Create(false, true);
        }


       
    }
}