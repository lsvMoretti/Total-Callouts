using System;
using System.Windows.Forms;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Engine.Scripting.Entities;
/*1. Recieve callout of missing person in nearby error.
 2. Head to caller and retrive info of person.
 3. Head out and find missing person.
 4. Take missing person back to caller.*/
namespace Total_Callouts.Callouts
{
    [CalloutInfo("MissingPerson", CalloutProbability.VeryHigh)]

    public class MissingPerson : Callout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
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
