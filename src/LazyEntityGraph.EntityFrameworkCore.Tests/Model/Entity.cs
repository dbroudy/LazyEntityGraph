using System.ComponentModel.DataAnnotations;

namespace LazyEntityGraph.EntityFrameworkCore.Tests.Model
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}