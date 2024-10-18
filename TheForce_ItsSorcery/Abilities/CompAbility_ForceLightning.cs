using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using VFECore;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace TheForce_ItsSorcery.Abilities
{

    public class CompAbility_ForceLightning : CompAbilityEffect
    {
        private readonly List<IntVec3> tmpCells = new List<IntVec3>();
        private Pawn Pawn => parent.pawn;
        private new CompProperties_AbilityForceLightning Props => (CompProperties_AbilityForceLightning)props;
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target != null && target.Cell.IsValid) // Ensure the target is valid
            {
                List<IntVec3> affectedCells = AffectedCells(target.Cell);

                foreach (IntVec3 cell in affectedCells)
                {
                    List<Thing> thingsInCell = cell.GetThingList(Pawn.Map);

                    foreach (Thing thing in thingsInCell)
                    {
                        if (thing is Pawn targetPawn && targetPawn != Pawn) // Ignore the caster
                        {
                            ShootProjectile(targetPawn);
                        }
                    }
                }
            }
        }

        protected Projectile ShootProjectile(GlobalTargetInfo target)
        {
            // Get the projectile definition from the properties
            if (Props.projectileDef == null)
            {
                Log.Error($"{nameof(CompAbility_ForceLightning)}: Props.projectileDef is null.");
                return null; // Return early if it is null
            }

            // Proceed to create the projectile
            ForceLightningProjectile projectile = ThingMaker.MakeThing(Props.projectileDef) as ForceLightningProjectile;
            if (projectile == null)
            {
                Log.Error($"{nameof(CompAbility_ForceLightning)}: Unable to create projectile from definition.");
                return null; // Handle the null case
            }

            if (Pawn.Map == null)
            {
                Log.Error($"{nameof(CompAbility_ForceLightning)}: Pawn's map is null.");
                return null; // Or handle appropriately
            }

            // Determine the origin based on the pawn's position
            Vector3 origin = Pawn.DrawPosHeld ?? Pawn.Position.ToVector3Shifted();
            IntVec3 positionHeld = Pawn.Position;

            // Spawn the projectile in the map
            Projectile spawnedProjectile = GenSpawn.Spawn(projectile, positionHeld, Pawn.Map) as Projectile;

            // Check if the spawned projectile is of the correct type
            if (!(spawnedProjectile is ForceLightningProjectile forceLightningProjectile))
            {
                Log.Error($"{nameof(CompAbility_ForceLightning)}: Spawned projectile is not a ForceLightningProjectile.");
                return null; // Handle the incorrect type case
            }

            // Handle launching the projectile
            if (target.HasThing)
            {
                forceLightningProjectile.Launch(Pawn, origin, target.Thing, target.Thing, ProjectileHitFlags.All);
            }
            else
            {
                forceLightningProjectile.Launch(Pawn, origin, target.Cell, target.Cell, ProjectileHitFlags.NonTargetWorld);
            }

            return forceLightningProjectile;
        }

        public override void DrawEffectPreview(LocalTargetInfo target)
        {
            GenDraw.DrawFieldEdges(AffectedCells(target));
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            if (Pawn.Faction != null)
            {
                foreach (IntVec3 item in AffectedCells(target))
                {
                    List<Thing> thingList = item.GetThingList(Pawn.Map);
                    for (int i = 0; i < thingList.Count; i++)
                    {
                        if (thingList[i].Faction == Pawn.Faction)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private List<IntVec3> AffectedCells(LocalTargetInfo target)
        {
            tmpCells.Clear();
            Vector3 vector = Pawn.Position.ToVector3Shifted().Yto0();
            IntVec3 intVec = target.Cell.ClampInsideMap(Pawn.Map);
            if (Pawn.Position == intVec)
            {
                return tmpCells;
            }
            float lengthHorizontal = (intVec - Pawn.Position).LengthHorizontal;
            float num = (float)(intVec.x - Pawn.Position.x) / lengthHorizontal;
            float num2 = (float)(intVec.z - Pawn.Position.z) / lengthHorizontal;
            intVec.x = Mathf.RoundToInt((float)Pawn.Position.x + num * Props.range);
            intVec.z = Mathf.RoundToInt((float)Pawn.Position.z + num2 * Props.range);
            float target2 = Vector3.SignedAngle(intVec.ToVector3Shifted().Yto0() - vector, Vector3.right, Vector3.up);
            float num3 = Props.lineWidthEnd / 2f;
            float num4 = Mathf.Sqrt(Mathf.Pow((intVec - Pawn.Position).LengthHorizontal, 2f) + Mathf.Pow(num3, 2f));
            float num5 = 57.29578f * Mathf.Asin(num3 / num4);
            int num6 = GenRadial.NumCellsInRadius(Props.range);
            for (int i = 0; i < num6; i++)
            {
                IntVec3 intVec2 = Pawn.Position + GenRadial.RadialPattern[i];
                if (CanUseCell(intVec2) && Mathf.Abs(Mathf.DeltaAngle(Vector3.SignedAngle(intVec2.ToVector3Shifted().Yto0() - vector, Vector3.right, Vector3.up), target2)) <= num5)
                {
                    tmpCells.Add(intVec2);
                }
            }
            List<IntVec3> list = GenSight.BresenhamCellsBetween(Pawn.Position, intVec);
            for (int j = 0; j < list.Count; j++)
            {
                IntVec3 intVec3 = list[j];
                if (!tmpCells.Contains(intVec3) && CanUseCell(intVec3))
                {
                    tmpCells.Add(intVec3);
                }
            }
            return tmpCells;
            bool CanUseCell(IntVec3 c)
            {
                if (!c.InBounds(Pawn.Map))
                {
                    return false;
                }
                if (c == Pawn.Position)
                {
                    return false;
                }
                if (!Props.canHitFilledCells && c.Filled(Pawn.Map))
                {
                    return false;
                }
                if (!c.InHorDistOf(Pawn.Position, Props.range))
                {
                    return false;
                }
                ShootLine resultingLine;
                return parent.verb.TryFindShootLineFromTo(parent.pawn.Position, c, out resultingLine);
            }
        }

    }

    public class CompProperties_AbilityForceLightning : CompProperties_AbilityEffect
    {
        public float range;

        public float lineWidthEnd;

        public ThingDef filthDef;

        public EffecterDef effecterDef;

        public bool canHitFilledCells;

        public ThingDef projectileDef;

        public CompProperties_AbilityForceLightning()
        {
            compClass = typeof(CompAbility_ForceLightning);
        }
    }


    public class ForceLightningProjectile : ExpandableProjectile
    {
        protected int curDuration;
        public override void Tick()
        {
            base.Tick();

            // Ensure projectile definition and game ticks are valid
            if (this.def == null)
            {
                Log.Error("Projectile def is null in Tick.");
                return;
            }

            if (Find.TickManager == null)
            {
                Log.Error("TickManager is null.");
                return;
            }

            // Apply damage based on tickDamageRate
            if (Find.TickManager.TicksGame % this.def.tickDamageRate == 0)
            {
                var projectileLine = MakeProjectileLine(StartingPosition, DrawPos, this.Map);
                if (projectileLine != null)
                {
                    foreach (var pos in projectileLine)
                    {
                        if (!this.Destroyed)
                        {
                            DoDamage(pos);
                        }
                    }
                }
                else
                {
                    Log.Warning("Projectile line is null.");
                }
            }

            // Handle final animations if not already triggered
            if (!doFinalAnimations && (!IsMoving || pawnMoved))
            {
                doFinalAnimations = true;

                if (this.def.graphicData != null)
                {

                    var finalAnimationDuration = this.def.lifeTimeDuration - this.def.graphicData.MaterialsFadeOut.Length;
                    if (finalAnimationDuration > curDuration)
                    {
                        curDuration = finalAnimationDuration;
                    }

                    // Stop motion if necessary
                    if (!this.def.reachMaxRangeAlways && pawnMoved)
                    {
                        StopMotion();
                    }
                }
                else
                {
                    Log.Warning("Graphic data is null in Tick.");
                }
            }

            // Increment the projectile's lifetime and destroy if exceeded
            if (Find.TickManager.TicksGame % this.TickFrameRate == 0 && def.lifeTimeDuration > 0)
            {
                curDuration++;
                if (curDuration > def.lifeTimeDuration)
                {
                    this.Destroy();
                }
            }
        }

        private void StopMotion()
        {
            if (!stopped)
            {
                stopped = true;
                curPosition = this.DrawPos;
                this.destination = this.curPosition ?? DrawPos; // Ensure curPosition is not null
            }
        }
    }
}



