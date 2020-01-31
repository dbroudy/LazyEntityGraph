using System.ComponentModel.DataAnnotations.Schema;

namespace LazyEntityGraph.EntityFramework.Tests.Model
{
    public class CategoryLocalized : Localization
    {
        [ForeignKey(nameof(LocalizationFor))]
        public int CategoryId { get; set; }
        
        public string CategoryName { get; set; }
        
        public virtual Category LocalizationFor { get; set; }
    }
}