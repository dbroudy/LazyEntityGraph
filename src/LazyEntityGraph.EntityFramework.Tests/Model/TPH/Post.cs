using System;
using System.Collections.Generic;

namespace LazyEntityGraph.EntityFramework.Tests.Model.TPH
{
    public class Post : LocalizedEntity<PostLocalized>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DatePublished { get; set; }

        public int PosterId { get; set; }
        public virtual User Poster { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}