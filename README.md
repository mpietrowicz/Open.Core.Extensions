# Open.Core.Extensions 
## Fast extensions currently only Fluent Nhibernate (.net netcoreapp3.1, net5.0, netstandard2.1)
1. Add reference from nuget Open.Core.Extensions.Nhibernate
2. Register simple Nhibernate Factory in Microsoft ioc using SqlLite :

Register in  Startup -> ConfigureServices

```c#
        public void ConfigureServices(IServiceCollection services)
        {
            ...            
            services.RegisterNhSqlLite(GetType().Assembly);
            ...
        }
```
You can resolve the interfaces after registering services.RegisterNhSqlLite(GetType().Assembly);
- ISession (scoped)
- IStatelessSession (scoped)