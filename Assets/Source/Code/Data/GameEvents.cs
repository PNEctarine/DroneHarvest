using System;

namespace Source.Code.Data
{
    public class GameEvents
    {
        public Action OnDronesSpeedChanged;
        public Action OnResourcesSpawnTimeChanged;

        public void ClearAllEvents()
        {
            OnDronesSpeedChanged = null;
            OnResourcesSpawnTimeChanged = null;
        }
    }
}
