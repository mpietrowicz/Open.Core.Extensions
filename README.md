# Open.Core.Extensions 
## Fast extensions currently only Fluent Nhibernate (.net netcoreapp3.1, net5.0, netstandard2.1)
1. Add reference from nuget Open.Core.Extensions.Nhibernate
2. Register simple Nhibernate Factory in Microsoft ioc using SqlLite :

Register in  `Startup -> ConfigureServices` as so:
```c#
        public void ConfigureServices(IServiceCollection services)
        {
            ...            
            services.RegisterNhSqlLite(GetType().Assembly);
            ...
        }
```


Auto configuration :
- The name of the database will be `database.db` by default.
- Second Level Cache will be `disabled`.
- Expose Configuration will be `enabled`.
- Delete on startup  will be `disabled`;
- Mappings will be register in current residing `Assembly`;


You can resolve the interfaces after registering `services.RegisterNhSqlLite(GetType().Assembly);`
- ISession `Scoped`
```c#
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using NHibernate;
    
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private ISession Session { get; }

        public WeatherForecastController(ISession session )
        {
            Session = session;
        }
    }
```
- IStatelessSession `Scoped`

```c#
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using NHibernate;
    
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IStatelessSession StatelessSession { get; }

        public WeatherForecastController(IStatelessSession statelessSession)
        {
            StatelessSession = statelessSession;
        }
    }
```

