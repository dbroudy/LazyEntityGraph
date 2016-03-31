using System;

namespace LazyEntityGraph.Core
{
    public interface IInstanceCreator
    {
        object Create(Type type);
    }

    public static class InstanceCreatorExtensions
    {
        public static T Create<T>(this IInstanceCreator creator)
        {
            return (T)creator.Create(typeof(T));
        }
    }
}