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
    public class MVA : Callout
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
