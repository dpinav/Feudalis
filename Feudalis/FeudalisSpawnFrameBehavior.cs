using System;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Feudalis
{
    public class FeudalisSpawnFrameBehavior : SpawnFrameBehaviorBase
    {
        public override MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn)
        {
            return GetSpawnFrameFromSpawnPoints(SpawnPoints.ToList(), team, hasMount);
        }
    }
}
