using System;
using System.Threading.Tasks;
using JKang.EventSourcing.Domain;
using JKang.EventSourcing.Persistence;
using JKang.EventSourcing.Snapshotting.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Repository
{
    public interface IRepository<T>
    {
        Task SaveAsync(T obj);
        Task<T> FindAsync(Guid id);
    }
    
    public class Repository<T> : AggregateRepository<T, Guid>, IRepository<T> where T : Aggregate<Guid>
    {
        public Repository(IEventStore<T, Guid> eventStore, ISnapshotStore<T, Guid> snapshotStore)
            : base(eventStore, snapshotStore)
        { }

        public Task SaveAsync(T gameWorld) =>
            this.SaveAggregateAsync(gameWorld);

        public Task<T> FindAsync(Guid id) =>
            FindAggregateAsync(id);
    }

    public static class RepositoryUtility
    {
        public static void Flush<T>(T entity) where T : Aggregate<Guid>
        {
            using (var scope = ServerMain.ServiceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<IRepository<T>>();
                handler.SaveAsync(entity);
            }
        }
    }
}