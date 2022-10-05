#nullable enable
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace BaseGame
{
    public interface ISimulation
    {
        UniTask Initialize(CancellationToken cancelletatonToken);
        void Add<T>(T obj);
        void Remove<T>(T obj);
        void Update();
        IReadOnlyList<IComponent> GetAll();
        IEnumerable<T> GetAll<T>();
        T? GetFirst<T>();
        T? Get<T>(ID id);
    }
}