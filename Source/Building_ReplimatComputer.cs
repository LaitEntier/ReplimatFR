﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;

namespace Replimat
{
    [StaticConstructorOnStartup]
    class Building_ReplimatComputer : Building
    {

        public CompPowerTrader powerComp;

        public static readonly Vector2 BarSize = new Vector2(1.3f, 0.2f);

        public static readonly Material BatteryBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(Color.green);
         
        public static readonly Material BatteryBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.1f, 0.1f, 0.1f));

        public float CapPercent = 0;

        public List<Building_ReplimatFeedTank> GetTanks => Map.listerThings.ThingsOfDef(ReplimatDef.FeedTankDef).Select(x => x as Building_ReplimatFeedTank).Where(x => x.PowerComp.PowerNet == this.PowerComp.PowerNet).ToList();


        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.powerComp = base.GetComp<CompPowerTrader>();
        }

        public override void Draw()
        {
            base.Draw();

            GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
            r.center = this.DrawPos + Vector3.up * 0.1f;
            r.size = BarSize;
            r.fillPercent = CapPercent;
            r.filledMat = BatteryBarFilledMat;
            r.unfilledMat = BatteryBarUnfilledMat;
            r.margin = 0.15f;
            Rot4 rotation = base.Rotation;
           // rotation.Rotate(RotationDirection.Clockwise);
            r.rotation = rotation;
            GenDraw.DrawFillableBar(r);

        }

        public override void Tick()
        {
            base.Tick();

            if (this.IsHashIntervalTick(60))
            {
                float totalAvailableFeedstock = GetTanks.Sum(x => x.storedFeedstock);
                float totalSpace = GetTanks.Sum(x => x.storedFeedstockMax);
                CapPercent = totalAvailableFeedstock / totalSpace;
            }
        }

        public bool Working
        {
            get
            {
                if (powerComp.PowerOn)
                {
                    return true;
                }
                return false;
            }
        }
    }
}