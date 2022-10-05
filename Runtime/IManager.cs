#nullable enable

using Cysharp.Threading.Tasks;
using System.Threading;

namespace BaseGame
{
    public interface IManager
    {
        UniTask Initialize(CancellationToken cancelletatonToken);
    }
}
