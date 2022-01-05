﻿using System;
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
        public Vector SIU { get; }
        public string PosId { get; set; }
        public string NewCourse;
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

        public string UserDefineID { get; set; }
        public string UserDefineLocation { get; set; }

        public UserControl1(ScriptContext scriptContext)
        {
            SC = scriptContext;
            SIU = new Vector(SC.Image.UserOrigin.x, SC.Image.UserOrigin.y, SC.Image.UserOrigin.z);
            IEnumerable<Beam> beams = SC.PlanSetup.Beams;
            //Here for PlanSum
            //if (beams is null)
            //{
            //    for(int i = 0; i < SC.PlanSumsInScope.Count()-1; i++)
            //    {
            //        MessageBoxResult Result = MessageBox.Show("Is this PlanSum you would like to edit the Beam? \n\n"+ SC.PlanSumsInScope.ElementAt(i).Id, "", MessageBoxButton.YesNo);
            //        if (Result == MessageBoxResult.Yes)
            //        {
            //            for (int a = 0; a < SC.PlanSumsInScope.ElementAtOrDefault(i).PlanSetups.Count()-1; i++)
            //            {
            //                beams.(SC.PlanSumsInScope.ElementAtOrDefault(i).PlanSetups.ElementAt(a).Beams);
            //            }
            //            break;
            //        }
            //    }
            //}    
            IEnumerable<Structure> markerStructures = SC.StructureSet.Structures.Where(s => s.DicomType == "MARKER");
            IEnumerable<VVector> isocenters = beams.Where(b => !b.IsSetupField).Select(b => b.IsocenterPosition).Distinct();
            IEnumerable<Course> Courses = SC.Patient.Courses;
            BeamViewModels = new ObservableCollection<BeamViewModel>();
            MarkerViewModels = new ObservableCollection<MarkerViewModel>();
            CourseNumber = 1;
            int lastcourse = Courses.Count()-2;
            string lastbeamcourse = Courses.ElementAtOrDefault(lastcourse).PlanSetups.Where(s => s.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved).LastOrDefault().Beams.Where(s => !s.IsSetupField).LastOrDefault().Id.FirstOrDefault().ToString();
            if (Courses.Count() > 1 & int.TryParse(lastbeamcourse, out int value))
            {
                CourseNumber = int.Parse(lastbeamcourse) + 2;
            }
            NewCourse = "";

            foreach (var item in beams.Where(b => b.IsSetupField).Select((beam, i) => new { beam, i }))
            {
                double gantryAngle = item.beam.ControlPoints.First().GantryAngle;
                if (item.i == beams.Where(b => b.IsSetupField).Count() - 1 && gantryAngle == 0) BeamViewModels.Add(new BeamViewModel(item.beam, CourseNumber, item.i, true));
                else BeamViewModels.Add(new BeamViewModel(item.beam, CourseNumber, item.i, false));
            }
            foreach (var item in beams.Where(b => !b.IsSetupField).Select((beam, i) => new { beam, i }))
            {
                BeamViewModels.Add(new BeamViewModel(item.beam, CourseNumber, item.i, false));
            }

            Vector userOrigin = new Vector(0.0, 0.0, 0.0);
            MarkerViewModels.Add(new MarkerViewModel(userOrigin, "", NewCourse.ToString()));

            foreach (var iso in isocenters)
            {
                foreach (Structure structure in markerStructures)
                {
                    if (IsNear(structure.CenterPoint, iso))
                    {
                        Vector translatedIsocenter = new Vector(Math.Round((structure.CenterPoint.x - SIU.X) / 10, 2), Math.Round((structure.CenterPoint.y - SIU.Y) / 10, 2), Math.Round((structure.CenterPoint.z - SIU.Z) / 10, 2));
                        MarkerViewModels.Add(new MarkerViewModel(translatedIsocenter, structure.Id, NewCourse));
                    }
                }
            }

            RefreshBeamName();
            InitializeComponent();
            DataContext = this;
        }

        public bool IsNear(VVector v1, VVector v2, double precision = 0.0001)
        {
            if (Math.Abs(v1.x - v2.x) < precision && Math.Abs(v1.y - v2.y) < precision && Math.Abs(v1.z - v2.z) < precision) return true;
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
                MarkerViewModels.Add(m);
            }

            BeamViewModel b1 = new BeamViewModel("Beam A", 1.3294, 1, 30.9242, "aaaa", true, "AAAAA", "", false);
            BeamViewModel b2 = new BeamViewModel("Beam B", 2.3492, 2, 34.343, "bbbbb", false, "STATIC", "lung", false);
            BeamViewModel b3 = new BeamViewModel("Beam Eun-woo", 2.3492, 2, 34.343, "cc", false, "sdsds", "lung", false);
            BeamViewModel b4 = new BeamViewModel("Beam 90 #1", 90.0, 2, 34.343, "cc", false, "TOTAL", "lung", false);
            BeamViewModel b5 = new BeamViewModel("Beam 90 #2", 90.0, 2, 34.343, "cc", false, "TOTAL", "lung", false);
            BeamViewModel b6 = new BeamViewModel("Beam 270 #1", 270.0, 2, 34.343, "cc", false, "TOTAL", "lung", false);
            BeamViewModel b999 = new BeamViewModel("Last Setup Beam", 0.0, 2, 34.343, "cc", true, "TOTAL", "lung", false);
            BeamViewModels.Add(b1);
            BeamViewModels.Add(b2);
            BeamViewModels.Add(b3);
            BeamViewModels.Add(b4);
            BeamViewModels.Add(b5);
            BeamViewModels.Add(b6);
            BeamViewModels.Add(b999);

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
                foreach (BeamViewModel beam in BeamsListBox.SelectedItems.OfType<BeamViewModel>())
                {
                    beam.IsUserDefine = true;
                    beam.UserDefineLocation = UserDefineLocation;
                    try
                    {
                        beam.CourseNumber = int.Parse(UserDefineID);
                    }
                    catch { }
                    beam.SetProperName(beam.TotalBeamNumber);
                    beam.IsUserDefine = false;
                    beam.CourseNumber = CourseNumber;
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

        private void UserDefine_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            foreach (BeamViewModel beam in BeamViewModels.Where(b => !b.IsSetupBeam))
            {
                beam.IsUserDefine = checkBox.IsChecked.Value;
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
            }
        }
    }
}
