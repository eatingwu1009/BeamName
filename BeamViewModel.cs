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
        public string BeamId { get; set; }
        public string BeamName { get; set; }
        public string ProperBeamName { get; set; }
        public double IsocenterX { get; }
        public double IsocenterY { get; }
        public double IsocenterZ { get; }
        public bool IsSetupBeam { get; }
        public double GantryAngle { get; }
        public double LastGantryAngle { get; }
        public string MlcPlanType { get; }
        public bool IsLastSetupBeam { get; }
        public int CourseNumber { get; set; }
        public int BeamNumber { get; set; }
        public string EnergyModeDisplayName { get; }
        public bool UserEnergyModeInName { get; set; }
        public double JawPositionX1 { get; set; }
        public double JawPositionX2 { get; set; }
        public double JawPositionY1 { get; set; }
        public double JawPositionY2 { get; set; }

        private Beam _beam { get; }

        public BeamViewModel(Beam beam, int courseNumber, int beamNumber, bool isLastSetupBeam = false, bool useEnergyModeInName = false)
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
            MlcPlanType = beam.MLCPlanType.ToString();
            IsLastSetupBeam = isLastSetupBeam;
            CourseNumber = courseNumber;
            BeamNumber = beamNumber;
            EnergyModeDisplayName = beam.EnergyModeDisplayName;
            UserEnergyModeInName = useEnergyModeInName;

            VRect<double> jawPositions = beam.ControlPoints.First().JawPositions;
            JawPositionX1 = jawPositions.X1;
            JawPositionX2 = jawPositions.X2;
            JawPositionY1 = jawPositions.Y1;
            JawPositionY2 = jawPositions.Y2;

            SetProperName();
        }

        public void SetProperName()
        {
            if (IsSetupBeam)
            {
                if (!IsLastSetupBeam) ProperBeamName = "SetupG" + GantryAngle.ToString("0");
                else ProperBeamName = "CBCT";
            }
            else
            {
                switch (_beam.MLCPlanType.ToString())
                {
                    case "Static":
                        ProperBeamName = CourseNumber.ToString() + "-" + BeamNumber.ToString() + "G" + GantryAngle.ToString("0");
                        break;

                    default:
                        ProperBeamName = CourseNumber.ToString() + "-" + BeamNumber.ToString() + "G" + GantryAngle.ToString("0") + "-G" + LastGantryAngle.ToString("0");
                        break;
                }

                if (UserEnergyModeInName) ProperBeamName += "_" + EnergyModeDisplayName;
            }
        }

        public void RenameBeam()
        {
            _beam.Id = ProperBeamName;
        }
    }
}