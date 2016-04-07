using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;

namespace LazyEntityGraph.EntityFramework
{
    internal static class EdmxMetadataWorkspaceFactory<TContext> where TContext : DbContext
    {
        private static MetadataWorkspace _cached;

        public static MetadataWorkspace GetMetadataWorkspace(string contextName)
        {
            if (_cached != null)
                return _cached;

            var paths = new[] { $"res://*/{contextName}.csdl", $"res://*/{contextName}.ssdl", $"res://*/{contextName}.msl" };
            var assembliesToConsider = new[] { typeof(TContext).Assembly };
            return _cached = new MetadataWorkspace(paths, assembliesToConsider);
        }
    }
}