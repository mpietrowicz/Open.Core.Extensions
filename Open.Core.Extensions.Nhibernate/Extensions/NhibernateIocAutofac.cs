using System;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Open.Core.Extensions.Nhibernate.Factorys;
using Open.Core.Extensions.Nhibernate.Interfaces;

namespace Open.Core.Extensions.Nhibernate.Extensions
{
    public static class NhibernateIocAutofac
    {
        
      public static ContainerBuilder RegisterNhSqlLite(this ContainerBuilder serviceCollection,
            params Assembly[] mapping)
        {
            return RegisterNhSqlLite(serviceCollection, String.Empty, false, true,0, mapping);
        }
        
        public static ContainerBuilder RegisterNhSqlLite(this ContainerBuilder serviceCollection,
            string databaseFile,
            params Assembly[] mapping)
        {
            return RegisterNhSqlLite(serviceCollection, String.Empty, false, true,0, mapping);
        }

        public static ContainerBuilder RegisterNhSqlLite(this ContainerBuilder serviceCollection, bool cache,
            bool exposeConfiguration, int AdoNetBatchSize, params Assembly[] mapping)
        {
            return RegisterNhSqlLite(serviceCollection, String.Empty, cache, exposeConfiguration,0,
                mapping);
        }

        public static ContainerBuilder RegisterNhSqlLite(this ContainerBuilder serviceCollection,
            string databaseFile, bool cache, bool exposeConfiguration, int AdoNetBatchSize, params Assembly[] mapping)
        {
            if (string.IsNullOrEmpty(databaseFile))
            {
                databaseFile =
                    $"database.db";
            }

            var nh = new NhibernateFactorySqlLite($"{databaseFile}");
            if (cache)
            {
                nh.UseSecondLevelCache();
            }

            if (exposeConfiguration)
            {
                nh.ExposeConfiguration();
            }

            if (mapping.Length>0)
            {
                nh.AddMappings(mapping);
            }

            if (AdoNetBatchSize >0)
            {
                nh.SetAdoNetBatchSize(AdoNetBatchSize);
            }

            serviceCollection.RegisterInstance(nh).As<INhibernateFactory>().SingleInstance();
            serviceCollection.Register(c => c.Resolve<INhibernateFactory>().GetCurrentStatelessSession()).InstancePerLifetimeScope();
            serviceCollection.Register(c => c.Resolve<INhibernateFactory>().GetCurrentSession()).InstancePerLifetimeScope();
            return serviceCollection;
        }
        
        
        public static ContainerBuilder RegisterNhGenericRepository(this ContainerBuilder serviceCollection)
        {
            serviceCollection.RegisterGeneric( typeof(GenericRepository<>)).As(typeof(IGenericRepository<>)).InstancePerLifetimeScope();
            serviceCollection.RegisterGeneric( typeof(GenericRepositoryStateless<>)).As(typeof(IGenericRepositoryStateless<>)).InstancePerLifetimeScope();
            return serviceCollection;
        }
    }
}