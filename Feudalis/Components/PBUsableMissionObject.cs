using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Feudalis.Components
{
    public abstract class PBUsableMissionObject : UsableMissionObject
    {

        public int MaxUseDistance = 50;

        public override bool IsDisabledForAgent(Agent agent)
        {
            Vec3 objectLocation = GameEntity.GlobalPosition;
            Vec3 agentLocation = agent?.Position ?? Vec3.Invalid;

            float distance = objectLocation.DistanceSquared(agentLocation);

            return distance <= this.MaxUseDistance && base.IsDisabledForAgent(agent);
        }

    }
}
