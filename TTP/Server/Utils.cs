using System;
using System.IO;
using System.Reflection;
using JKang.EventSourcing.Domain;
using Microsoft.Extensions.DependencyInjection;
using Server.Repository;

namespace Server
{
    public static class MyExtensions
    {
        public static IServiceCollection AddTextFileStoreForType<T>(this IServiceCollection source) where T : Aggregate<Guid>
        {
            return source
                .AddScoped<IRepository<T>, Repository<T>>()
                .AddEventSourcing(builder =>
                {
                    var path = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Logs/Unit";
                    if (Directory.Exists(path) == false)
                    {
                        Directory.CreateDirectory(path);
                    }

                    builder.UseTextFileEventStore<T, Guid>(x =>
                        x.Folder = path);
                });
        }
    }
}