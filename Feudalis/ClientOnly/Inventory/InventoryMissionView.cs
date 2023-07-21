using Feudalis.Inventory;
using Feudalis.Utils;
using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace Feudalis.ClientOnly.Inventory
{
    public class InventoryMissionView : MissionView
    {

        private InventoryVM _dataSource;
        private GauntletLayer _gauntletLayer;
        private SpriteCategory _inventoryCategory;

        private MissionMultiplayerFeudalisClient _client;
        private InventoryMissionRepresentative _representative;

        public Boolean IsActive { get; private set; }

        public NetworkCommunicator PlayerNetwork { get; private set; }
        public MissionPeer Player { get; private set; }
        public bool HasPeerAndMissionComponents { get; private set; } = false;

        public override void OnMissionScreenInitialize()
        {
            base.OnMissionScreenInitialize();

            ViewOrderPriority = 15;
            _gauntletLayer = new GauntletLayer(ViewOrderPriority);

            this.HasPeerAndMissionComponents = false;
            LoadSpritesIfNecessary();

        }

        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();

            MissionScreen.RemoveLayer(_gauntletLayer);
            _inventoryCategory?.Unload();
            _dataSource?.OnFinalize();
            _dataSource = null;
            _gauntletLayer = null;

        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);
            _dataSource?.Tick(dt);

            if (!this.HasPeerAndMissionComponents)
            {
                this.TryGetPeerAndMissionComponents();
            }

            if (this.IsActive)
            {
                bool hasPressedESC = _gauntletLayer.Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.Escape);
                bool hasPressedI = _gauntletLayer.Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.I);

                if (hasPressedI || hasPressedESC)
                {
                    FeudalisChatLib.ChatMessage("Closed Inventory", Colors.Magenta);
                    this.CloseInventory();
                }
            }
            if (!this.IsActive)
            {

                bool hasPressedI = Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.I);

                if (hasPressedI)
                {
                    FeudalisChatLib.ChatMessage("Opened Inventory", Colors.Magenta);
                    this.OpenInventory();
                }
            }
        }

        private void OpenInventory()
        {
            LoadSpritesIfNecessary();

            if (!this.HasPeerAndMissionComponents)
            {
                FeudalisChatLib.ChatMessage("Trying to get peer and components", Colors.Magenta);
                this.TryGetPeerAndMissionComponents();
            }

            if (this._representative == null)
            {
                FeudalisChatLib.ChatMessage("Representative is null", Colors.Magenta);
                return;
            }

            if (this.IsActive || this.Player.ControlledAgent == null)
            {
                FeudalisChatLib.ChatMessage("Controlled Agent is null", Colors.Magenta);
                return;
            }

            this.IsActive = true;
            if (_dataSource == null)
            {
                this._dataSource = new InventoryVM(this, 30);
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

        private void TryGetPeerAndMissionComponents()
        {
            this.PlayerNetwork = GameNetwork.MyPeer;
            this.Player = this.PlayerNetwork.GetComponent<MissionPeer>();
            if (this.Player == null)
                return;

            _representative = InventoryMissionRepresentative.GetInventoryRepresentative(this.PlayerNetwork);
            if (_representative == null)
                return;
            this.HasPeerAndMissionComponents = true;
        }

    }
}
