namespace LazyEntityGraph.EntityFrameworkCore.Tests.Model
{
    public abstract class Localization : Entity
    {
        public string LanguageCode { get; set; }
    }
}