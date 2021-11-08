using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate;
using NHibernate.Mapping;
using Open.Core.Extensions.Nhibernate.Interfaces;

namespace Open.Core.Extensions.Nhibernate
{
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
}