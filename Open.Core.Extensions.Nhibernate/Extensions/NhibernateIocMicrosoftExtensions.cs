using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using Open.Core.Extensions.Nhibernate.Factorys;
using Open.Core.Extensions.Nhibernate.Interfaces;

namespace Open.Core.Extensions.Nhibernate.Extensions
{
    public static class NhibernateIocMicrosoftExtensions
    {
        public static IServiceCollection RegisterNhSqlLite(this IServiceCollection serviceCollection,
            params Assembly[] mapping)
        {
            return RegisterNhSqlLite(serviceCollection as ServiceCollection, String.Empty, false, true, mapping);
        }
        
        public static IServiceCollection RegisterNhSqlLite(this IServiceCollection serviceCollection,
            string databaseFile,
            params Assembly[] mapping)
        {
            return RegisterNhSqlLite(serviceCollection as ServiceCollection, String.Empty, false, true, mapping);
        }

        public static IServiceCollection RegisterNhSqlLite(this IServiceCollection serviceCollection, bool cache,
            bool exposeConfiguration, params Assembly[] mapping)
        {
            return RegisterNhSqlLite(serviceCollection as ServiceCollection, String.Empty, cache, exposeConfiguration,
                mapping);
        }

        public static IServiceCollection RegisterNhSqlLite(this IServiceCollection serviceCollection,
            string databaseFile, bool cache, bool exposeConfiguration, params Assembly[] mapping)
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

            serviceCollection.AddSingleton(typeof(INhibernateFactory),nh);
            serviceCollection.AddScoped(provider => nh.GetCurrentSession());
            serviceCollection.AddScoped(provider => nh.GetCurrentStatelessSession());
            return serviceCollection;
        }
    }
}