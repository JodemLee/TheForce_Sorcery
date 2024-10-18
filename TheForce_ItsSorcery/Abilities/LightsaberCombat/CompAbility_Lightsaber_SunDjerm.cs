using RimWorld;
using TheForce_ItsSorcery.Abilities;
using Verse;

namespace TheForce_ItsSorcery.Abilities.LightsaberCombat
{
    internal class CompAbility_Lightsaber_SunDjerm : CompAbility_LightsaberBase
    {
        public override void AttackTarget(LocalTargetInfo target)
        {
            base.AttackTarget(target);

            // Check if the target has a weapon
            if (target.Pawn != null && target.Pawn.equipment != null && target.Pawn.equipment.Primary != null)
            {
                // Drop the weapon from the pawn's hand
                ThingWithComps weapon = target.Pawn.equipment.Primary;
                target.Pawn.equipment.TryDropEquipment(weapon, out ThingWithComps droppedWeapon, target.Pawn.Position, true);

                // Destroy the dropped weapon
                if (droppedWeapon != null)
                {
                    droppedWeapon.Destroy();
                }
            }
        }
    }

    public class CompAbilityProperties_Lightsaber_SunDjerm : CompProperties_AbilityEffect
    {
        public CompAbilityProperties_Lightsaber_SunDjerm()
        {
            compClass = typeof(CompAbility_Lightsaber_SunDjerm);
        }
    }
}