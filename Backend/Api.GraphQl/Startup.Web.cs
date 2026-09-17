using Asp.Versioning.ApiExplorer;
using BeCause.NuGet.Core.Domain.Converters;
using BeCause.NuGet.Exceptions.Api;
using BeCause.NuGet.IdentityServer.Swagger;
using BeCause.NuGet.Versioning;
using HealthTracker.Api.GraphQl.ErrorHandling;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace HealthTracker.Api.GraphQl;

internal partial class Startup
{  
    private void ConfigureWebServices(IServiceCollection services)
    {
        services.AddMajorVersionedApi(1);

        services
            .AddGraphQLServer()
            .AddAuthorization()
            .ModifyOptions(opts => opts.StrictValidation = false)
            .BindRuntimeType<uint, UnsignedIntType>()
            //.BindRuntimeType<MediatR.INotification, AnyType>()
            .BindRuntimeType<System.Text.Json.JsonDocument, StringType>()
            .AddQueryType(q => q.Name("Query"))
            //.AddAuthorization()
            .AddSorting()
            .AddFiltering()
            .AddProjections()
            .RegisterDbContextFactory<ReadApplicationDbContext>()
            .AddQueryType<Queries.DailyRecordQuery>()
            .AddMutationType<Mutations.Mutation>()
            //.AddResolutionGraphQL()
            //.AddTopicGraphQL()
            .AddErrorFilter<GraphQLErrorFilter>();

        services.AddBeCauseExceptionFilters();

        services.AddControllers(options => options.UseBeCauseExceptionFilters())
            .AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                opts.JsonSerializerOptions.AddEnumerationJsonConverters([typeof(Startup).Assembly]);
                opts.JsonSerializerOptions.AddFlagsValueObjectJsonConverters([typeof(Startup).Assembly]);
            }
            );
        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
    }

    private void ConfigureWebPipeline(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        app.UsePathBase(Configuration["BasePath"]);

        app.UseRouting();

        app.UseEndpoints(endpoints => { endpoints.MapGraphQL(); });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        if (!env.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }
    }
}
