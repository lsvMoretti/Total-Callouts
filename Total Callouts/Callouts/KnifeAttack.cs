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
    [CalloutInfo("KnifeAttack", CalloutProbability.VeryHigh)]

    public class KnifeAttack : Callout
    {
        private Ped Suspect;
        private Ped Victim;
        private Vector3 SpawnPoint;
        private Vector3 VictimPoint;
        private Blip SuspectBlip;
        private Blip VictimBlip;
        private LHandle Pursuit;
        private bool PursuitCreated = false;
        private bool Notified = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(250f));
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 30f);
            AddMinimumDistanceCheck(20f, SpawnPoint);
            CalloutMessage = "Knife Brandished";
            CalloutPosition = SpawnPoint;
            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT ASSISTANCE_REQUIRED IN_OR_ON_POSITION", SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Suspect = new Ped(SpawnPoint);
            if(Suspect)
            {
                Suspect.Inventory.GiveNewWeapon("WEAPON_KNIFE", 1, true);
                SuspectBlip = Suspect.AttachBlip();
                SuspectBlip.IsFriendly = false;
                VictimPoint = Suspect.GetOffsetPositionRight(2);
                Victim = new Ped(VictimPoint);
            }
            
            if(Victim)
            {
                Victim.Tasks.Cower(-1);
                Suspect.Tasks.AimWeaponAt(Victim, -1);
            }
           
            Game.DisplaySubtitle("Detain the ~r~suspect~w~.", 7500);
            Game.DisplayHelp("Press ~r~End~w~ to finish the callout");

            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            if (!PursuitCreated && Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) <= 80f)
            {
                if(Suspect && Victim)
                {
                    Suspect.Tasks.FireWeaponAt(Victim, -1, FiringPattern.FullAutomatic);
                }
            }
            if (!PursuitCreated && Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) <= 10f)
            {
                if(Suspect && Victim)
                {
                    Suspect.Tasks.ReactAndFlee(Victim);
                    Pursuit = Functions.CreatePursuit();
                    Functions.AddPedToPursuit(Pursuit, Suspect);
                    Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                    PursuitCreated = true;
                }
            }
            if (PursuitCreated && !Functions.IsPursuitStillRunning(Pursuit) && Notified == false)
            {
                Game.DisplaySubtitle("Check the ~o~victim~w~.", 7500);
                SuspectBlip.Delete();
                if (Victim)
                {
                    VictimBlip = Victim.AttachBlip();
                    VictimBlip.IsFriendly = true;
                    VictimBlip.EnableRoute(System.Drawing.Color.Yellow);
                    if (Game.LocalPlayer.Character.Position.DistanceTo(VictimPoint) <= 20f)
                    {
                        Game.DisplayHelp("Press ~r~End~w~ to finish the callout");
                        Notified = true;
                    }
                }
            }
            if (Game.IsKeyDownRightNow(Keys.End))
            {
                Game.DisplayNotification("You have ended the callout");
                End();
            }
            base.Process();
        }
        public override void End()
        {
            if (Suspect.Exists()) { Suspect.Dismiss(); }
            if (Victim.Exists()) { Victim.Dismiss(); }
            if (SuspectBlip.Exists()) { SuspectBlip.Delete(); }
            if (VictimBlip.Exists()) { Victim.Delete(); }
            base.End();
        }
    }
}
