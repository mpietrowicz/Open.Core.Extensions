using System;
using System.IO;
using System.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Open.Core.Extensions.Nhibernate.Factorys
{
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