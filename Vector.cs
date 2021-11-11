namespace BeamName
{
    public class Vector : ViewModelBase
    {
        private double _x;
        private double _y;
        private double _z;

        public double X
        {
            get => _x;
            set
            {
                _x = value;
                RaisePropertyChanged();
            }
        }
        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                RaisePropertyChanged();
            }
        }
        public double Z
        {
            get => _z;
            set
            {
                _z = value;
                RaisePropertyChanged();
            }
        }

        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
