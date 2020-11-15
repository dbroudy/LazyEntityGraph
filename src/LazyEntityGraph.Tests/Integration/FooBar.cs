using System.Collections.Generic;

namespace LazyEntityGraph.Tests.Integration
{
    public class Foo
    {
        public Foo()
        {
            Bars = new HashSet<Bar>();
        }

        public int Id { get; set; }
        public int BarId { get; set; }
        public virtual Bar Bar { get; set; }
        public virtual ICollection<Bar> Bars { get; set; }
    }

    public class Bar
    {
        public Bar()
        {
            Foos = new HashSet<Foo>();
        }

        public int Id { get; set; }
        public virtual Foo Foo { get; set; }
        public virtual ICollection<Foo> Foos { get; set; }
    }

    public class Faz : Foo { }

    public class Baz
    {
        public int FooId { get; set; }

        public virtual Foo Foo { get; set; }
    }

    public class MultiFoo
    {
        public int Id { get; set; }

        public long BarId1 { get; set; }
        public string BarId2 { get; set; }
        public virtual MultiBar Bar { get; set; }
    }

    public class MultiBar
    {
        public long Id1 { get; set; }
        public string Id2 { get; set; }
    }
}
