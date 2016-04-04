using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace LazyEntityGraph.EntityFramework
{
    internal static class MetadataWorkspaceFactory<TContext> where TContext : DbContext
    {
        private static MetadataWorkspace _cachedEdmx;
        private static MetadataWorkspace _cachedCodeFirst;

        public static MetadataWorkspace GetMetadataWorkspaceFromEdmx(string contextName)
        {
            if (_cachedEdmx != null)
                return _cachedEdmx;

            return _cachedEdmx = new MetadataWorkspace(new[]
            {
                $"res://*/{contextName}.csdl",
                $"res://*/{contextName}.ssdl",
                $"res://*/{contextName}.msl"
            },
                new[] { typeof(TContext).Assembly });
        }

        private const string HardCacheEdmx = "cached.edmx";

        public static MetadataWorkspace GetMetadataWorkspaceFromCodeFirst(Func<string, TContext> createFromConnectionString, bool hardCache)
        {
            if (_cachedCodeFirst != null)
                return _cachedCodeFirst;

            if (hardCache && File.Exists(HardCacheEdmx))
            {
                var xDoc = XDocument.Load(HardCacheEdmx);
                return _cachedCodeFirst = GetMetadataWorkspace(xDoc);
            }

            using (var ctx = createFromConnectionString("App=EntityFramework"))
            using (var ms = new MemoryStream())
            using (var writer = new XmlTextWriter(ms, Encoding.UTF8))
            {
                EdmxWriter.WriteEdmx(ctx, writer);

                if (hardCache)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    using (var file = File.Create(HardCacheEdmx))
                        ms.WriteTo(file);
                }

                ms.Seek(0, SeekOrigin.Begin);
                var xDoc = XDocument.Load(ms);
                return _cachedCodeFirst = GetMetadataWorkspace(xDoc);
            }
        }

        private static MetadataWorkspace GetMetadataWorkspace(XDocument xDoc)
        {
            var runtime = xDoc.Root.Elements().First(c => c.Name.LocalName == "Runtime");
            var cSpaceLoader = new EdmItemCollection(GetXmlReader(runtime, "ConceptualModels"));
            var sSpaceLoader = new StoreItemCollection(GetXmlReader(runtime, "StorageModels"));
            var mSpaceLoader = new StorageMappingItemCollection(cSpaceLoader, sSpaceLoader,
                GetXmlReader(runtime, "Mappings"));
            return new MetadataWorkspace(() => cSpaceLoader, () => sSpaceLoader, () => mSpaceLoader);
        }

        private static IEnumerable<XmlReader> GetXmlReader(XContainer runtimeElement, string elementName)
        {
            var model = runtimeElement.Elements().First(c => c.Name.LocalName == elementName).Elements().First();
            yield return XmlReader.Create(new StringReader(model.ToString()));
        }
    }
}