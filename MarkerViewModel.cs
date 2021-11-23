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
        private int _newCourse;
        public int NewCourse
        {
            get => _newCourse;
            set
            {
                _newCourse = value;
                RaisePropertyChanged();
            }
        }
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
        private StructureSet structures { get; }
        private Vector SIU { get; }

        public MarkerViewModel(Vector position, string positionId, int newCourse)
        {
            Position = position;
            PositionId = positionId;
            NewCourse = newCourse;
        }

        public MarkerViewModel(Image image, StructureSet structures, VVector isocenter)
        {
            _image = image;
            SIU = new Vector(image.UserOrigin.x, image.UserOrigin.y, image.UserOrigin.z);

            foreach (Structure structure in structures.Structures)
            {
                Position = new Vector(Math.Round((structure.CenterPoint.x - SIU.X) / 10, 2), Math.Round((structure.CenterPoint.y - SIU.Y) / 10, 2), Math.Round((structure.CenterPoint.z - SIU.Z) / 10, 2));
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