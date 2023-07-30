using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Objects.Siege;
using MathF = TaleWorlds.Library.MathF;

namespace Feudalis
{
    public class PBGate : UsableMachine, ITargetable
    {
        public const string OuterGateTag = "outer_gate";
        public const string InnerGateTag = "inner_gate";
        private static int _batteringRamHitSoundId = -1;
        public PBGate.DoorOwnership OwningTeam;
        public string OpeningAnimationName = "castle_gate_a_opening";
        public string ClosingAnimationName = "castle_gate_a_closing";
        public string HitAnimationName = "castle_gate_a_hit";
        public string PlankHitAnimationName = "castle_gate_a_plank_hit";
        public string HitMeleeAnimationName = "castle_gate_a_hit_melee";
        public string DestroyAnimationName = "castle_gate_a_break";
        public string LeftDoorBoneName = "bn_bottom_l";
        public string RightDoorBoneName = "bn_bottom_r";
        public string ExtraCollisionObjectTagRight = "extra_collider_r";
        public string ExtraCollisionObjectTagLeft = "extra_collider_l";
        private int _openingAnimationIndex = -1;
        private int _closingAnimationIndex = -1;
        private bool _leftExtraColliderDisabled;
        private bool _rightExtraColliderDisabled;
        private bool _civilianMission;
        public bool ActivateExtraColliders = true;
        public string SideTag;
        private SynchedMissionObject _door;
        private Skeleton _doorSkeleton;
        private GameEntity _extraColliderRight;
        private GameEntity _extraColliderLeft;
        private readonly List<GameEntity> _attackOnlyDoorColliders;
        private float _previousAnimationProgress = -1f;
        private GameEntity _agentColliderRight;
        private GameEntity _agentColliderLeft;
        private LadderQueueManager _queueManager;
        private bool _afterMissionStartTriggered;
        private sbyte _rightDoorBoneIndex;
        private sbyte _leftDoorBoneIndex;
        private AgentPathNavMeshChecker _pathChecker;
        public bool AutoOpen;
        private SynchedMissionObject _plank;
        private WorldFrame _middleFrame;
        private WorldFrame _defenseWaitFrame;
        private Action DestructibleComponentOnMissionReset;

        public TacticalPosition MiddlePosition { get; private set; }

        private static int BatteringRamHitSoundIdCache
        {
            get
            {
                if (PBGate._batteringRamHitSoundId == -1)
                    PBGate._batteringRamHitSoundId = SoundEvent.GetEventIdFromString("event:/mission/siege/door/hit");
                return PBGate._batteringRamHitSoundId;
            }
        }

        public TacticalPosition WaitPosition { get; private set; }

        public override FocusableObjectType FocusableObjectType => FocusableObjectType.Gate;

        public PBGate.GateState State { get; private set; }

        public bool IsGateOpen => this.State == PBGate.GateState.Open || this.IsDestroyed;

        public IPrimarySiegeWeapon AttackerSiegeWeapon { get; set; }

        public PBGate() => this._attackOnlyDoorColliders = new List<GameEntity>();

        public Vec3 GetPosition() => this.GameEntity.GlobalPosition;

        public WorldFrame MiddleFrame => this._middleFrame;

        public WorldFrame DefenseWaitFrame => this._defenseWaitFrame;

        protected override void OnInit()
        {
            base.OnInit();
            DestructableComponent destructableComponent = this.GameEntity.GetScriptComponents<DestructableComponent>().FirstOrDefault<DestructableComponent>();
            if (destructableComponent != null)
            {
                destructableComponent.OnNextDestructionState += new Action(this.OnNextDestructionState);
                this.DestructibleComponentOnMissionReset = new Action((destructableComponent).Reset);
                if (!GameNetwork.IsClientOrReplay)
                {
                    destructableComponent.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnDestroyed);
                    destructableComponent.OnHitTaken += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnHitTaken);
                    destructableComponent.OnCalculateDestructionStateIndex += new Func<int, int, int, int>(this.OnCalculateDestructionStateIndex);
                }
                destructableComponent.BattleSide = BattleSideEnum.Defender;
            }
            this.CollectGameEntities(true);
            this.GameEntity.SetAnimationSoundActivation(true);
            if (GameNetwork.IsClientOrReplay)
                return;
            this._queueManager = this.GameEntity.GetScriptComponents<LadderQueueManager>().FirstOrDefault<LadderQueueManager>();
            if (this._queueManager == null)
            {
                GameEntity gameEntity = this.GameEntity.GetChildren().FirstOrDefault<GameEntity>((Func<GameEntity, bool>)(ce => ce.GetScriptComponents<LadderQueueManager>().Any<LadderQueueManager>()));
                if ((NativeObject)gameEntity != (NativeObject)null)
                    this._queueManager = gameEntity.GetFirstScriptOfType<LadderQueueManager>();
            }
            if (this._queueManager != null)
            {
                MatrixFrame identity = MatrixFrame.Identity;
                identity.origin.y -= 2f;
                identity.rotation.RotateAboutSide(-1.57079637f);
                identity.rotation.RotateAboutForward(3.14159274f);
                this._queueManager.Initialize(this._queueManager.ManagedNavigationFaceId, identity, -identity.rotation.u, BattleSideEnum.Defender, 15, 0.628318548f, 3f, 2.2f, 0.0f, 0.0f, false, 1f, (float)int.MaxValue, 5f, false, -2, -2, int.MaxValue, 15);
                this._queueManager.IsDeactivated = false;
            }

            List<GameEntity> source1 = this.GameEntity.CollectChildrenEntitiesWithTag("middle_pos");
            if (source1.Count > 0)
            {
                GameEntity gameEntity = source1.FirstOrDefault<GameEntity>();
                this.MiddlePosition = gameEntity.GetFirstScriptOfType<TacticalPosition>();
                MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
                this._middleFrame = new WorldFrame(globalFrame.rotation, globalFrame.origin.ToWorldPosition());
                this._middleFrame.Origin.GetGroundVec3();
            }
            else
            {
                MatrixFrame globalFrame = this.GameEntity.GetGlobalFrame();
                this._middleFrame = new WorldFrame(globalFrame.rotation, globalFrame.origin.ToWorldPosition());
            }
            List<GameEntity> source2 = this.GameEntity.CollectChildrenEntitiesWithTag("wait_pos");
            if (source2.Count > 0)
            {
                GameEntity gameEntity = source2.FirstOrDefault<GameEntity>();
                this.WaitPosition = gameEntity.GetFirstScriptOfType<TacticalPosition>();
                MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
                this._defenseWaitFrame = new WorldFrame(globalFrame.rotation, globalFrame.origin.ToWorldPosition());
                this._defenseWaitFrame.Origin.GetGroundVec3();
            }
            else
                this._defenseWaitFrame = this._middleFrame;
            this._openingAnimationIndex = MBAnimation.GetAnimationIndexWithName(this.OpeningAnimationName);
            this._closingAnimationIndex = MBAnimation.GetAnimationIndexWithName(this.ClosingAnimationName);
            this.SetScriptComponentToTick(this.GetTickRequirement());
            this.OnCheckForProblems();
        }

        public void SetUsableTeam(Team team)
        {
            foreach (StandingPoint standingPoint in (List<StandingPoint>)this.StandingPoints)
            {
                if (standingPoint is StandingPointWithTeamLimit pointWithTeamLimit)
                    pointWithTeamLimit.UsableTeam = team;
            }
        }

        public override void AfterMissionStart()
        {
            this._afterMissionStartTriggered = true;
            base.AfterMissionStart();
            this.SetInitialStateOfGate();
            this.InitializeExtraColliderPositions();
            if (this.OwningTeam == PBGate.DoorOwnership.Attackers)
                this.SetUsableTeam(Mission.Current.AttackerTeam);
            else if (this.OwningTeam == PBGate.DoorOwnership.Defenders)
                this.SetUsableTeam(Mission.Current.DefenderTeam);
        }

        protected override void OnRemoved(int removeReason)
        {
            base.OnRemoved(removeReason);
            DestructableComponent destructableComponent = this.GameEntity.GetScriptComponents<DestructableComponent>().FirstOrDefault<DestructableComponent>();
            if (destructableComponent == null)
                return;
            destructableComponent.OnNextDestructionState -= new Action(this.OnNextDestructionState);
            if (GameNetwork.IsClientOrReplay)
                return;
            destructableComponent.OnDestroyed -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnDestroyed);
            destructableComponent.OnHitTaken -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnHitTaken);
            destructableComponent.OnCalculateDestructionStateIndex -= new Func<int, int, int, int>(this.OnCalculateDestructionStateIndex);
        }

        protected override void OnEditorInit()
        {
            base.OnEditorInit();
            if (!this.GameEntity.HasTag("outer_gate") || !this.GameEntity.HasTag("inner_gate"))
                return;
            MBDebug.ShowWarning("Castle gate has both the outer gate tag and the inner gate tag.");
        }

        protected override void OnMissionReset()
        {
            Action componentOnMissionReset = this.DestructibleComponentOnMissionReset;
            if (componentOnMissionReset != null)
                componentOnMissionReset();
            this.CollectGameEntities(false);
            base.OnMissionReset();
            this.SetInitialStateOfGate();
            this._previousAnimationProgress = -1f;
        }

        // This should probably pick the last state from the backend
        private void SetInitialStateOfGate()
        {
            this._doorSkeleton.SetAnimationAtChannel(this._closingAnimationIndex, 0);
            this._doorSkeleton.SetAnimationParameterAtChannel(0, 0.99f);
            this._doorSkeleton.Freeze(false);
            this.State = PBGate.GateState.Closed;
        }

        public override string GetDescriptionText(GameEntity gameEntity = null) => new TextObject("{=6wZUG0ev}Gate").ToString();

        public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
        {
            TextObject forStandingPoint = new TextObject(usableGameObject.GameEntity.HasTag("open") ? "{=5oozsaIb}{KEY} Open" : "{=TJj71hPO}{KEY} Close");
            forStandingPoint.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
            return forStandingPoint;
        }

        public void OpenDoor()
        {
            if (this.IsDisabled)
                return;
            this.State = PBGate.GateState.Open;
            int animationIndexAtChannel = this._doorSkeleton.GetAnimationIndexAtChannel(0);
            float parameterAtChannel = this._doorSkeleton.GetAnimationParameterAtChannel(0);
            this._door.SetAnimationAtChannelSynched(this._openingAnimationIndex, 0);
            int closingAnimationIndex = this._closingAnimationIndex;
            if (animationIndexAtChannel == closingAnimationIndex)
                this._door.SetAnimationChannelParameterSynched(0, 1f - parameterAtChannel);
            this._plank?.SetVisibleSynched(false);
        }

        public void CloseDoor()
        {
            if (this.IsDisabled)
                return;
            this.State = PBGate.GateState.Closed;
            int animationIndexAtChannel = this._doorSkeleton.GetAnimationIndexAtChannel(0);
            float parameterAtChannel = this._doorSkeleton.GetAnimationParameterAtChannel(0);
            this._door.SetAnimationAtChannelSynched(this._closingAnimationIndex, 0);
            int openingAnimationIndex = this._openingAnimationIndex;
            if (animationIndexAtChannel != openingAnimationIndex)
                return;
            this._door.SetAnimationChannelParameterSynched(0, 1f - parameterAtChannel);
        }

        private void UpdateDoorBodies(bool updateAnyway)
        {
            if (this._attackOnlyDoorColliders.Count == 2)
            {
                float parameterAtChannel = this._doorSkeleton.GetAnimationParameterAtChannel(0);
                if (!((double)this._previousAnimationProgress != (double)parameterAtChannel | updateAnyway))
                    return;
                this._previousAnimationProgress = parameterAtChannel;
                MatrixFrame frame1 = this._doorSkeleton.GetBoneEntitialFrameWithIndex(this._leftDoorBoneIndex);
                MatrixFrame frame2 = this._doorSkeleton.GetBoneEntitialFrameWithIndex(this._rightDoorBoneIndex);
                this._attackOnlyDoorColliders[0].SetFrame(ref frame2);
                this._attackOnlyDoorColliders[1].SetFrame(ref frame1);
                this._agentColliderLeft?.SetFrame(ref frame1);
                this._agentColliderRight?.SetFrame(ref frame2);
                if (!((NativeObject)this._extraColliderLeft != (NativeObject)null) || !((NativeObject)this._extraColliderRight != (NativeObject)null))
                    return;
                if (this.State == PBGate.GateState.Closed)
                {
                    if (!this._leftExtraColliderDisabled)
                    {
                        this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag | BodyFlags.Disabled);
                        this._leftExtraColliderDisabled = true;
                    }
                    if (this._rightExtraColliderDisabled)
                        return;
                    this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag | BodyFlags.Disabled);
                    this._rightExtraColliderDisabled = true;
                }
                else
                {
                    float num1 = (frame2.origin - frame1.origin).Length * 0.5f;
                    float input = Vec3.DotProduct(frame2.rotation.s, Vec3.Side) / (frame2.rotation.s.Length * 1f);
                    float num2 = MathF.Sqrt((float)(1.0 - (double)input * (double)input));
                    float num3 = num1 * 1.1f;
                    float num4 = MBMath.Map(input, 0.3f, 1f, 0.0f, 1f) * (num1 * 0.2f);
                    this._extraColliderLeft.SetLocalPosition(frame1.origin - new Vec3(num3 - num1 + num4, num1 * num2));
                    this._extraColliderRight.SetLocalPosition(frame2.origin - new Vec3((float)-((double)num3 - (double)num1) - num4, num1 * num2));
                    float num5 = (double)input >= 0.0 ? num1 - num1 * input : num1 + num1 * -input;
                    float num6 = (num3 - num5) / num1;
                    if ((double)num6 <= 9.9999997473787516E-05)
                    {
                        if (!this._leftExtraColliderDisabled)
                        {
                            this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag | BodyFlags.Disabled);
                            this._leftExtraColliderDisabled = true;
                        }
                    }
                    else
                    {
                        if (this._leftExtraColliderDisabled)
                        {
                            this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag & ~BodyFlags.Disabled);
                            this._leftExtraColliderDisabled = false;
                        }
                        frame1 = this._extraColliderLeft.GetFrame();
                        frame1.rotation.Orthonormalize();
                        frame1.origin -= new Vec3(num3 - num3 * num6);
                        this._extraColliderLeft.SetFrame(ref frame1);
                    }
                    frame2 = this._extraColliderRight.GetFrame();
                    frame2.rotation.Orthonormalize();
                    float num7 = (double)input >= 0.0 ? num1 - num1 * input : num1 + num1 * -input;
                    float num8 = (num3 - num7) / num1;
                    if ((double)num8 <= 9.9999997473787516E-05)
                    {
                        if (this._rightExtraColliderDisabled)
                            return;
                        this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag | BodyFlags.Disabled);
                        this._rightExtraColliderDisabled = true;
                    }
                    else
                    {
                        if (this._rightExtraColliderDisabled)
                        {
                            this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag & ~BodyFlags.Disabled);
                            this._rightExtraColliderDisabled = false;
                        }
                        frame2.origin += new Vec3(num3 - num3 * num8);
                        this._extraColliderRight.SetFrame(ref frame2);
                    }
                }
            }
            else
            {
                if (this._attackOnlyDoorColliders.Count != 1)
                    return;
                MatrixFrame entitialFrameWithName = this._doorSkeleton.GetBoneEntitialFrameWithName(this.RightDoorBoneName);
                this._attackOnlyDoorColliders[0].SetFrame(ref entitialFrameWithName);
                this._agentColliderRight?.SetFrame(ref entitialFrameWithName);
            }
        }

        public override ScriptComponentBehavior.TickRequirement GetTickRequirement() => this.GameEntity.IsVisibleIncludeParents() ? ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement() : base.GetTickRequirement();

        protected override void OnTick(float dt)
        {
            base.OnTick(dt);
            if (!this.GameEntity.IsVisibleIncludeParents())
                return;
            if (this._afterMissionStartTriggered)
                this.UpdateDoorBodies(false);
            if (!GameNetwork.IsClientOrReplay)
                this.ServerTick(dt);
            if (!this.Ai.HasActionCompleted)
                return;
            bool flag1 = false;
            for (int index = 0; index < this.StandingPoints.Count; ++index)
            {
                if (this.StandingPoints[index].HasUser || this.StandingPoints[index].HasAIMovingTo)
                {
                    flag1 = true;
                    break;
                }
            }
            if (flag1)
                return;
            bool flag2 = false;
            for (int index = 0; index < this._userFormations.Count; ++index)
            {
                if (this._userFormations[index].CountOfDetachableNonplayerUnits > 0)
                {
                    flag2 = true;
                    break;
                }
            }
            if (flag2)
                return;
        }

        protected override bool IsAgentOnInconvenientNavmesh(Agent agent, StandingPoint standingPoint) => false;

        private void ServerTick(float dt)
        {
            if (this.IsDeactivated)
                return;
            foreach (StandingPoint standingPoint in (List<StandingPoint>)this.StandingPoints)
            {
                if (standingPoint.HasUser)
                {
                    if (standingPoint.GameEntity.HasTag("open"))
                    {
                        this.OpenDoor();
                    }
                    else
                    {
                        this.CloseDoor();
                    }
                }
            }

            if (!((NativeObject)this._doorSkeleton != (NativeObject)null) || this.IsDestroyed)
                return;
            float parameterAtChannel = this._doorSkeleton.GetAnimationParameterAtChannel(0);
            foreach (StandingPoint standingPoint in (List<StandingPoint>)this.StandingPoints)
            {
                bool flag = (double)parameterAtChannel < 1.0 || standingPoint.GameEntity.HasTag(this.State == PBGate.GateState.Open ? "open" : "close");
                standingPoint.SetIsDeactivatedSynched(flag);
            }
            if ((double)parameterAtChannel >= 1.0 && this.State == PBGate.GateState.Open)
            {
                if ((NativeObject)this._extraColliderRight != (NativeObject)null)
                {
                    this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag | BodyFlags.Disabled);
                    this._rightExtraColliderDisabled = true;
                }
                if ((NativeObject)this._extraColliderLeft != (NativeObject)null)
                {
                    this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag | BodyFlags.Disabled);
                    this._leftExtraColliderDisabled = true;
                }
            }
            if (this._plank == null || this.State != PBGate.GateState.Closed || (double)parameterAtChannel <= 0.89999997615814209)
                return;
            this._plank.SetVisibleSynched(true);
        }

        public TargetFlags GetTargetFlags()
        {
            TargetFlags targetFlags = (TargetFlags)(0 | 4);
            if (this.IsDestroyed)
                targetFlags |= TargetFlags.NotAThreat;
            if (DebugSiegeBehavior.DebugAttackState == DebugSiegeBehavior.DebugStateAttacker.DebugAttackersToBattlements)
                targetFlags |= TargetFlags.DebugThreat;
            return targetFlags;
        }

        public float GetTargetValue(List<Vec3> weaponPos) => 10f;

        public GameEntity GetTargetEntity() => this.GameEntity;

        public BattleSideEnum GetSide() => BattleSideEnum.Defender;

        public GameEntity Entity() => this.GameEntity;

        protected void CollectGameEntities(bool calledFromOnInit)
        {
            this.CollectDynamicGameEntities(calledFromOnInit);
            if (GameNetwork.IsClientOrReplay)
                return;
            List<GameEntity> source = this.GameEntity.CollectChildrenEntitiesWithTag("plank");
            if (source.Count <= 0)
                return;
            this._plank = source.FirstOrDefault<GameEntity>().GetFirstScriptOfType<SynchedMissionObject>();
        }

        protected void OnNextDestructionState()
        {
            this.CollectDynamicGameEntities(false);
            this.UpdateDoorBodies(true);
        }

        protected void CollectDynamicGameEntities(bool calledFromOnInit)
        {
            this._attackOnlyDoorColliders.Clear();
            List<GameEntity> list;
            if (calledFromOnInit)
            {
                list = this.GameEntity.CollectChildrenEntitiesWithTag("gate").ToList<GameEntity>();
                this._leftExtraColliderDisabled = false;
                this._rightExtraColliderDisabled = false;
                this._agentColliderLeft = this.GameEntity.GetFirstChildEntityWithTag("collider_agent_l");
                this._agentColliderRight = this.GameEntity.GetFirstChildEntityWithTag("collider_agent_r");
            }
            else
                list = this.GameEntity.CollectChildrenEntitiesWithTag("gate").Where<GameEntity>((Func<GameEntity, bool>)(x => x.IsVisibleIncludeParents())).ToList<GameEntity>();
            if (list.Count == 0)
                return;
            if (list.Count > 1)
            {
                int num1 = int.MinValue;
                int num2 = int.MaxValue;
                GameEntity gameEntity1 = (GameEntity)null;
                GameEntity gameEntity2 = (GameEntity)null;
                foreach (GameEntity gameEntity3 in list)
                {
                    int num3 = int.Parse(((IEnumerable<string>)((IEnumerable<string>)gameEntity3.Tags).FirstOrDefault<string>((Func<string, bool>)(x => x.Contains("state_"))).Split('_')).Last<string>());
                    if (num3 > num1)
                    {
                        num1 = num3;
                        gameEntity1 = gameEntity3;
                    }
                    if (num3 < num2)
                    {
                        num2 = num3;
                        gameEntity2 = gameEntity3;
                    }
                }
                this._door = calledFromOnInit ? gameEntity2.GetFirstScriptOfType<SynchedMissionObject>() : gameEntity1.GetFirstScriptOfType<SynchedMissionObject>();
            }
            else
                this._door = list[0].GetFirstScriptOfType<SynchedMissionObject>();
            this._doorSkeleton = this._door.GameEntity.Skeleton;
            GameEntity gameEntity4 = this._door.GameEntity.CollectChildrenEntitiesWithTag("collider_r").FirstOrDefault<GameEntity>();
            if ((NativeObject)gameEntity4 != (NativeObject)null)
                this._attackOnlyDoorColliders.Add(gameEntity4);
            GameEntity gameEntity5 = this._door.GameEntity.CollectChildrenEntitiesWithTag("collider_l").FirstOrDefault<GameEntity>();
            if ((NativeObject)gameEntity5 != (NativeObject)null)
                this._attackOnlyDoorColliders.Add(gameEntity5);
            if ((NativeObject)gameEntity4 == (NativeObject)null || (NativeObject)gameEntity5 == (NativeObject)null)
            {
                this._agentColliderLeft?.SetVisibilityExcludeParents(false);
                this._agentColliderRight?.SetVisibilityExcludeParents(false);
            }
            GameEntity gameEntity6 = this._door.GameEntity.CollectChildrenEntitiesWithTag(this.ExtraCollisionObjectTagLeft).FirstOrDefault<GameEntity>();
            if ((NativeObject)gameEntity6 != (NativeObject)null)
            {
                if (!this.ActivateExtraColliders)
                {
                    gameEntity6.RemovePhysics();
                }
                else
                {
                    if (!calledFromOnInit)
                    {
                        MatrixFrame frame = (NativeObject)this._extraColliderLeft != (NativeObject)null ? this._extraColliderLeft.GetFrame() : this._doorSkeleton.GetBoneEntitialFrameWithName(this.LeftDoorBoneName);
                        this._extraColliderLeft = gameEntity6;
                        this._extraColliderLeft.SetFrame(ref frame);
                    }
                    else
                        this._extraColliderLeft = gameEntity6;
                    if (this._leftExtraColliderDisabled)
                        this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag | BodyFlags.Disabled);
                    else
                        this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag & ~BodyFlags.Disabled);
                }
            }
            GameEntity gameEntity7 = this._door.GameEntity.CollectChildrenEntitiesWithTag(this.ExtraCollisionObjectTagRight).FirstOrDefault<GameEntity>();
            if ((NativeObject)gameEntity7 != (NativeObject)null)
            {
                if (!this.ActivateExtraColliders)
                {
                    gameEntity7.RemovePhysics();
                }
                else
                {
                    if (!calledFromOnInit)
                    {
                        MatrixFrame frame = (NativeObject)this._extraColliderRight != (NativeObject)null ? this._extraColliderRight.GetFrame() : this._doorSkeleton.GetBoneEntitialFrameWithName(this.RightDoorBoneName);
                        this._extraColliderRight = gameEntity7;
                        this._extraColliderRight.SetFrame(ref frame);
                    }
                    else
                        this._extraColliderRight = gameEntity7;
                    if (this._rightExtraColliderDisabled)
                        this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag | BodyFlags.Disabled);
                    else
                        this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag & ~BodyFlags.Disabled);
                }
            }
            if (this._door == null || !((NativeObject)this._doorSkeleton != (NativeObject)null))
                return;
            this._leftDoorBoneIndex = Skeleton.GetBoneIndexFromName(this._doorSkeleton.GetName(), this.LeftDoorBoneName);
            this._rightDoorBoneIndex = Skeleton.GetBoneIndexFromName(this._doorSkeleton.GetName(), this.RightDoorBoneName);
        }

        private void InitializeExtraColliderPositions()
        {
            if ((NativeObject)this._extraColliderLeft != (NativeObject)null)
            {
                MatrixFrame entitialFrameWithName = this._doorSkeleton.GetBoneEntitialFrameWithName(this.LeftDoorBoneName);
                this._extraColliderLeft.SetFrame(ref entitialFrameWithName);
                this._extraColliderLeft.SetVisibilityExcludeParents(true);
            }
            if ((NativeObject)this._extraColliderRight != (NativeObject)null)
            {
                MatrixFrame entitialFrameWithName = this._doorSkeleton.GetBoneEntitialFrameWithName(this.RightDoorBoneName);
                this._extraColliderRight.SetFrame(ref entitialFrameWithName);
                this._extraColliderRight.SetVisibilityExcludeParents(true);
            }
            this.UpdateDoorBodies(true);
            foreach (GameEntity onlyDoorCollider in this._attackOnlyDoorColliders)
                onlyDoorCollider.SetVisibilityExcludeParents(true);
            if ((NativeObject)this._agentColliderLeft != (NativeObject)null)
                this._agentColliderLeft.SetVisibilityExcludeParents(true);
            if (!((NativeObject)this._agentColliderRight != (NativeObject)null))
                return;
            this._agentColliderRight.SetVisibilityExcludeParents(true);
        }

        private void OnHitTaken(
          DestructableComponent hitComponent,
          Agent hitterAgent,
          in MissionWeapon weapon,
          ScriptComponentBehavior attackerScriptComponentBehavior,
          int inflictedDamage)
        {
            if (GameNetwork.IsClientOrReplay || inflictedDamage < 200 || this.State != PBGate.GateState.Closed || !(attackerScriptComponentBehavior is BatteringRam))
                return;
            this._plank?.SetAnimationAtChannelSynched(this.PlankHitAnimationName, 0);
            this._door.SetAnimationAtChannelSynched(this.HitAnimationName, 0);
            Mission.Current.MakeSound(PBGate.BatteringRamHitSoundIdCache, this.GameEntity.GlobalPosition, false, true, -1, -1);
        }

        private void OnDestroyed(
          DestructableComponent destroyedComponent,
          Agent destroyerAgent,
          in MissionWeapon weapon,
          ScriptComponentBehavior attackerScriptComponentBehavior,
          int inflictedDamage)
        {
            if (GameNetwork.IsClientOrReplay)
                return;
            this._plank?.SetVisibleSynched(false);
            foreach (UsableMissionObject standingPoint in (List<StandingPoint>)this.StandingPoints)
                standingPoint.SetIsDeactivatedSynched(true);
            if (attackerScriptComponentBehavior is BatteringRam)
                this._door.SetAnimationAtChannelSynched(this.DestroyAnimationName, 0);
        }

        private int OnCalculateDestructionStateIndex(
          int destructionStateIndex,
          int inflictedDamage,
          int destructionStateCount)
        {
            return inflictedDamage < 200 ? destructionStateIndex : MathF.Min(destructionStateIndex, destructionStateCount - 1);
        }

        public Vec3 GetTargetingOffset() => Vec3.Zero;

        public enum DoorOwnership
        {
            Defenders,
            Attackers,
        }

        public enum GateState
        {
            Open,
            Closed,
        }
    }
}
