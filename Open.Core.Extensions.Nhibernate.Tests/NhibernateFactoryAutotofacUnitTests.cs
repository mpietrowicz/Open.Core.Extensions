using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using Open.Core.Extensions.Nhibernate.Extensions;
using Open.Core.Extensions.Nhibernate.Factorys;
using Open.Core.Extensions.Nhibernate.Tests.Maps;
using Xunit;

namespace Open.Core.Extensions.Nhibernate.Tests
{
    public class NhibernateFactoryAutotofacUnitTests
    {
        [Fact]
        public async Task Build()
        {
            
            var tc = new NhibernateFactorySqlLite("test_db.db");
            tc.AddMappings(GetType().Assembly);
            tc.DeleteOnInitDatabase();
            tc.UseSecondLevelCache();
            tc.ExposeConfiguration();
            var session = tc.GetCurrentSession();

            var response = await session.QueryOver<Image>().SingleOrDefaultAsync();

            var id = Guid.NewGuid().ToString();
            Assert.Null(response);
            await session.SaveAsync(new Image()
            {
                Name = "Test"
            });
            await session.FlushAsync();
            var resp = await session.QueryOver<Image>().ListAsync();
            Assert.NotNull(resp);
            Assert.Equal("Test",resp.FirstOrDefault()?.Name);
            Assert.Null(resp.FirstOrDefault()?.image);

        }

        [Fact]
        public void ExtensionsRegisterSessionAndStatelessSession()
        {
            var serviceProvider = new ContainerBuilder()
                .RegisterNhSqlLite(GetType().Assembly)
                .Build();

            var session = serviceProvider.Resolve<ISession>();
            var sessionStatles = serviceProvider.Resolve<IStatelessSession>();
            
            Assert.NotNull(session);
            Assert.NotNull(sessionStatles);
        }
        
        [Fact]
        public async Task ExtensionsRegisterGenericRepository()
        {
            var serviceProvider = new ContainerBuilder()
                .RegisterNhSqlLite(GetType().Assembly)
                .RegisterNhGenericRepository()
                .Build();

            var session = serviceProvider.Resolve<ISession>();
            var sessionStatles = serviceProvider.Resolve<IStatelessSession>();
            var genericRepository = serviceProvider.Resolve<IGenericRepository<Image>>();


            var all = await genericRepository.Get();

            var name = Guid.NewGuid().ToString();
            await genericRepository.Insert(new Image()
            {
                Name = name
            });
            all = await genericRepository.Get();

            var find = await genericRepository.Find(x => x.Name == name);
           
            Assert.NotNull(all);
            Assert.NotNull(find);
            Assert.NotNull(session);
            Assert.NotNull(sessionStatles);
        }
        
        [Fact]
        public async Task ExtensionsRegisterStatlessGenericRepository()
        {
            var serviceProvider = new ContainerBuilder()
                .RegisterNhSqlLite(GetType().Assembly)
                .RegisterNhGenericRepository()
                .Build();

            var session = serviceProvider.Resolve<ISession>();
            var sessionStatles = serviceProvider.Resolve<IStatelessSession>();
            var genericRepository = serviceProvider.Resolve<IGenericRepositoryStateless<Image>>();


            var all = await genericRepository.Get();

            var name = Guid.NewGuid().ToString();
            await genericRepository.Insert(new Image()
            {
                Name = name
            });
            all = await genericRepository.Get();

            var find = await genericRepository.Find(x => x.Name == name);
           
            Assert.NotNull(all);
            Assert.NotNull(find);
            Assert.NotNull(session);
            Assert.NotNull(sessionStatles);
        }
    }
}