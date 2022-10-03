#nullable enable

namespace BaseGame
{
    /// <summary>
    /// Only gets called in editor.
    /// </summary>
    public readonly struct PlayModeStateChanged : IEvent
    {
        public readonly bool isPlaying;

        public PlayModeStateChanged(bool isPlaying)
        {
            this.isPlaying = isPlaying;
        }
    }
}
