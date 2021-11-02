using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BeamName
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public ObservableCollection<Beam> Beams { get; set; }
        public HashSet<VVector> UIso { get; set; }
        public List<string> Position { get; set; }
        public List<string> PositionID { get; set; }
        public List<string> BeamName { get; set; }
        public List<string> SetupBeam { get; set; }
        public List<string> TreatmentBeam { get; set; }
        public ScriptContext SC { get; set; }
        public UserControl1(ScriptContext scriptContext)
        {
            SC = scriptContext;
            //Beams = (ObservableCollection<Beam>)scriptContext.ExternalPlanSetup.Beams; 
            BeamName = new List<string>();
            SetupBeam = new List<String>();
            TreatmentBeam = new List<String>();
            UIso = new HashSet<VVector>();
            Position = new List<string>();
            PositionID = new List<string>();

            string beamID = string.Empty;
            string msg = string.Empty;
            double G_First = new double();
            double G_Last = new double();
            int a = new int(); a = 1;

            foreach (Beam beam in SC.PlanSetup.Beams)
            {
                if (beam.IsSetupField != true) TreatmentBeam.Add(GetBeamID(beam));
                if (beam.IsSetupField == true) SetupBeam.Add(GetBeamID(beam));
            }
            var TxBeam = SC.PlanSetup.Beams.Where(s => TreatmentBeam.Contains(GetBeamID(s))).ToList();
            var SBeam = SC.PlanSetup.Beams.Where(s => SetupBeam.Contains(GetBeamID(s))).ToList();
            var CBCT = SBeam.Where(o => o.ControlPoints.First().GantryAngle.Equals(0)).LastOrDefault();

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

            var MarIso = SC.StructureSet.Structures.Where(s => s.DicomType == "MARKER").ToList();
            foreach (Beam beam in SC.ExternalPlanSetup.Beams)
            {
                UIso.Add(beam.IsocenterPosition);
                BeamName.Add(beam.Id);
            }
            var common = MarIso.Where(s => UIso.Equals(s.CenterPoint)).ToList();
            foreach (var c in common)
            {
                Position.Add("x:\t" + Math.Round(c.CenterPoint.x / 10, 2) + "y:\t" + Math.Round(c.CenterPoint.y / 10, 2) + "z:\t" + Math.Round(c.CenterPoint.z / 10, 2));
                PositionID.Add(c.Id);
            }

            InitializeComponent();
            DataContext = this;
        }
        private static string GetBeamID(Beam beam) => beam.Id.Substring(0, beam.Id.Length);
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            //BeamParameters beamParameters = Beams.GetEditableParameters();
            //beamParameters.SetJawPositions(new VRect<double>(20, 20, 20, 20));
            //beam.ApplyParameters(beamParameters);//Add setupfield until_v16
        }

        private void Button_Back(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void Button_Apply(object sender, RoutedEventArgs e)
        {

        }
        private void RefreshBeamName()
        {
            BeamName.Clear();
            //foreach (var structure in _ss.Structures)
            //{
            //    DeleteStructures.Add(new DeleteStructure(structure.Id));
            //}
        }
        private void Button_ReName(object sender, RoutedEventArgs e)
        {
            int a = 0;

            SC.Patient.BeginModifications();
            foreach (Beam beam in SC.PlanSetup.Beams.Where(b => b.IsSetupField))
            {
                beam.Id = BeamName[a];
                //BeamParameters beamParameters = beam.GetEditableParameters();
                //beamParameters.SetJawPositions(new VRect<double>(20, 20, 20, 20));
                //beam.ApplyParameters(beamParameters);//Add setupfield until_v16
                a = a + 1;
            }
            foreach (Beam beam in SC.PlanSetup.Beams.Where(b => !b.IsSetupField))
            {
                beam.Id = BeamName[a];
                a = a + 1;
            }
        }
    }
}
