using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Windows.Forms;
using BeamName;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
// [assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
    public class Script
    {
        public Script()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext context, System.Windows.Window window, ScriptEnvironment environment)
        {
            string beamID = string.Empty;
            double G_First = new double();
            double G_Last = new double();
            List<String> TreatmentBeam = new List<String>();
            List<String> SetupBeam = new List<String>();
            foreach (Beam beam in context.PlanSetup.Beams)
            {
                if (beam.IsSetupField != true) TreatmentBeam.Add(beam.Id.Substring(0, beam.Id.Length));
                if (beam.IsSetupField == true) SetupBeam.Add(beam.Id.Substring(0, beam.Id.Length));
            }
            var TxBeam = context.PlanSetup.Beams.Where(s => TreatmentBeam.Contains(GetBeamID(s))).ToList();
            var SBeam = context.PlanSetup.Beams.Where(s => TreatmentBeam.Contains(GetBeamID(s))).ToList();
            foreach (Beam beam in TxBeam)
            {
                switch (beam.MLCPlanType.ToString())
                {
                    case "Static":
                    beamID += "\n" + GetBeamID(beam) + "\t---->G" + beam.ControlPoints.First().GantryAngle;
                    break;

                    default:
                    G_First = Convert.ToInt32((beam.ControlPoints.First().GantryAngle));
                    G_Last = Convert.ToInt32((beam.ControlPoints.Last().GantryAngle));
                    beamID += "\n" + GetBeamID(beam) + "\t---->G" + G_First + "-G" + G_Last;
                    break;
                }      
             }
            foreach (Beam beam in SBeam)
            {
                if (SetupBeam.Count == 3)
                {

                }
            }
            string msg = string.Format("Check Beam Names \n\nSetupG0\t---->New\nSetupG90\t---->New\nCBCT\t---->New{0}", beamID);
                //msg +=string.Format("{0}",dummy1);
                //msg +=string.Format("{0},{1},{2},{3}",beam.Id,gantryangle,collimatorangle,couchangle);
            MessageBoxResult Result = System.Windows.MessageBox.Show(msg, "NamerGenie", MessageBoxButton.YesNoCancel, (MessageBoxImage)System.Windows.Forms.MessageBoxIcon.Information);
            if (Result == MessageBoxResult.Yes)
            {
                window.Content = new UserControl1();
                window.Title = "BeamNamer";
                window.Height = 480;
                window.Width = 420;
            }
            if (Result == MessageBoxResult.No)
            {
                window.Content = new UserControl1();
                window.Title = "BeamNamer";
                window.Height = 480;
                window.Width = 420;
            }
        }

        private static string GetBeamID(Beam beam) => beam.Id.Substring(0, beam.Id.Length);
    }
}
