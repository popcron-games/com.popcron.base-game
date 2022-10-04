#nullable enable
using System.Collections.Generic;

namespace BaseGame
{
    public interface IVariables
    {
        IEnumerable<(string, BaseVariable)> GetVariables();
    }
}