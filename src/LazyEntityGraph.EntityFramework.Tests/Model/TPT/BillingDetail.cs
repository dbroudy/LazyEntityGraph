using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LazyEntityGraph.EntityFramework.Tests.Model.TPT
{
    public abstract class BillingDetail
    {
        [Key]
        public int BillingDetailId { get; set; }
        
        public string Number { get; set; }
        
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        
        public virtual User User { get; set; }
    }
}