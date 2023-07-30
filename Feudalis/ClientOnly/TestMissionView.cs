using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.TwoDimension;

namespace Feudalis.ClientOnly
{
    public class TestMissionView : MissionView
    {
        private bool _loaded = false;

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);
            if (!_loaded)
            {
                SpriteData spriteData = UIResourceManager.SpriteData;
                TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
                ResourceDepot uiResourceDepot = UIResourceManager.UIResourceDepot;
                spriteData.SpriteCategories["ui_mpmission"].Load((ITwoDimensionResourceContext)resourceContext, uiResourceDepot);
                _loaded = true;
            }
        }

    }
}
