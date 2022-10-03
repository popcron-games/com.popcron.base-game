#nullable enable

using System;

namespace BaseGame
{   
    public static class WarnIfManagersAreMissing
    {
        [OnEnable(true)]    
        private static void DoIt()
        {
            foreach (Type type in TypeCache.GetTypesAssignableFrom<ISingletonManager>())
            {
                //find a prefab that has a component of this type
            }
        }
    }
}
