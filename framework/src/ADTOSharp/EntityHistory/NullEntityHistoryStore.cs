using System.Threading.Tasks;

namespace ADTOSharp.EntityHistory
{
    public class NullEntityHistoryStore : IEntityHistoryStore
    {
        public static NullEntityHistoryStore Instance { get; } = new NullEntityHistoryStore();

        public Task SaveAsync(EntityChangeSet entityChangeSet)
        {
            return Task.CompletedTask;
        }

        public void Save(EntityChangeSet entityChangeSet)
        {
        }
    }
}
