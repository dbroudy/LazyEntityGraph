using System.Collections.Generic;
using LazyEntityGraph.EntityFrameworkCore.Tests.Model.TPH;

namespace LazyEntityGraph.EntityFrameworkCore.Tests.Model
{
    public class User : Entity
    {
        public string Username { get; set; }

        public virtual ContactDetails ContactDetails { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}