using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.Linq;
using Open.Core.Extensions.Nhibernate.Extensions;
using Open.Core.Extensions.Nhibernate.Factorys;
using Open.Core.Extensions.Nhibernate.Tests.Maps;
using Xunit;

namespace Open.Core.Extensions.Nhibernate.Tests
{
    public class NhibernateFactoryUnitTests
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
                Id = id,
                Name = "Test"
            });
            await session.FlushAsync();
            var resp = await session.QueryOver<Image>().ListAsync();
            Assert.NotNull(resp);
            Assert.Equal("Test",resp.FirstOrDefault()?.Name);
            Assert.Equal(id,resp.FirstOrDefault()?.Id);
            Assert.Null(resp.FirstOrDefault()?.image);

        }

        [Fact]
        public void ExtensionsRegisterSessionAndStatelessSession()
        {
            var serviceProvider = new ServiceCollection()
                .RegisterNhSqlLite(GetType().Assembly)
                .BuildServiceProvider();

            var session = serviceProvider.GetService<ISession>();
            var sessionStatles = serviceProvider.GetService<IStatelessSession>();
            
            Assert.NotNull(session);
            Assert.NotNull(sessionStatles);
        }
    }
}