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
            this._client = Mission.GetMissionBehavior<MissionMultiplayerFeudalisClient>();

            LoadSpritesIfNecessary();

            _lobbyComponent = Mission.GetMissionBehavior<MissionLobbyComponent>();
            _lobbyComponent.OnPostMatchEnded += OnPostMatchEnded;

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
            _dataSource?.Tick(dt);

            if (this.IsActive)
            {
                bool hasPressedESC = _gauntletLayer.Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.Escape);
                bool hasPressedU = _gauntletLayer.Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.U);

                if (hasPressedU || hasPressedESC)
                    this.CloseInventory();
            }
            else
            {
                bool hasPressedU = Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.U);

                if (hasPressedU)
                    this.OpenInventory();
            }
        }

        private void OnPostMatchEnded()
        {
            _dataSource.IsEnabled = false;
        }

        private void OpenInventory()
        {
            if (this.IsActive || this._client?.MyRepresentative?.ControlledAgent == null)
            {
                return;
            }
            this.IsActive = true;
            if (_dataSource == null)
            {
                this._dataSource = new InventoryVM(this._client, 28);
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

        private void LoadSpritesIfNecessary()
        {
            if (_inventoryCategory == null || !_inventoryCategory.IsLoaded)
            {
                SpriteData spriteData = UIResourceManager.SpriteData;
                TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
                ResourceDepot uiResourceDepot = UIResourceManager.UIResourceDepot;
                _inventoryCategory = spriteData.SpriteCategories["ui_inventory"];
                _inventoryCategory.Load((ITwoDimensionResourceContext)resourceContext, uiResourceDepot);
                spriteData.SpriteCategories["ui_mpmission"].Load((ITwoDimensionResourceContext)resourceContext, uiResourceDepot);
            }
        }

    }
}
