#nullable enable
using System;
using System.Collections.Generic;

namespace BaseGame
{
    public interface IDependentUpon
    {
        IEnumerable<Type> Dependencies { get; }
    }
}
