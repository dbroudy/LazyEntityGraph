using System.ComponentModel.DataAnnotations;

namespace LazyEntityGraph.EntityFramework.Tests.Model
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}