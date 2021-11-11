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
        private string _positionId;
        public string PositionId
        {
            get => _positionId;
            set
            {
                _positionId = value;
                RaisePropertyChanged();
            }
        }
        private Vector _position;
        public Vector Position
        {
            get => _position;
            set
            {
                _position = value;
                RaisePropertyChanged();
            }
        }

        private Image _image { get; }
        private Structure _structure { get; }
        private Vector SIU { get; }
        private VVector PlanIso { get; }

        public MarkerViewModel(Vector position, string positionId)
        {
            Position = position;
            PositionId = positionId;
        }

        public MarkerViewModel(Image image, Structure structure, IEnumerable<VVector> isocenters)
        {
            _structure = structure;
            _image = image;
            SIU = new Vector(image.UserOrigin.x, image.UserOrigin.y, image.UserOrigin.z);

            foreach (VVector isocenter in isocenters)
            {
                Position = new Vector(Math.Round((isocenter.x - SIU.X) / 10, 2), Math.Round((isocenter.y - SIU.Y) / 10, 2), Math.Round((isocenter.z - SIU.Z) / 10, 2));
                if (isocenter.Equals(SIU))
                {
                    PositionId = "UserOrigin";
                    break;
                }
                else if (isocenter.Equals(structure.CenterPoint))
                {
                    PositionId = structure.Id;
                    break;
                }
            }
        }
    }
}