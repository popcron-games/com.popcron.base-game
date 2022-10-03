#nullable enable

using Cysharp.Threading.Tasks;

namespace BaseGame
{
    public interface IManager
    {
        UniTask Initialize();
    }
}
