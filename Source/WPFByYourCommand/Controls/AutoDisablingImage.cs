using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace WPFByYourCommand.Controls
{
    public class AutoDisablingImage : Image
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoDisablingImage"/> class.
        /// </summary>

        static AutoDisablingImage()
        {
            // Override the metadata of the IsEnabled property.
            IsEnabledProperty.OverrideMetadata(typeof(AutoDisablingImage), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAutoGreyScaleImageIsEnabledPropertyChanged)));
            //SourceProperty.OverrideMetadata(typeof(AutoDisablingImage), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAutoGreyScaleImageIsEnabledPropertyChanged)));
        }

        /// <summary>
        /// Called when [auto grey scale image is enabled property changed].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAutoGreyScaleImageIsEnabledPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            if (args != null && !(args.OldValue ?? false).Equals(args.NewValue))
            {
                if (source is AutoDisablingImage autoGreyScaleImg)
                    autoGreyScaleImg.Opacity = !Convert.ToBoolean(args.NewValue ?? false, CultureInfo.InvariantCulture) ? 0.4 : 1;
            }
        }


    }
}
