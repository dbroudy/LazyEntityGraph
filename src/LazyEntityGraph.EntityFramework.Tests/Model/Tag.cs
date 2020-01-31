using System.Collections.Generic;
using LazyEntityGraph.EntityFramework.Tests.Model.TPH;

namespace LazyEntityGraph.EntityFramework.Tests.Model
{
    public class Tag : Entity
    {
        public string TagName { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}