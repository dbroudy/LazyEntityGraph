using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LazyEntityGraph.EntityFramework.Tests.Model.TPC
{
    public abstract class Invoice : Entity
    {
        public DateTime Date { get; set; }
        
        public double Sum { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        
        public virtual User User { get; set; }
    }
}
