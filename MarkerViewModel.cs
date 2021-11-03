using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace BeamName
{
    public class MarkerViewModel : ViewModelBase
    {
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public double PositionZ { get; set; }

        private Structure _structure { get; }

        public MarkerViewModel(Structure structure, IEnumerable<VVector> isocenters)
        {
            _structure = structure;

            foreach (VVector isocenter in isocenters)
            {
                if (structure.CenterPoint.Equals(isocenter))
                {
                    PositionX = structure.CenterPoint.x / 10;
                    PositionY = structure.CenterPoint.y / 10;
                    PositionZ = structure.CenterPoint.z / 10;
                    break;
                }
            }
        }
    }
}