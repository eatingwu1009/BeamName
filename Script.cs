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
[assembly: AssemblyVersion("1.0.0.4")]
[assembly: AssemblyFileVersion("1.0.0.4")]
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

        [STAThread]
        public static void Main()
        {
            TestContext testContext = new TestContext();

            Window window = new Window();
            window.Content = new UserControl1(testContext);

            System.Windows.Application app = new System.Windows.Application();
            app.Run(window);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext context, System.Windows.Window window, ScriptEnvironment environment)
        {
            window.Content = new UserControl1(context);
            window.Title = "BeamNamer";
            window.Height = 455;
            window.Width = 650;
            //msg = string.Format("Your beams will be Renamed\n\n{0}", beamID + "\n\nIf names are correct please confirm YES🙂.\nIf decide to modify please choose NO🙁‍");
            //if (SetupBeam.Count != 3)
            //{
            //    foreach (Beam beam in TxBeam)
            //    {
            //        switch (beam.MLCPlanType.ToString())
            //        {
            //            case "Static":
            //                G_First = Convert.ToInt32((beam.ControlPoints.First().GantryAngle));
            //                beamID += "\n" + GetBeamID(beam) + "\t---->1-" + a + "G" + beam.ControlPoints.First().GantryAngle;
            //                BeamName.Add("1-" + a + "G" + G_First);
            //                break;

            //            default:
            //                G_First = Convert.ToInt32((beam.ControlPoints.First().GantryAngle));
            //                G_Last = Convert.ToInt32((beam.ControlPoints.Last().GantryAngle));
            //                beamID += "\n" + GetBeamID(beam) + "\t---->1-" + a + "G" + G_First + "-G" + G_Last;
            //                BeamName.Add("1-" + a + "G" + G_First + "-G" + G_Last);
            //                break;
            //        }
            //        a = a + 1;
            //    }
            //    msg = string.Format("Check Beam Names \n\nSetupG0\t---->New\nSetupG90\t---->New\nCBCT\t---->New{0}", beamID);
            //}
            //MessageBoxManager.Yes = "YES🙂";
            //MessageBoxManager.No = "NO🙁";
            //MessageBoxManager.Register();

        }
        //public void SetJawPositions (VRect<double> positions);
        
    }
}
