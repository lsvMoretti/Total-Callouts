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
    public class KnifeAttack : Callout
    {
        private Ped Suspect;
        private Ped Victim;
        private Vector3 SpawnPoint;
        private Vector3 VictimPoint;
        private Blip SuspectBlip;
        private LHandle Pursuit;
        private bool PursuitCreated = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(250f));
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 30f);
            AddMinimumDistanceCheck(20f, SpawnPoint);
            CalloutMessage = "Assualt";
            CalloutPosition = SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT ASSISTANCE_REQUIRED IN_OR_ON_POSITION", SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Suspect = new Ped(SpawnPoint);
            Suspect.BlockPermanentEvents = true;
            Suspect.Inventory.GiveNewWeapon("WEAPON_KNIFE", 1, true);
            Suspect.Tasks.AimWeaponAt(Victim, -1);
            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.IsFriendly = false;
            VictimPoint = Suspect.GetOffsetPositionRight(2);

            Victim = new Ped(VictimPoint);
            Victim.BlockPermanentEvents = true;
            Victim.Tasks.Cower(-1);

            Game.DisplaySubtitle("Detain the ~r~suspect~w~.", 7500);
            Game.DisplayNotification("Press End to finish the callout");

            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            if (!PursuitCreated && Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) <= 10f)
            {
                Suspect.Tasks.ReactAndFlee(Victim);
                Pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(Pursuit, Suspect);
                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                PursuitCreated = true;
            }
            if (PursuitCreated && !Functions.IsPursuitStillRunning(Pursuit))
            {
                Game.DisplaySubtitle("Check the ~r~victim~w~.", 7500);
                if (Game.LocalPlayer.Character.Position.DistanceTo(VictimPoint) <= 10f)
                {
                    Game.DisplayNotification("Press End to finish the callout");
                }
            }
            if (Game.IsKeyDownRightNow(Keys.End))
            {
                End();
            }
            base.Process();
        }
        public override void End()
        {
            if (Suspect.Exists()) { Suspect.Dismiss(); }
            if (Victim.Exists()) { Victim.Dismiss(); }
            if (SuspectBlip.Exists()) { SuspectBlip.Delete(); }
            base.End();
        }
    }
}
