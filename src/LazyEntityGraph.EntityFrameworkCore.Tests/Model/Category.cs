namespace LazyEntityGraph.EntityFrameworkCore.Tests.Model
{
    public class Category : LocalizedEntity<CategoryLocalized>
    {
        public string CategoryName { get; set; }
    }
}