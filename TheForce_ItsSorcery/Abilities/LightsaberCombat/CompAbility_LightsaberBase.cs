using RimWorld;
using RimWorld.Planet;
using Verse;

namespace TheForce_ItsSorcery.Abilities.LightsaberCombat
{
    internal class CompAbility_LightsaberBase : CompAbilityEffect
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            {
                base.Apply(target, dest);
                if (target != null)
                {
                    AttackTarget((LocalTargetInfo)target);
                }
            }
        }

        public virtual void AttackTarget(LocalTargetInfo target)
        {
            if (target.Pawn != null)
            {
                Log.Message($"Attacking target: {target.Pawn.Name}");
                parent.pawn.meleeVerbs.TryMeleeAttack(target.Pawn, parent.pawn.equipment.PrimaryEq.PrimaryVerb, true);
            }
            else
            {
                Log.Message("Invalid target.");
            }
        }
    }

    public class CompAbilityProperties_LightsaberBase : CompProperties_AbilityEffect
    {
        public CompAbilityProperties_LightsaberBase()
        {
            compClass = typeof(CompAbility_LightsaberBase);
        }
    }
}
