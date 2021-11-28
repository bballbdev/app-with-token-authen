using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Configuration;
using AppWithTokenAuthen.Handlers;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.FileSystems;
using AppWithTokenAuthen.Providers;
using Microsoft.Owin.Security.OAuth;

namespace AppWithTokenAuthen.App_Start
{
    public class TrimmingConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
                if (reader.Value != null)
                    return (reader.Value as string).Trim();
            if (reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Float)
                return reader.Value?.ToString()?.Trim();

            return reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var text = (string)value;
            if (text == null)
                writer.WriteNull();
            else
                writer.WriteValue(text.Trim());
        }
    }



    public static class Helper
    {
        public static void SerializerSettings(this HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new TrimmingConverter());
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
        }

        public static void AddCustomExceptionHandler(this HttpConfiguration config)
        {
            config.Services.Replace(typeof(IExceptionHandler), new CustomExceptionHandler());
        }
    }



    public class AppSeed
    {
        //private static void RegisterRepoAndService(ContainerBuilder builder)
        //{
        //    var service = Assembly.GetAssembly(typeof(UtilsService));
        //    builder.RegisterAssemblyTypes(service)
        //        .Where(t => t.Name.EndsWith("Service"))
        //        .InstancePerRequest()
        //        .AsImplementedInterfaces()
        //        .PropertiesAutowired();

        //    RegisterNLog(builder);
        //}

        //public static void RegisterAutoFac(HttpConfiguration config, IAppBuilder app)
        //{
        //    var builder = new ContainerBuilder();
        //    builder.RegisterWebApiFilterProvider(config);
        //    //RegisterRepoAndService(builder);
        //    builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();
        //    //builder.RegisterControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();
        //    var container = builder.Build();
        //    //DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        //    GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);  //Set the WebApi DependencyResolver
        //    config.DependencyResolver = GlobalConfiguration.Configuration.DependencyResolver;
        //    app.UseAutofacMiddleware(container);
        //    //app.UseAutofacMvc();
        //    app.UseAutofacWebApi(config);
        //}

        public static void RegisterStaticFile(IAppBuilder app)
        {
#if DEBUG
            app.UseCors(CorsOptions.AllowAll);
#endif
            app.UseFileServer(GetAngularFileServer(AppDomain.CurrentDomain.BaseDirectory));

        }

        private static FileServerOptions GetAngularFileServer(string root)
        {
            var physicalFileSystem = new PhysicalFileSystem(Path.Combine(root, "wwwroot"));
            var options = new FileServerOptions
            {
                RequestPath = PathString.Empty,
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = false;
            return options;
        }

        //public static void RegisterNLog(ContainerBuilder builder)
        //{
        //    builder.Register((context) => LogManager.GetLogger("application"))
        //            .As<ILogger>()
        //            .InstancePerRequest()
        //            .PropertiesAutowired();
        //}
    }
}