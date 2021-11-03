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
        public ObservableCollection<BeamViewModel> BeamViewModels { get; }
        public ObservableCollection<MarkerViewModel> MarkerViewModels { get; }
        public ScriptContext SC { get; }
        public int CourseNumber { get; set; }

        public UserControl1(ScriptContext scriptContext)
        {
            SC = scriptContext;
            IEnumerable<Beam> beams = SC.PlanSetup.Beams;
            IEnumerable<Structure> structures = SC.StructureSet.Structures;
            IEnumerable<VVector> isocenters = beams.Select(b => b.IsocenterPosition).Distinct();
            BeamViewModels = new ObservableCollection<BeamViewModel>();
            MarkerViewModels = new ObservableCollection<MarkerViewModel>();
            CourseNumber = 1;

            foreach (var item in beams.Where(b => b.IsSetupField).Select((beam, i) => new { beam, i }))
            {
                bool isLastSetupField = false;
                if (item.i == beams.Where(b => b.IsSetupField).Count()) isLastSetupField = true;
                BeamViewModels.Add(new BeamViewModel(item.beam, CourseNumber, item.i, isLastSetupField));
            }

            foreach (var item in beams.Where(b => !b.IsSetupField).Select((beam, i) => new { beam, i }))
            {
                BeamViewModels.Add(new BeamViewModel(item.beam, CourseNumber, item.i));
            }

            foreach(Structure structure in structures.Where(s => s.DicomType == "MARKER"))
            {
                MarkerViewModels.Add(new MarkerViewModel(structure, isocenters));
            }

            InitializeComponent();
            DataContext = this;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            //BeamParameters beamParameters = Beams.GetEditableParameters();
            //beamParameters.SetJawPositions(new VRect<double>(20, 20, 20, 20));
            //beam.ApplyParameters(beamParameters);//Add setupfield until_v16
        }

        private void Button_Back(object sender, RoutedEventArgs e)
        {
            //Window.GetWindow(this).Close();

        }

        private void Button_Apply(object sender, RoutedEventArgs e)
        {
            RefreshBeamName();
        }
        private void RefreshBeamName()
        {
            foreach (BeamViewModel beam in BeamViewModels)
            {
                beam.SetProperName();
            }
        }
        private void Button_ReName(object sender, RoutedEventArgs e)
        {
            SC.Patient.BeginModifications();
            RefreshBeamName();
            foreach(BeamViewModel beam in BeamViewModels)
            {
                beam.RenameBeam();
            }
        }

        private void EngCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (BeamViewModel beam in BeamViewModels.Where(b => !b.IsSetupBeam))
            {
                beam.UserEnergyModeInName = true;
                beam.SetProperName();
            }
        }
    }
}
