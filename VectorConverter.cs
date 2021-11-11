using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BeamName
{
    public class VectorConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty PrecisionProperty = DependencyProperty.Register(nameof(Precision), typeof(int), typeof(VectorConverter));
        public int Precision
        {
            get => (int)GetValue(PrecisionProperty);
            set => SetValue(PrecisionProperty, value);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringFormat = "0.";
            for (int i = 0; i < Precision; i++) stringFormat += "0";

            if (value as Vector is null)
            {
                if (value is null) return "Parameter is null";
                else return $"Error: Tried converting to Vector, but got type {value.GetType()}";
            }
            else
            {
                Vector v = (Vector)value;
                return $"({v.X.ToString(stringFormat)}, {v.Y.ToString(stringFormat)}, {v.Z.ToString(stringFormat)})";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
