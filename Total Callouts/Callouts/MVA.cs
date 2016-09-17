//Created by theonethatownz
using System;
using System.Windows.Forms;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Engine.Scripting.Entities;


namespace Total_Callouts.Callouts
{
    [CalloutInfo("MVA", CalloutProbability.VeryHigh)]
    public class MVA : Callout
    {
        private Ped Victim1;
        private Ped Victim2;
        private Vehicle Victim1Veh;
        private Vehicle Victim2Veh;
        private Vector3 SpawnPoint1;
        private Vector3 SpawnPoint2;
        private Vector3 VSpawnPoint1;
        private Vector3 VSpawnPoint2;
        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint1 = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(250f));
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint1, 30f);
            AddMinimumDistanceCheck(20f, SpawnPoint1);
            CalloutMessage = "MVA";
            CalloutPosition = SpawnPoint1;
            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT ASSISTANCE_REQUIRED IN_OR_ON_POSITION", SpawnPoint1);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Victim1 = new Ped(SpawnPoint1);
            if(Victim1)
            {
                VSpawnPoint1 = Victim1.GetOffsetPositionFront(2);
                SpawnPoint2 = Victim1.GetOffsetPositionRight(4);
                Victim2 = new Ped(SpawnPoint2);
                if (Victim2)
                {
                    VSpawnPoint2 = Victim2.GetOffsetPositionFront(2);
                }
                Victim1Veh = new Vehicle("ZENTORNO", VSpawnPoint1);
                Victim2Veh = new Vehicle("FELON", VSpawnPoint2);
                if(Victim1Veh)
                {
                    
                }
                if(Victim2Veh)
                {

                }
            }
            Game.DisplaySubtitle("Check out the ~y~accident~w~.", 7500);
            Game.DisplayHelp("Press ~r~End~w~ to finish the callout");
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            base.Process();
        }
        public override void End()
        { 
            base.End();
        }
    }
}
