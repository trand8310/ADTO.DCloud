using System.Linq;
using ADTOSharp.Dependency;

namespace ADTOSharp.ObjectMapping
{
    public sealed class NullObjectMapper : IObjectMapper, ISingletonDependency
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullObjectMapper Instance { get; } = new NullObjectMapper();

        public TDestination Map<TDestination>(object source)
        {
            throw new ADTOSharpException("ADTOSharp.ObjectMapping.IObjectMapper should be implemented in order to map objects.");
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            throw new ADTOSharpException("ADTOSharp.ObjectMapping.IObjectMapper should be implemented in order to map objects.");
        }

        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source)
        {
            throw new ADTOSharpException("ADTOSharp.ObjectMapping.IObjectMapper should be implemented in order to map objects.");
        }
    }
}
