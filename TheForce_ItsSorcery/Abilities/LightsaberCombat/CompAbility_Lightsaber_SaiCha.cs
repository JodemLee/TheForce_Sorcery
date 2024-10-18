using RimWorld;
using System.Linq;
using TheForce_ItsSorcery.Abilities;
using Verse;

namespace TheForce_ItsSorcery.Abilities.LightsaberCombat
{
    internal class CompAbility_Lightsaber_SaiCha : CompAbility_LightsaberBase
    {
        public override void AttackTarget(LocalTargetInfo target)
        {
            if (target.Pawn != null)
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
            else
            {
                Messages.Message("Invalid target.", MessageTypeDefOf.RejectInput);
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
            // Example calculation, you might want to use actual health data
            return 50;
        }

        private BodyPartRecord FindManipulationTarget(Pawn target)
        {
            var manipulationSources = target.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined)
                .Where(part => part.def.tags.Contains(BodyPartTagDefOf.BreathingPathway)).ToList();

            if (manipulationSources.Any())
            {
                return manipulationSources.RandomElement();
            }
            else
            {
                return null;
            }
        }
    }

    public class CompAbilityProperties_Lightsaber_SaiCha : CompProperties_AbilityEffect
    {
        public CompAbilityProperties_Lightsaber_SaiCha()
        {
            compClass = typeof(CompAbility_Lightsaber_SaiCha);
        }
    }
}
