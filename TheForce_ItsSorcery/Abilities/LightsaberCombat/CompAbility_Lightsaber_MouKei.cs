using RimWorld;
using System.Collections.Generic;
using System.Linq;
using TheForce_ItsSorcery.Abilities;
using Verse;

namespace TheForce_ItsSorcery.Abilities.LightsaberCombat
{
    internal class CompAbility_Lightsaber_MouKei : CompAbility_LightsaberBase
    {
        public override void AttackTarget(LocalTargetInfo target)
        {
            var manipulationTargets = FindManipulationTarget(target.Pawn);
            if (manipulationTargets.Any())
            {
                foreach (var limb in manipulationTargets)
                {
                    DestroyLimb(target.Pawn, limb);
                }
            }
            else
            {
                Messages.Message("No limbs are left to Amputate.", MessageTypeDefOf.RejectInput);
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
            return 50; //
        }


        private List<BodyPartRecord> FindManipulationTarget(Pawn target)
        {
            var manipulationSources = target.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined)
                .Where(part => part.def.tags.Contains(BodyPartTagDefOf.ManipulationLimbCore) || part.def.tags.Contains(BodyPartTagDefOf.MovingLimbCore)).ToList();

            if (manipulationSources.Any())
            {
                // If there are manipulation sources, return a random one
                return manipulationSources;
            }
            else
            {
                // If there are no manipulation sources, return null    
                return null;
            }
        }
    }

    public class CompAbilityProperties_Lightsaber_MouKei : CompProperties_AbilityEffect
    {
        public CompAbilityProperties_Lightsaber_MouKei()
        {
            compClass = typeof(CompAbility_Lightsaber_MouKei);
        }
    }
}