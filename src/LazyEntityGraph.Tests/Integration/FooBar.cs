using System.Collections.Generic;

namespace LazyEntityGraph.Tests.Integration
{
    public class Foo
    {
        public int Id { get; set; }
        public int BarId { get; set; }
        public virtual Bar Bar { get; set; }
        public virtual ICollection<Bar> Bars { get; set; }
    }

    public class Bar
    {
        public int Id { get; set; }
        public virtual Foo Foo { get; set; }
        public virtual ICollection<Foo> Foos { get; set; }
    }
}
