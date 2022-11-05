using System;
using TaleWorlds.TwoDimension;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace Feudalis.ClientOnly.Inventory
{
    public class InventoryMissionView : MissionView
    {
        public Boolean IsActive { get; private set; }

        private InventoryVM _dataSource;
        private GauntletLayer _gauntletLayer;
        private SpriteCategory _inventoryCategory;

        private MissionMultiplayerFeudalisClient _client;

        private MissionLobbyComponent _lobbyComponent;

        public override void OnMissionScreenInitialize()
        {
            base.OnMissionScreenInitialize();

            ViewOrderPriority = 15;

            _gauntletLayer = new GauntletLayer(ViewOrderPriority);

            var spriteData = UIResourceManager.SpriteData;
            var resourceContext = UIResourceManager.ResourceContext;
            var resourceDepot = UIResourceManager.UIResourceDepot;
            _inventoryCategory = spriteData.SpriteCategories["ui_mpmission"];
            _inventoryCategory.Load(resourceContext, resourceDepot);

            _lobbyComponent = Mission.GetMissionBehavior<MissionLobbyComponent>();
            _lobbyComponent.OnPostMatchEnded += OnPostMatchEnded;

            _dataSource.IsEnabled = true;
        }

        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();

            MissionScreen.RemoveLayer(_gauntletLayer);
            _inventoryCategory?.Unload();
            _dataSource.OnFinalize();
            _dataSource = null;
            _gauntletLayer = null;

            _lobbyComponent.OnPostMatchEnded -= OnPostMatchEnded;
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);
            _dataSource.Tick(dt);

            if (this.IsActive)
            {
                
                if (HasPressedU() || HasPressedEsc())
                    this.CloseInventory();
            }
            else
            {
                if (HasPressedU())
                    this.OpenInventory();
            }
        }

        private void OnPostMatchEnded()
        {
            _dataSource.IsEnabled = false;
        }

        private void OpenInventory()
        {
            if (this.IsActive)
            {
                return;
            }
            this.IsActive = true;
            if (_dataSource == null)
            {
                this._client = Mission.GetMissionBehavior<MissionMultiplayerFeudalisClient>();
                this._dataSource = new InventoryVM(this._client, 30);
            }
            this._dataSource.RefreshValues();
            if (this._gauntletLayer != null)
            {
                this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority);
            }
            this._gauntletLayer.IsFocusLayer = true;
            this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.Mouse);
            //this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
            this._gauntletLayer.LoadMovie("FeudalisInventoryWindow", this._dataSource);
            this.MissionScreen.AddLayer(this._gauntletLayer);
            ScreenManager.TrySetFocus(this._gauntletLayer);
        }

        private void CloseInventory()
        {
            if (this.IsActive)
            {
                this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
                this.MissionScreen.RemoveLayer(this._gauntletLayer);
                this.IsActive = false;
            }
        }

        private Boolean HasPressedU()
        {
            return Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.U);
        }

        private Boolean HasPressedEsc()
        {
            return 
                this._gauntletLayer != null 
                && _gauntletLayer.Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.Escape);
        }

    }
}
