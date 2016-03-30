using System;

namespace LazyEntityGraph.Core
{
    public interface IInstanceCreator
    {
        object Create(Type type);
    }

    public static class InstaceCreatorExtensions
    {
        public static T Create<T>(this IInstanceCreator creator)
        {
            return (T)creator.Create(typeof(T));
        }
    }
}