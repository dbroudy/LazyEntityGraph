using System.Collections.Generic;

namespace LazyEntityGraph.EntityFrameworkCore.Tests.Model
{
    public abstract class LocalizedEntity<T> : Entity
        where T : Localization
    {
        public virtual ICollection<T> Localizations { get; set; }
    }
}