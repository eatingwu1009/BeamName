using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
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
        public PlanSetup IsOldPlan { get; }
        public Vector SIU { get; }
        public string PosId { get; set; }
        public IEnumerable<Course> Courses { get; }
        private int _courseNumber = 1;
        public int CourseNumber
        {
            get => _courseNumber;
            set
            {
                _courseNumber = value;
                if (NumberCheckBox != null && NumberCheckBox.IsChecked.Value) UpdateBeamCourseNumbers(value);
                RaisePropertyChanged();
            }
        }
        private bool _difIsoIsChecked = false;
        public bool DifIsoIsChecked
        {
            get => _difIsoIsChecked;
            set
            {
                _difIsoIsChecked = value;
                RaisePropertyChanged(nameof(DifIsoIsChecked));
            }
        }

        public string UserDefineID { get; set; }
        public string UserDefineLocation { get; set; }

        public UserControl1(ScriptContext scriptContext)
        {
            SC = scriptContext;

            List<Beam> beams = new List<Beam>();
            List<Structure> markerStructures = new List<Structure>();
            //Here for PlanSum
            int q = 0;
            if (SC.PlanSetup is null)
            {
                for (int i = 0; i < SC.PlanSumsInScope.Count(); i++)
                {
                    MessageBoxResult Result = MessageBox.Show("Is this PlanSum you would like to edit the Beam? \n\n"
                        + SC.PlanSumsInScope.ElementAt(i).PlanSetups.LastOrDefault().CreationDateTime + " : "
                        + SC.PlanSumsInScope.ElementAt(i).Id, "", MessageBoxButton.YesNo);
                    if (Result == MessageBoxResult.Yes)
                    {
                        StructureSet PlanSumSS = SC.PlanSumsInScope.ElementAt(i).StructureSet;
                        SIU = new Vector(PlanSumSS.Image.UserOrigin.x, PlanSumSS.Image.UserOrigin.y, PlanSumSS.Image.UserOrigin.z);
                        foreach (Structure st in PlanSumSS.Structures.Where(s => s.DicomType == "MARKER"))
                        {
                            markerStructures.Add(st);
                        }
                        for (int a = 0; a < SC.PlanSumsInScope.ElementAtOrDefault(i).PlanSetups.Count(); a++)
                        {
                            foreach (Beam beam in SC.PlanSumsInScope.ElementAt(i).PlanSetups.ElementAt(a).Beams) beams.Add(beam);
                        }
                        q = i;
                        break;
                    }
                }
            }
            else
            {
                beams = SC.PlanSetup.Beams.ToList();
                SIU = new Vector(SC.Image.UserOrigin.x, SC.Image.UserOrigin.y, SC.Image.UserOrigin.z);
                foreach (Structure st in SC.StructureSet.Structures.Where(s => s.DicomType == "MARKER"))
                {
                    markerStructures.Add(st);
                }
            }

            IEnumerable<VVector> isocenters = beams.Where(b => !b.IsSetupField).Select(b => b.IsocenterPosition).Distinct();
            IEnumerable<Course> Courses = SC.Patient.Courses;
            BeamViewModels = new ObservableCollection<BeamViewModel>();
            MarkerViewModels = new ObservableCollection<MarkerViewModel>();
            CourseNumber = 1;
            //int lastcourse = Courses.Count() - 2;
            //string lastbeamcourse = Courses.ElementAtOrDefault(lastcourse).PlanSetups.Where(s => s.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved).LastOrDefault().Beams.Where(s => !s.IsSetupField).LastOrDefault().Id.FirstOrDefault().ToString();
            IsOldPlan = SC.ExternalPlansInScope.Where(s => s.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved).LastOrDefault();
            if (IsOldPlan != null)
            {
                string lastbeamcourse = IsOldPlan.Beams.Where(s => !s.IsSetupField).LastOrDefault().Id.FirstOrDefault().ToString();
                if (int.TryParse(lastbeamcourse, out int value))
                {
                    CourseNumber = int.Parse(lastbeamcourse) + 1;
                }

            }

            IEnumerable<Beam> setupBeams = beams.Where(b => b.IsSetupField).OrderBy(b => b.ControlPoints.First().GantryAngle);
            foreach (var item in setupBeams.Select((beam, i) => new { beam, i }))
            {
                double gantryAngle = item.beam.ControlPoints.First().GantryAngle;
                if (item.i == beams.Where(b => b.IsSetupField).Count() - 1 && gantryAngle == 0) BeamViewModels.Add(new BeamViewModel(item.beam, CourseNumber, item.i, true));
                else BeamViewModels.Add(new BeamViewModel(item.beam, CourseNumber, item.i, false));
            }

            IEnumerable<Beam> couchNonZeroBeams = beams
                .Where(b => !b.IsSetupField && !b.ControlPoints.FirstOrDefault().PatientSupportAngle.Equals(0))
                .OrderBy(b => new Tuple<double, double, double>(b.IsocenterPosition.x, b.IsocenterPosition.y, b.IsocenterPosition.z));
            Dictionary<Tuple<double, double, double>, int> isocenterToBeamNumber = isocenters.ToDictionary(i => new Tuple<double, double, double>(i.x, i.y, i.z), i => 1);
            foreach (Beam beam in couchNonZeroBeams)
            {
                VVector ip = beam.IsocenterPosition;
                var key = new Tuple<double, double, double>(ip.x, ip.y, ip.z);
                BeamViewModels.Add(new BeamViewModel(beam, CourseNumber, isocenterToBeamNumber[key], false));
                if (!beam.Technique.ToString().Equals("TOTAL"))
                {
                    isocenterToBeamNumber[key] = isocenterToBeamNumber[key] + 1;
                }
            }

            IEnumerable<Beam> couchZeroBeams = beams
                .Where(b => !b.IsSetupField && b.ControlPoints.FirstOrDefault().PatientSupportAngle.Equals(0))
                .OrderBy(b => new Tuple<double, double, double>(b.IsocenterPosition.x, b.IsocenterPosition.y, b.IsocenterPosition.z));
            foreach (Beam beam in couchZeroBeams)
            {
                VVector ip = beam.IsocenterPosition;
                var key = new Tuple<double, double, double>(ip.x, ip.y, ip.z);
                BeamViewModels.Add(new BeamViewModel(beam, CourseNumber, isocenterToBeamNumber[key], false));
                if (!beam.Technique.ToString().Equals("TOTAL"))
                {
                    isocenterToBeamNumber[key] = isocenterToBeamNumber[key] + 1;
                }
            }

            //Vector userOrigin = new Vector(0.0, 0.0, 0.0);
            //MarkerViewModels.Add(new MarkerViewModel(userOrigin, "", NewCourse.ToString()));

            foreach (var iso in isocenters)
            {
                Vector translatedIsocenter = TransformToOrigin(new Vector(iso));
                MarkerViewModels.Add(new MarkerViewModel(translatedIsocenter, "", ""));
                foreach (Structure structure in markerStructures)
                {
                    if (IsNear(structure.CenterPoint, iso))
                    {
                        MarkerViewModels.Where(s => s.Position.Equals(translatedIsocenter)).FirstOrDefault().PositionId = structure.Id;
                    }
                }
            }
            DefaltCoursenumber();
            RefreshBeamName();
            InitializeComponent();
            DataContext = this;
        }

        public Vector TransformToOrigin(Vector v)
        {
            double newX = Math.Round((v.X - SIU.X) / 10, 2);
            double newY = Math.Round((v.Y - SIU.Y) / 10, 2);
            double newZ = Math.Round((v.Z - SIU.Z) / 10, 2);
            return new Vector(newX, newY, newZ);
        }

        public bool IsNear(VVector v1, VVector v2, double precision = 0.0001)
        {
            if (Math.Abs(v1.x - v2.x) < precision && Math.Abs(v1.y - v2.y) < precision && Math.Abs(v1.z - v2.z) < precision) return true;
            else return false;
        }

        public bool IsNear(Vector v1, Vector v2, double precision = 0.0001)
        {
            if (Math.Abs(v1.X - v2.X) < precision && Math.Abs(v1.Y - v2.Y) < precision && Math.Abs(v1.Z - v2.Z) < precision) return true;
            else return false;
        }

        public UserControl1(TestContext testContext)
        {
            CourseNumber = 1;
            MarkerViewModels = new ObservableCollection<MarkerViewModel>();
            BeamViewModels = new ObservableCollection<BeamViewModel>();
            for (int i = 0; i < 5; i++)
            {
                MarkerViewModel m = new MarkerViewModel(new Vector(i, i, i), "Position Id = " + i.ToString(), "");
                m.PositionId = "Position" + i.ToString();
                if (i == 0) m.PositionId = "isocenter_neck";
                MarkerViewModels.Add(m);
            }

            SIU = new Vector(0, 0, 0);

            //BeamViewModel b1 = new BeamViewModel("Beam A", 1.3294, 1, 30.9242, "aaaa", true, "AAAAA", "", "",false);
            //BeamViewModel b2 = new BeamViewModel("Beam EWzuikeaile", 2.3492, 2, 34.343, "bbbbb", false, "STATIC", "lung", "", false);
            //BeamViewModel b3 = new BeamViewModel("Beam Eun-woo", 2.3492, 2, 34.343, "cc", false, "sdsds", "lung", "", false);
            //BeamViewModel b4 = new BeamViewModel("Beam 90 #1", 90.0, 2, 34.343, "cc", false, "TOTAL", "lung", "", false);
            //BeamViewModel b5 = new BeamViewModel("Beam 90 #2", 90.0, 2, 34.343, "cc", false, "TOTAL", "lung", "", false);
            //BeamViewModel b6 = new BeamViewModel("Beam 270 #1", 270.0, 2, 34.343, "cc", false, "TOTAL", "lung", "", false);
            //BeamViewModel b999 = new BeamViewModel("Last Setup Beam", 0.0, 2, 34.343, "cc", true, "TOTAL", "lung", "", false);
            //BeamViewModels.Add(b1);
            //BeamViewModels.Add(b2);
            //BeamViewModels.Add(b3);
            //BeamViewModels.Add(b4);
            //BeamViewModels.Add(b5);
            //BeamViewModels.Add(b6);
            //BeamViewModels.Add(b999);

            for (int i = 0; i < 5; i++)
            {
                BeamViewModel b = new BeamViewModel("AAA", 0, i, 0, i.ToString(), true, "", "", "");
                BeamViewModels.Add(b);
            }

            for (int i = 0; i < 5; i++)
            {
                BeamViewModel b = new BeamViewModel("BBB" + i.ToString(), 0, i, 0, i.ToString(), false, "", "", "");
                BeamViewModels.Add(b);
            }

            RefreshBeamName();
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
            if (DifIsoIsChecked) DoIsoRefresh();
            int lBeamIndex = 1;
            int rBeamIndex = 1;
            int BeamIndex = 1;
            foreach (BeamViewModel beam in BeamViewModels)
            {
                if (beam.Technique == "TOTAL" && beam.GantryAngle == 90)
                {
                    beam.SetProperName(lBeamIndex);
                    lBeamIndex += 1;
                }
                else if (beam.Technique == "TOTAL" && beam.GantryAngle == 270)
                {
                    beam.SetProperName(rBeamIndex);
                    rBeamIndex += 1;
                }
                else if (beam.IsSetupBeam is false)
                {
                    beam.SetProperName(BeamIndex);
                    BeamIndex += 1;
                }
                else beam.SetProperName();
            }

            if (BeamsListBox != null)
            {
                //foreach (var item in beams.Where(b => b.IsSetupField).Select((beam, i) => new { beam, i }))
                //BeamsListBox.SelectedItems.
                int b = 1;
                foreach (BeamViewModel beam in BeamsListBox.SelectedItems.OfType<BeamViewModel>())
                {
                    beam.IsUserDefine = true;
                    beam.UserDefineLocation = UserDefineLocation;
                    beam.BeamNumber = b;
                    b++;
                    try
                    {
                        beam.CourseNumber = int.Parse(UserDefineID);
                    }
                    catch { }
                    beam.SetProperName(beam.BeamNumber);
                    beam.IsUserDefine = false;
                }
            }
        }
        private void Button_ReName(object sender, RoutedEventArgs e)
        {
            SC.Patient.BeginModifications();
            RefreshBeamName();
            foreach (BeamViewModel beam in BeamViewModels)
            {
                beam.RenameBeam();
            }
            //need upgrade
            //foreach (Beam beam in SC.ExternalPlanSetup.Beams.Where(b => b.IsSetupField))
            //{
            //    BeamParameters beamParameters = beam.GetEditableParameters();
            //    beamParameters.SetJawPositions(new VRect<double>(20, 20, 20, 20));
            //    beam.ApplyParameters(beamParameters);
            //}
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
            UpdateBeamCourseNumbers(CourseNumber);
        }
        private void Number_unChecked(object sender, RoutedEventArgs e)
        {
            UpdateBeamCourseNumbers(1);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void UpdateBeamCourseNumbers(int courseNumber)
        {
            foreach (BeamViewModel beam in BeamViewModels)
            {
                beam.CourseNumber = courseNumber;
            }
        }

        private void DifIso_isChecked(object sender, RoutedEventArgs e)
        {
            UpdateBeamCourseNumbers(CourseNumber);
            int i = 0;
            foreach (MarkerViewModel marker in MarkerViewModels)
            {
                marker.NewCourse = (CourseNumber + i).ToString();
                i++;
                foreach (BeamViewModel beamViewModel in BeamViewModels)
                {
                    beamViewModel.IsUserDefine = true;
                    Vector beamPosition = TransformToOrigin(new Vector(beamViewModel.IsocenterX, beamViewModel.IsocenterY, beamViewModel.IsocenterZ));
                    if (IsNear(beamPosition, marker.Position))
                    {
                        beamViewModel.UserDefineLocation = marker.PositionId;
                        if (int.TryParse(marker.NewCourse, out int value))
                        {
                            beamViewModel.CourseNumber = int.Parse(marker.NewCourse);
                        }
                    }
                }
            }
        }

        private void DifIso_isUnchecked(object sender, RoutedEventArgs e)
        {
            DefaltCoursenumber();
            foreach (MarkerViewModel marker in MarkerViewModels)
            {
                foreach (BeamViewModel beamViewModel in BeamViewModels)
                {
                    beamViewModel.IsUserDefine = false;
                    Vector beamPosition = TransformToOrigin(new Vector(beamViewModel.IsocenterX, beamViewModel.IsocenterY, beamViewModel.IsocenterZ));
                    if (IsNear(beamPosition, marker.Position))
                    {
                        beamViewModel.UserDefineLocation = marker.StructureId;
                    }
                }
            }
        }

        private void DoIsoRefresh()
        {
            foreach (MarkerViewModel marker in MarkerViewModels)
            {
                foreach (BeamViewModel beamViewModel in BeamViewModels)
                {
                    beamViewModel.IsUserDefine = true;
                    Vector beamPosition = TransformToOrigin(new Vector(beamViewModel.IsocenterX, beamViewModel.IsocenterY, beamViewModel.IsocenterZ));
                    if (IsNear(beamPosition, marker.Position))
                    {
                        beamViewModel.UserDefineLocation = marker.PositionId;
                        if (int.TryParse(marker.NewCourse, out int value))
                        {
                            beamViewModel.CourseNumber = int.Parse(marker.NewCourse);
                        }
                    }
                }
            }
        }

        private void DefaltCoursenumber()
        {
            int i = 0;
            foreach (MarkerViewModel marker in MarkerViewModels)
            {
                marker.NewCourse = (CourseNumber + i).ToString();
                i++;
                foreach (BeamViewModel beamViewModel in BeamViewModels)
                {
                    beamViewModel.IsUserDefine = true;
                    Vector beamPosition = TransformToOrigin(new Vector(beamViewModel.IsocenterX, beamViewModel.IsocenterY, beamViewModel.IsocenterZ));
                    if (IsNear(beamPosition, marker.Position))
                    {
                        if (int.TryParse(marker.NewCourse, out int value))
                        {
                            beamViewModel.CourseNumber = int.Parse(marker.NewCourse);
                        }
                    }
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DifIsoIsChecked) DoIsoRefresh();
            else DifIso_isUnchecked(null, null);
        }
    }
}
