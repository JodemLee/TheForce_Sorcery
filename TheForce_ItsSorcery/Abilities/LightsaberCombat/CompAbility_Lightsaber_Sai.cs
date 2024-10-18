using RimWorld;
using TheForce_ItsSorcery.Abilities;
using Verse;

namespace TheForce_ItsSorcery.Abilities.LightsaberCombat
{
    internal class CompAbility_Lightsaber_Sai : CompAbility_LightsaberBase
    {
        public override void AttackTarget(LocalTargetInfo target)
        {
            var map = parent.pawn.Map;
            var flyer = PawnFlyer.MakeFlyer(ThingDefOf.PawnFlyer, parent.pawn, target.Cell, null, null);
            GenSpawn.Spawn(flyer, parent.pawn.Position, map);
            parent.pawn.Position = target.Cell;
            parent.pawn.stances.SetStance(new Stance_Mobile());
            parent.pawn.meleeVerbs.TryMeleeAttack(target.Pawn, null, true);
        }

    }

    public class CompAbilityProperties_Lightsaber_Sai : CompProperties_AbilityEffect
    {
        public CompAbilityProperties_Lightsaber_Sai()
        {
            compClass = typeof(CompAbility_Lightsaber_Sai);
        }
    }
}