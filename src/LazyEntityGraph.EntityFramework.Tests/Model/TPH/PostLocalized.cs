using System.ComponentModel.DataAnnotations.Schema;

namespace LazyEntityGraph.EntityFramework.Tests.Model.TPH
{
    public class PostLocalized : Localization
    {
        [ForeignKey(nameof(LocalizationFor))]
        public int PostId { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
        public virtual Post LocalizationFor { get; set; }
    }
}