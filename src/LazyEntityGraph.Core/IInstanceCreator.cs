using System;

namespace LazyEntityGraph.Core
{
    public interface IInstanceCreator
    {
        object Create(Type type);
    }
}