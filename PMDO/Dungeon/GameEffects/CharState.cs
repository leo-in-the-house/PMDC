﻿using System;
using RogueEssence;
using RogueEssence.Dungeon;

namespace PMDO.Dungeon
{
    [Serializable]
    public class StickyHoldState : CharState
    {
        public StickyHoldState() { }
        public override GameplayState Clone() { return new StickyHoldState(); }
    }

    [Serializable]
    public class AnchorState : CharState
    {
        public AnchorState() { }
        public override GameplayState Clone() { return new AnchorState(); }
    }
    [Serializable]
    public class HitAndRunState : CharState
    {
        public HitAndRunState() { }
        public override GameplayState Clone() { return new HitAndRunState(); }
    }

    [Serializable]
    public class SleepWalkerState : CharState
    {
        public SleepWalkerState() { }
        public override GameplayState Clone() { return new SleepWalkerState(); }
    }

    [Serializable]
    public class ChargeWalkerState : CharState
    {
        public ChargeWalkerState() { }
        public override GameplayState Clone() { return new ChargeWalkerState(); }
    }

    [Serializable]
    public class DrainDamageState : CharState
    {
        public DrainDamageState() { }
        public override GameplayState Clone() { return new DrainDamageState(); }
    }

    [Serializable]
    public class NoRecoilState : CharState
    {
        public NoRecoilState() { }
        public override GameplayState Clone() { return new NoRecoilState(); }
    }

    [Serializable]
    public class HeatproofState : CharState
    {
        public HeatproofState() { }
        public override GameplayState Clone() { return new HeatproofState(); }
    }

    [Serializable]
    public class LavaState : CharState
    {
        public LavaState() { }
        public override GameplayState Clone() { return new LavaState(); }
    }

    [Serializable]
    public class MagicGuardState : CharState
    {
        public MagicGuardState() { }
        public override GameplayState Clone() { return new MagicGuardState(); }
    }

    [Serializable]
    public class SandState : CharState
    {
        public SandState() { }
        public override GameplayState Clone() { return new SandState(); }
    }

    [Serializable]
    public class HailState : CharState
    {
        public HailState() { }
        public override GameplayState Clone() { return new HailState(); }
    }

    [Serializable]
    public class SnipeState : CharState
    {
        public SnipeState() { }
        public override GameplayState Clone() { return new SnipeState(); }
    }

    [Serializable]
    public class PoisonHealState : CharState
    {
        public PoisonHealState() { }
        public override GameplayState Clone() { return new PoisonHealState(); }
    }

    [Serializable]
    public class HeavyWeightState : CharState
    {
        public HeavyWeightState() { }
        public override GameplayState Clone() { return new HeavyWeightState(); }
    }

    [Serializable]
    public class LightWeightState : CharState
    {
        public LightWeightState() { }
        public override GameplayState Clone() { return new LightWeightState(); }
    }

    [Serializable]
    public class TrapState : CharState
    {
        public TrapState() { }
        public override GameplayState Clone() { return new TrapState(); }
    }

    [Serializable]
    public class GripState : CharState
    {
        public GripState() { }
        public override GameplayState Clone() { return new GripState(); }
    }

    [Serializable]
    public class BindState : CharState
    {
        public BindState() { }
        public override GameplayState Clone() { return new BindState(); }
    }

    [Serializable]
    public class GemBoostState : CharState
    {
        public GemBoostState() { }
        public override GameplayState Clone() { return new GemBoostState(); }
    }
}
