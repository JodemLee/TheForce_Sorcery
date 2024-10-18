using RimWorld;
using System.Linq;
using TheForce_ItsSorcery.Abilities;
using TheForce_ItsSorcery.Abilities.LightsaberCombat;
using Verse;

namespace TheForce_ItsSorcery.Abilities.LightsaberCombat
{
    internal class CompAbility_Lightsaber_ChoSun : CompAbility_LightsaberBase
    {
        public override void AttackTarget(LocalTargetInfo target)
        {
            var manipulationTarget = FindManipulationTarget(target.Pawn);
            if (manipulationTarget != null)
            {
                DestroyLimb(target.Pawn, manipulationTarget);
            }
            else
            {
                Messages.Message("No limbs are left to manipulate.", MessageTypeDefOf.RejectInput);
            }
        }

        private void DestroyLimb(Pawn target, BodyPartRecord limb)
        {
            int damageAmount = CalculateDamageToDestroyLimb(limb, target);

            ThingDef weaponDef = null;
            if (target.equipment?.Primary != null)
            {
                weaponDef = target.equipment.Primary.def;
            }

            DamageDef cutDamage = DamageDefOf.Cut;
            var damageInfo = new DamageInfo(cutDamage, damageAmount, 50, -1, parent.pawn, limb, weaponDef);
            target.TakeDamage(damageInfo);
        }

        private int CalculateDamageToDestroyLimb(BodyPartRecord limb, Pawn target)
        {
            // Get the current health of the limb
            return 50; //
        }

        private BodyPartRecord FindManipulationTarget(Pawn target)
        {
            var manipulationSources = target.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined)
                .Where(part => part.def.tags.Contains(BodyPartTagDefOf.ManipulationLimbCore) || part.def.tags.Contains(BodyPartTagDefOf.ManipulationLimbSegment)).ToList();

            if (manipulationSources.Any())
            {
                // If there are manipulation sources, return a random one
                return manipulationSources.RandomElement();
            }
            else
            {
                // If there are no manipulation sources, return null
                return null;
            }
        }
    }

    public class CompAbilityProperties_Lightsaber_ChoSun : CompProperties_AbilityEffect
    {
        public CompAbilityProperties_Lightsaber_ChoSun()
        {
            compClass = typeof(CompAbility_Lightsaber_ChoSun);
        }
    }
}
