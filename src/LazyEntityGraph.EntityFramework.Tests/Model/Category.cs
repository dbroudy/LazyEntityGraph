namespace LazyEntityGraph.EntityFramework.Tests.Model
{
    public class Category : LocalizedEntity<CategoryLocalized>
    {
        public string CategoryName { get; set; }
    }
}