#nullable enable
using System;
using UnityEngine;

namespace BaseGame
{
    public class PlayerLoopSettings : ScriptableSingleton<PlayerLoopSettings>
    {
        [SerializeField, TypeFilter(assignableFrom: typeof(Simulation))]
        private SerializedType simulationType;

        public Type SimulationType => simulationType;
    }
}