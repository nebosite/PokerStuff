using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace OddsMaster
{
    public class ProfitConverter : IValueConverter
    {
        public static ProfitConverter ToCellColor { get; private set; } = new ProfitConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = (double)value;
            if (doubleValue == 0.0) return Brushes.LightGray;
            var ratio = doubleValue / 3.0;
            byte r, g, b;
            if (ratio < 0)
            {
                if (ratio < -1.0) ratio = -1.0;
                byte v = (byte)(-ratio * 255);
                r = 255;
                g = b =  (byte)(255 - v);
            }
            else
            {
                if (ratio > 1.0) ratio = 1.0;
                byte v = (byte)(ratio * 255);
                g = 255;
                b = r = (byte)(255 - v);
            }

            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }

    public enum ConversionType
    {
        TrueToCollapsed,
        TrueToVisible
    }

    public class VisibilityConverter : IValueConverter
    {
        public static VisibilityConverter TrueToCollapsed { get; private set; } = new VisibilityConverter() { ConversionType = ConversionType.TrueToCollapsed };
        public static VisibilityConverter TrueToVisible { get; private set; } = new VisibilityConverter() { ConversionType = ConversionType.TrueToVisible };

        public ConversionType ConversionType { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch(ConversionType)
            {
                case ConversionType.TrueToCollapsed: return !(bool)value ? Visibility.Visible : Visibility.Hidden;
                case ConversionType.TrueToVisible: return (bool)value ? Visibility.Visible : Visibility.Hidden;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
