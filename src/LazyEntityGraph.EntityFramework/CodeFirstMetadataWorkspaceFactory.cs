using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace LazyEntityGraph.EntityFramework
{
    internal static class CodeFirstMetadataWorkspaceFactory<TContext> where TContext : DbContext
    {
        private static MetadataWorkspace _cachedWorkspace;
        private static readonly object _syncRoot = new object();

        private static string GetCacheName()
        {
            var migration = typeof(TContext).Assembly.GetExportedTypes()
                .Where(typeof(DbMigration).IsAssignableFrom)
                .Select(Activator.CreateInstance)
                .Cast<IMigrationMetadata>()
                .Select(x => x.Id)
                .OrderByDescending(x => x)
                .FirstOrDefault()
                   ?? "NoMigration";

            return typeof(TContext).Name + "_" + migration + ".edmx";
        }

        public static MetadataWorkspace GetMetadataWorkspace(Func<string, TContext> createFromConnectionString, bool hardCache)
        {
            if (_cachedWorkspace != null)
                return _cachedWorkspace;

            var cacheName = GetCacheName();

            if (hardCache && File.Exists(cacheName))
            {
                var xDoc = XDocument.Load(cacheName);
                return _cachedWorkspace = GetMetadataWorkspace(xDoc);
            }

            lock (_syncRoot)
            {
                // check again inside lock
                if (hardCache && File.Exists(cacheName))
                {
                    var xDoc = XDocument.Load(cacheName);
                    return _cachedWorkspace = GetMetadataWorkspace(xDoc);
                }

                Database.SetInitializer<TContext>(null);

                using (var ctx = createFromConnectionString("App=EntityFramework;Connection Timeout=1;ConnectRetryCount=0"))
                using (var ms = new MemoryStream())
                using (var writer = new XmlTextWriter(ms, Encoding.UTF8))
                {
                    EdmxWriter.WriteEdmx(ctx, writer);

                    if (hardCache)
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        using (var file = File.Create(cacheName))
                            ms.WriteTo(file);
                    }

                    ms.Seek(0, SeekOrigin.Begin);
                    var xDoc = XDocument.Load(ms);
                    return _cachedWorkspace = GetMetadataWorkspace(xDoc);
                }
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