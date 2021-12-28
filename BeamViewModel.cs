using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace BeamName
{
    public class BeamViewModel : ViewModelBase
    {
        public string BeamId { get; }
        public string BeamName { get; set; }
        private string _properBeamName;
        public string ProperBeamName
        {
            get => _properBeamName;
            set
            {
                _properBeamName = value;
                RaisePropertyChanged();
            }
        }
        public double IsocenterX { get; }
        public double IsocenterY { get; }
        public double IsocenterZ { get; }
        public bool IsSetupBeam { get; }
        public bool IsUserDefine { get; set; }
        public double GantryAngle { get; }
        public double LastGantryAngle { get; }
        public string Technique { get; }
        public bool IsCbctBeam { get; }
        private int _courseNumber;
        public int CourseNumber
        {
            get => _courseNumber;
            set
            {
                _courseNumber = value;
                RaisePropertyChanged();
            }
        }
        public int BeamNumber { get; set; }
        public string EnergyModeDisplayName { get; }
        public string UserDefineLocation { get; set; }
        private bool _useEnergyModeInName;
        public bool UseEnergyModeInName
        {
            get => _useEnergyModeInName;
            set
            {
                _useEnergyModeInName = value;
                RaisePropertyChanged();
            }
        }
        public double JawPositionX1 { get; set; }
        public double JawPositionX2 { get; set; }
        public double JawPositionY1 { get; set; }
        public double JawPositionY2 { get; set; }

        private Beam _beam { get; }

        public BeamViewModel(string beamName, double gantryAngle, int beamNumber, double lastGantryAngle, string energyMode, bool isSetupBeam, string technique, string userDefineLocation, bool isCbctBeam = false)
        {
            BeamId = beamName;
            BeamName = beamName;
            GantryAngle = gantryAngle;
            BeamNumber = beamNumber;
            LastGantryAngle = lastGantryAngle;
            EnergyModeDisplayName = energyMode;
            UserDefineLocation = userDefineLocation;
            IsSetupBeam = isSetupBeam;
            Technique = technique;
            IsCbctBeam = isCbctBeam;
            SetProperName();
        }

        public BeamViewModel(Beam beam, int courseNumber, int beamNumber, bool isCbctBeam = false, bool useEnergyModeInName = false, bool isUserDefine = false)
        {
            _beam = beam;

            BeamId = beam.Id;
            BeamName = beam.Name;
            IsocenterX = beam.IsocenterPosition.x;
            IsocenterY = beam.IsocenterPosition.y;
            IsocenterZ = beam.IsocenterPosition.z;
            IsSetupBeam = beam.IsSetupField;
            GantryAngle = beam.ControlPoints.First().GantryAngle;
            LastGantryAngle = beam.ControlPoints.Last().GantryAngle;
            Technique = beam.Technique.ToString();
            IsCbctBeam = isCbctBeam;
            CourseNumber = courseNumber;
            BeamNumber = beamNumber;
            EnergyModeDisplayName = beam.EnergyModeDisplayName;
            UseEnergyModeInName = useEnergyModeInName;
            IsUserDefine = isUserDefine;


            VRect<double> jawPositions = beam.ControlPoints.First().JawPositions;
            JawPositionX1 = jawPositions.X1;
            JawPositionX2 = jawPositions.X2;
            JawPositionY1 = jawPositions.Y1;
            JawPositionY2 = jawPositions.Y2;

            SetProperName();
        }

        public void SetProperName(int totalBeamNumber = 0)
        {
            if (IsSetupBeam)
            {
                if (!IsCbctBeam) ProperBeamName = "SetupG" + GantryAngle.ToString("0");
                else ProperBeamName = "CBCT";
            }
            else
            {
                switch (Technique)
                {
                    case "STATIC":
                    case "SRS STATIC":
                        ProperBeamName = CourseNumber.ToString() + "-" + totalBeamNumber.ToString() + "G" + GantryAngle.ToString("0");
                        break;
                    case "TOTAL":
                        if (GantryAngle == 90)
                        {
                            ProperBeamName = "LL-" + totalBeamNumber.ToString(); 
                        }
                        else 
                        {
                            ProperBeamName = "RL-"+ totalBeamNumber.ToString();
                        }
                        break;

                    default:
                        ProperBeamName = CourseNumber.ToString() + "-" + totalBeamNumber.ToString() + "G" + GantryAngle.ToString("0") + "-G" + LastGantryAngle.ToString("0");
                        break;
                }
                if (UseEnergyModeInName) ProperBeamName += "_" + EnergyModeDisplayName;
                if (IsUserDefine) ProperBeamName += "_" + UserDefineLocation;
            }
        }

        public void RenameBeam()
        {
            _beam.Id = ProperBeamName;
        }
    }
}