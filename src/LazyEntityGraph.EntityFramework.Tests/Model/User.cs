using System.Collections.Generic;
using LazyEntityGraph.EntityFramework.Tests.Model.TPC;
using LazyEntityGraph.EntityFramework.Tests.Model.TPH;
using LazyEntityGraph.EntityFramework.Tests.Model.TPT;

namespace LazyEntityGraph.EntityFramework.Tests.Model
{
    public class User : Entity
    {
        public string Username { get; set; }

        public virtual ContactDetails ContactDetails { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        
        public virtual ICollection<BillingDetail> BillingDetails { get; set; }
        
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
