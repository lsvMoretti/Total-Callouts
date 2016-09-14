//Credits to Toast
using System;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Engine.Scripting.Entities;

//Identify where your Callouts folder is.
namespace Total_Callouts.Callouts
{
    //Name the callout, and set the probability.
    [CalloutInfo("PursuitinProgress", CalloutProbability.Low)]

    //Let PursuitinProgress inherit the Callout class.
    public class PursuitinProgress : Callout
    {
        public LHandle pursuit;
        public Vector3 SpawnPoint;
        public Blip myBlip;
        public Ped mySuspect;
        public Vehicle myVehicle;

        public override bool OnBeforeCalloutDisplayed()
        {
            //Get a valid spawnpoint for the callout.
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(300f));

            //Create a list of VehicleModels to get them randomly generated.
            Model[] VehicleModels = new Model[]
            {
                "NINFEF2", "BUS", "COACH", "AIRBUS", "AMBULANCE", "BARRACKS", "BARRACKS2", "BALLER", "BALLER2", "BANSHEE", "BJXL", "BENSON", "BOBCATXL", "BUCCANEER", "BUFFALO", "BUFFALO2", "BULLDOZER", "BULLET", "BURRITO", "BURRITO2", "BURRITO3", "BURRITO4", "BURRITO5", "CAVALCADE", "CAVALCADE2", "POLICET", "GBURRITO", "CAMPER", "CARBONIZZARE", "CHEETAH", "COMET2", "COGCABRIO", "COQUETTE", "GRESLEY", "DUNE2", "HOTKNIFE", "DUBSTA", "DUBSTA2", "DUMP", "DOMINATOR", "EMPEROR", "EMPEROR2", "EMPEROR3", "ENTITYXF", "EXEMPLAR", "ELEGY2", "F620", "FBI", "FBI2", "FELON", "FELON2", "FELTZER2", "FIRETRUK", "FQ2", "FUGITIVE", "FUTO", "GRANGER", "GAUNTLET", "HABANERO", "INFERNUS", "INTRUDER", "JACKAL", "JOURNEY", "JB700", "KHAMELION", "LANDSTALKER", "MESA", "MESA2", "MESA3", "MIXER", "MINIVAN", "MIXER2", "MULE", "MULE2", "ORACLE", "ORACLE2", "MONROE", "PATRIOT", "PBUS", "PACKER", "PENUMBRA", "PEYOTE", "POLICE", "POLICE2", "POLICE3", "POLICE4", "PHANTOM", "PHOENIX", "PICADOR", "POUNDER", "PRANGER", "PRIMO", "RANCHERXL", "RANCHERXL2", "RAPIDGT", "RAPIDGT2", "RENTALBUS", "RUINER", "RIOT", "RIPLEY", "SABREGT", "SADLER", "SADLER2", "SANDKING", "SANDKING2", "SHERIFF", "SHERIFF2", "SPEEDO", "SPEEDO2", "STINGER", "STOCKADE", "STINGERGT", "SUPERD", "STRATUM", "SULTAN", "AKUMA", "PCJ", "FAGGIO2", "DAEMON", "BATI2"
            };

            //Choose a random vehicle model for myVehicle by using the models we listed above, and spawn it at SpawnPoint.
            myVehicle = new Vehicle(VehicleModels[new Random().Next(VehicleModels.Length)], SpawnPoint);

            //Set myVehicle as persistent, so it doesn't randomly disappear.
            myVehicle.IsPersistent = true;

            //Spawn mySuspect at SpawnPoint.
            mySuspect = new Ped(SpawnPoint);
            

            //Warp mySuspect into myVehicle, -1 represents the drivers seat.
            mySuspect.WarpIntoVehicle(myVehicle, -1);

            //Set mySuspect as persistent, so it doesn't randomly disappear.
            mySuspect.IsPersistent = true;

            //Block permanent events from mySuspect so they don't react weirdly to different things from GTA V.
            mySuspect.BlockPermanentEvents = true;

            //If for some reason, the spawning of mySuspect failed, don't display the callout.
            if (!mySuspect.Exists()) return false;

            //If the peds are valid, display the area that the callout is in.
            this.ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 15f);
            this.AddMinimumDistanceCheck(5f, SpawnPoint);

            //Set the callout message(displayed in the notification), and the position(also shown in the notification)
            this.CalloutMessage = "Pursuit in Progress";
            this.CalloutPosition = SpawnPoint;

            //Play the scanner audio using SpawnPoint to identify "POSITION" stated in "IN_OR_ON_POSITION". These audio files can be found in GTA V > LSPDFR > Police Scanner.
            Functions.PlayScannerAudioUsingPosition("OFFICERS_REPORT CRIME_RESIST_ARREST IN_OR_ON_POSITION", this.SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Attach myBlip to mySuspect to show where they are.
            myBlip = mySuspect.AttachBlip();

            //Display a message to let the user know what to do.
            Game.DisplaySubtitle("Pursue the ~r~suspect~w~.", 7500);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();

            //Clean up what we spawned earlier, since the player didn't accept the callout.

            //This states that if mySuspect exists, then we need to delete it.
            if (mySuspect.Exists()) mySuspect.Delete();

            //This states that if myBlip exists, then we need to delete it.
            if (myBlip.Exists()) myBlip.Delete();
        }

        public override void Process()
        {
            base.Process();
            {
                //This states that if the player is less than or equal to 100 meters away from SpawnPoint, then it will do whatever is in the brackets.
                if (Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) <= 100f)
                {
                    //Create the pursuit.
                    this.pursuit = Functions.CreatePursuit();

                    //Add mySuspect to the pursuit.
                    Functions.AddPedToPursuit(this.pursuit, mySuspect);

                    /*Request Backup for an air unit and one local unit to join into the pursuit.
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.AirUnit);
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);*/
                }
            }
        }

        public override void End()
        {
            //Dismiss mySuspect, so it can be deleted by the game once the player leaves the scene.
            //Delete myBlip, so it doesn't stick around on the minimap annoying the player.

            //This states that if mySuspect exists, then we need to dismiss it.
            if (mySuspect.Exists()) mySuspect.Dismiss();

            //Delete the blip attached to mySuspect.
            if (myBlip.Exists()) myBlip.Delete();

            base.End();
        }
    }
}   