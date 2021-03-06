using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Text.RegularExpressions;

namespace BeamName
{
    public class MarkerViewModel : ViewModelBase
    {
        private string _newCourse;
        public string NewCourse
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
                _positionId = RemoveIsocenter(value);
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

        public string StructureId { get; }

        private Image _image { get; }
        private Structure _structure { get; }
        private StructureSet structures { get; }
        private Vector SIU { get; }

        public MarkerViewModel(Vector position, string positionId, string newCourse)
        {
            Position = position;
            PositionId = positionId;
            StructureId = RemoveIsocenter(positionId);
            NewCourse = newCourse;
        }

        private string RemoveIsocenter(string positionId)
        {
            string newPositionId = Regex.Replace(positionId, "isocenter", "", RegexOptions.IgnoreCase);
            newPositionId = newPositionId.Replace("_", "");
            return newPositionId;
        }
    }
}