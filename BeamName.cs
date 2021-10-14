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
[assembly: AssemblyVersion("1.0.0.2")]
[assembly: AssemblyFileVersion("1.0.0.2")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
[assembly: ESAPIScript(IsWriteable = true)]

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
            string msg = string.Empty;
            double G_First = new double();
            double G_Last = new double();
            int dummy = new int(); dummy = 0;
            int a = new int(); a = 1;

            List<String> TreatmentBeam = new List<String>();
            List<String> SetupBeam = new List<String>();
            List<String> BeamName = new List<String>();
            foreach (Beam beam in context.PlanSetup.Beams)
            {
                if (beam.IsSetupField != true) TreatmentBeam.Add(beam.Id.Substring(0, beam.Id.Length));
                if (beam.IsSetupField == true) SetupBeam.Add(beam.Id.Substring(0, beam.Id.Length));
            }
            var TxBeam = context.PlanSetup.Beams.Where(s => TreatmentBeam.Contains(GetBeamID(s))).ToList();
            var SBeam = context.PlanSetup.Beams.Where(s => SetupBeam.Contains(GetBeamID(s))).ToList();
        
            if (SetupBeam.Count == 3)
            {
                var CBCT = SBeam.Where(o => o.ControlPoints.First().GantryAngle.Equals(0)).Last();
                foreach (Beam beam in SBeam)
                    switch(Convert.ToInt32(beam.ControlPoints.First().GantryAngle))
                    {
                        case 0:
                        case 90:
                        case 270:
                            dummy = dummy +1;
                            break;
                    }
                if (dummy == 3 || dummy == 4)
                {
                    foreach (Beam beam in SBeam) if (GetBeamID(beam) != GetBeamID(CBCT))
                        {
                            beamID += "\n" + GetBeamID(beam) + "\t---->SetupG" + beam.ControlPoints.First().GantryAngle;
                            BeamName.Add("SetupG" + beam.ControlPoints.First().GantryAngle);
                        }
                    foreach (Beam beam in SBeam) if (GetBeamID(beam) == GetBeamID(CBCT))
                        {
                            beamID += "\n" + GetBeamID(beam) + "\t---->CBCT";
                            BeamName.Add("CBCT");
                        }
                    foreach (Beam beam in TxBeam)
                    {
                        switch (beam.MLCPlanType.ToString())
                        {
                            case "Static":
                                G_First = Convert.ToInt32((beam.ControlPoints.First().GantryAngle));
                                beamID += "\n" + GetBeamID(beam) + "\t---->1-"+ a + "G" + beam.ControlPoints.First().GantryAngle;
                                BeamName.Add("1-" + a + "G" + G_First);
                                break;

                            default:
                                G_First = Convert.ToInt32((beam.ControlPoints.First().GantryAngle));
                                G_Last = Convert.ToInt32((beam.ControlPoints.Last().GantryAngle));
                                beamID += "\n" + GetBeamID(beam) + "\t---->1-" + a + "G" + G_First + "-G" + G_Last;
                                BeamName.Add("1-" + a + "G" + G_First + "-G" + G_Last);
                                break;
                        }
                        a = a + 1;
                    }
                    msg = string.Format("Check Beam Names \n\n{0}", beamID);
                }
            }
            if (SetupBeam.Count != 3)
            { 
                foreach (Beam beam in TxBeam)
                {
                    switch (beam.MLCPlanType.ToString())
                    {
                        case "Static":
                            G_First = Convert.ToInt32((beam.ControlPoints.First().GantryAngle));
                            beamID += "\n" + GetBeamID(beam) + "\t---->1-" + a + "G" + beam.ControlPoints.First().GantryAngle;
                            BeamName.Add("1-" + a + "G" + G_First);
                            break;

                        default:
                            G_First = Convert.ToInt32((beam.ControlPoints.First().GantryAngle));
                            G_Last = Convert.ToInt32((beam.ControlPoints.Last().GantryAngle));
                            beamID += "\n" + GetBeamID(beam) + "\t---->1-" + a + "G" + G_First + "-G" + G_Last;
                            BeamName.Add("1-" + a + "G" + G_First + "-G" + G_Last);
                            break;
                    }
                    a = a + 1;
                }
                msg = string.Format("Check Beam Names \n\nSetupG0\t---->New\nSetupG90\t---->New\nCBCT\t---->New{0}", beamID);
            }
            MessageBoxResult Result = System.Windows.MessageBox.Show(msg, "NamerGenie", MessageBoxButton.YesNoCancel, (MessageBoxImage)System.Windows.Forms.MessageBoxIcon.Information);
            if (Result == MessageBoxResult.Yes)
            {
                a = 0;
                window.Content = new UserControl1();
                window.Title = "BeamNamer";
                window.Height = 480;
                window.Width = 420;

                context.Patient.BeginModifications();
                string Something = string.Join(",",BeamName);
                System.Windows.Forms.MessageBox.Show(Something.Trim());
                foreach (Beam beam in context.PlanSetup.Beams) if (beam.IsSetupField == true)
                {
                    beam.Id = BeamName[a];
                    a = a + 1;
                }
                foreach (Beam beam in context.PlanSetup.Beams) if (beam.IsSetupField != true)
                {
                    beam.Id = BeamName[a];
                    a = a + 1;
                }

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
