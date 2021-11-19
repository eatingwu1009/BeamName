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
using System.Runtime.CompilerServices;

namespace BeamName
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<BeamViewModel> BeamViewModels { get; }
        public ObservableCollection<MarkerViewModel> MarkerViewModels { get; }
        public ScriptContext SC { get; }
        private int _courseNumber = 1;
        public int CourseNumber
        {
            get => _courseNumber;
            set
            {
                _courseNumber = value;
                RaisePropertyChanged();
            }
        }

        public UserControl1(ScriptContext scriptContext)
        {
            SC = scriptContext;
            IEnumerable<Beam> beams = SC.PlanSetup.Beams;
            IEnumerable<Structure> structures = SC.StructureSet.Structures.Where(s => s.DicomType == "MARKER");
            IEnumerable<VVector> isocenters = beams.Select(b => b.IsocenterPosition).Distinct();
            BeamViewModels = new ObservableCollection<BeamViewModel>();
            MarkerViewModels = new ObservableCollection<MarkerViewModel>();
            CourseNumber = 1;

            foreach (var item in beams.Where(b => b.IsSetupField).Select((beam, i) => new { beam, i }))
            {
                bool isLastSetupField = false;
                if (item.i == beams.Where(b => b.IsSetupField).Count() - 1) isLastSetupField = true;
                BeamViewModels.Add(new BeamViewModel(item.beam, CourseNumber, item.i, isLastSetupField));
            }
            foreach (var item in beams.Where(b => !b.IsSetupField).Select((beam, i) => new { beam, i }))
            {
                BeamViewModels.Add(new BeamViewModel(item.beam, CourseNumber, item.i));
            }
            MarkerViewModels.Add(new MarkerViewModel(new Vector (0,0,0), "UserOrigin" , ""));
            foreach (VVector isocenter in isocenters)
            {
                MarkerViewModels.Add(new MarkerViewModel(new Vector(isocenter), isocenter.ToString(), isocenter.ToString()));
            }

            InitializeComponent();
            DataContext = this;
        }

        public UserControl1(TestContext testContext)
        {
            CourseNumber = 1;
            MarkerViewModels = new ObservableCollection<MarkerViewModel>();
            BeamViewModels = new ObservableCollection<BeamViewModel>();
            for(int i = 0; i < 2; i++)
            {
                MarkerViewModel m = new MarkerViewModel(new Vector(i, i, i), "Position Id = " + i.ToString(), "Position name = " + i.ToString());
                m.PositionId = "Position " + i.ToString();
                MarkerViewModels.Add(m);
            }

            BeamViewModel b1 = new BeamViewModel("Beam A", 1.3294, 1, 30.9242 , "aaaa", true, "AAAAA");
            BeamViewModel b2 = new BeamViewModel("Beam B", 2.3492, 2, 34.343, "bbbbb", false, "STATIC");
            BeamViewModel b3 = new BeamViewModel("Beam Eun-woo", 2.3492, 2, 34.343, "cc", false, "sdsds");
            BeamViewModels.Add(b1);
            BeamViewModels.Add(b2);
            BeamViewModels.Add(b3);

            InitializeComponent();
            DataContext = this;
        }

        private void Button_Back(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
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
            foreach (Beam beam in SC.ExternalPlanSetup.Beams.Where(b => b.IsSetupField))
            {
                BeamParameters beamParameters = beam.GetEditableParameters();
                beamParameters.SetJawPositions(new VRect<double>(20, 20, 20, 20));
                beam.ApplyParameters(beamParameters);
            }
        }

        private void EngCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            foreach (BeamViewModel beam in BeamViewModels.Where(b => !b.IsSetupBeam))
            {
                beam.UseEnergyModeInName = checkBox.IsChecked.Value;
            }
        }

        private void Number_Checked(object sender, RoutedEventArgs e)
        {
            foreach (BeamViewModel beam in BeamViewModels.Where(b => !b.IsSetupBeam))
            {
                beam.CourseNumber = CourseNumber;
            }
        }
        private void Number_unChecked(object sender, RoutedEventArgs e)
        {
            foreach (BeamViewModel beam in BeamViewModels.Where(b => !b.IsSetupBeam))
            {
                beam.CourseNumber = 1;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
