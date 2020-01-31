using System.ComponentModel.DataAnnotations;

namespace LazyEntityGraph.EntityFrameworkCore.Tests.Model
{
    public class ContactDetails
    {
        [Key]
        public virtual int UserId { get; set; }
        public virtual User User { get; set; }
    }
}