using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace MBG.Extensions.WPF
{
    public static class DependencyObjectExtensions
    {
        public static IEnumerable<T> GetChildren<T>(this DependencyObject parent) where T : DependencyObject
        {
            List<T> childControls = new List<T>();
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject childControl = VisualTreeHelper.GetChild(parent, i);
                if (childControl is T)
                {
                    childControls.Add((T)childControl);
                }
            }
            return childControls;
        }
    }
}