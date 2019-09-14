using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace OddsMaster
{
    class WpfHelper
    {
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            if(parentObject is T parent)
            {
                return parent;
            }
            else
                return FindParent<T>(parentObject);
        }

        public static object FindParentWithInterface(DependencyObject child, Type interfaceType) 
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            if (interfaceType.IsAssignableFrom(parentObject.GetType()))
            {
                return parentObject;
            }
            else
                return FindParentWithInterface(parentObject, interfaceType);
        }

    }
}
