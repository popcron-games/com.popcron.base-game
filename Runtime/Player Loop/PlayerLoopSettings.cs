#nullable enable
using UnityEngine;

namespace BaseGame
{
    public class PlayerLoopSettings : ScriptableSingleton<PlayerLoopSettings>
    {
        [SerializeField]
        private Simulation? simulation = null;

        public Simulation SimulationPrefab => simulation ?? throw ExceptionBuilder.Format("Simulation prefab is unassigned on {0}", this);
    }
}