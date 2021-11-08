using System.Reflection;
using NHibernate;

namespace Open.Core.Extensions.Nhibernate.Interfaces
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
}