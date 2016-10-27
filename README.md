# Microsoft.AspNetCore.ApiHelp

A toolchain for ASP.NET Core to automatically generating API documentation.

# Features
- [x] Read XML comments at run time (so the deployment need to include yourAssembly.xml file).
- [x] Get **Enum** raw constant value and XML comments.

    ```C#
    public enum Gender {
        /// <summary>
        /// 未知.
        /// </summary>
        Unknown,
        /// <summary>
        /// 男性.
        /// </summary>
        Male,
        /// <summary>
        /// 女性.
        /// </summary>
        Female,
    }
    ```
    
    ```JSON
    {
      "0": "未知", 
      "1": "男性", 
      "2": "女性"
    }
    ```
- [x] Generate API documentation for ASP.NET Core.
- [x] JsonOutputFormatter use camel case or not determined by the API (Because it's API).
- [x] More...

# Packages
- [Microsoft.AspNetCore.ApiHelp.Core](https://www.nuget.org/packages/Microsoft.AspNetCore.ApiHelp.Core/1.0.0)

    The core API toolchain for ASP.NET Core.

- [Microsoft.AspNetCore.ApiHelp](https://www.nuget.org/packages/Microsoft.AspNetCore.ApiHelp/1.0.0)

    The three styles UI (Swagger, JsonH, JsonEditor) for Microsoft.AspNetCore.ApiHelp.Core.

# How to use

## Install package.

`PM> Install-Package Microsoft.AspNetCore.ApiHelp`

## Configure Startup.cs.

```csharp
public void ConfigureServices(IServiceCollection services) {
    services.AddMvc()
        .AddApiHelp(options => {
            options.IgnoreObsoleteApi = true;
            options.GenerateStrategy = DocumentGenerateStrategy.Eager;
            options.IncludeSupportedMediaType = false;
        });
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
    loggerFactory.AddConsole(Configuration.GetSection("Logging"));

    app.UseApiHelp(new ApiHelpUIOptions {
        Title = "API toolchain for ASP.NET Core",
        UI = ApiHelpUI.Swagger,
    });

    app.UseMvc();
}
```

# Demo

How to integrate `ApiHelp` to your project, see [Host](src/Host), and open link [api/help/ui](http://localhost:5000/api/help/ui/)
to view the result.