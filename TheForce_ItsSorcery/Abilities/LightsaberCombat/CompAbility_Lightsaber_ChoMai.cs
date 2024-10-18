using RimWorld;
using System.Linq;
using TheForce_ItsSorcery.Abilities;
using Verse;

namespace TheForce_ItsSorcery.Abilities.LightsaberCombat
{
    internal class CompAbility_Lightsaber_ChoMai : CompAbility_LightsaberBase
    {
        public override void AttackTarget(LocalTargetInfo target)
        {
            var manipulationTarget = FindManipulationTarget(target.Pawn);
            if (manipulationTarget != null)
            {
                // Calculate damage and destroy the limb
                DestroyLimb(target.Pawn, manipulationTarget);

                // Drop the weapon if the target has one
                if (target.Pawn.equipment != null && target.Pawn.equipment.Primary != null)
                {
                    ThingWithComps weapon = target.Pawn.equipment.Primary;
                    target.Pawn.equipment.TryDropEquipment(weapon, out ThingWithComps droppedWeapon, target.Pawn.Position, true);
                }
            }
            else
            {
                Messages.Message("No manipulable limbs are left.", MessageTypeDefOf.RejectInput);
            }
        }

        private void DestroyLimb(Pawn target, BodyPartRecord limb)
        {
            int damageAmount = CalculateDamageToDestroyLimb(target, limb);
            Log.Message($"Damage needed to destroy limb: {damageAmount}");

            ThingDef weaponDef = target.equipment?.Primary?.def;

            DamageDef cutDamage = DamageDefOf.Cut;
            var damageInfo = new DamageInfo(cutDamage, damageAmount, 50, -1, parent.pawn, limb, weaponDef);

            // Ensure that at least 1 damage is dealt to guarantee limb destruction
            target.TakeDamage(damageInfo);
        }

        private int CalculateDamageToDestroyLimb(Pawn target, BodyPartRecord limb)
        {
            return 50;
        }

        private BodyPartRecord FindManipulationTarget(Pawn target)
        {
            // Filter body parts that can be manipulated
            var manipulationSources = target.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined)
                .Where(part => part.def.tags.Contains(BodyPartTagDefOf.ManipulationLimbSegment)).ToList();

            // Return a random manipulable limb
            return manipulationSources.Any() ? manipulationSources.RandomElement() : null;
        }
    }

    public class CompAbilityProperties_Lightsaber_ChoMai : CompProperties_AbilityEffect
    {
        public CompAbilityProperties_Lightsaber_ChoMai()
        {
            compClass = typeof(CompAbility_Lightsaber_ChoMai);
        }
    }
}

