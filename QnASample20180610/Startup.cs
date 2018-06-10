using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.Ai.QnA;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace QnASample20180610
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBot<QnAMakerBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);

                // Host・EndpointKey・KnowledgeBaseIDはappsettings.Development.jsonファイルに記述。無い場合は追加
                var host = Configuration.GetSection("QnAMaker-Host")?.Value;
                var endpointKey = Configuration.GetSection("QnAMaker-EndpointKey").Value;
                var knowledgeBaseId = Configuration.GetSection("QnAMaker-KnowledgeBaseId").Value;

                var qnaEndpoint = new QnAMakerEndpoint
                {
                    // add subscription key and knowledge base id
                    Host = host,
                    EndpointKey = endpointKey,
                    KnowledgeBaseId = knowledgeBaseId
                };

                var middleware = options.Middleware;
                middleware.Add(new QnAMakerMiddleware(qnaEndpoint));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }
    }
}