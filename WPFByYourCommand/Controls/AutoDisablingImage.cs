﻿using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace WPFByYourCommand.Controls
{
    [SuppressMessage("Design", "CA1501", Justification = "<En attente>")]
    public class AutoDisablingImage : Image
    {
        public static readonly DependencyProperty GreyOpacityProperty = DependencyProperty.Register(nameof(GreyOpacity), typeof(double), typeof(AutoDisablingImage), new UIPropertyMetadata(0.4, new PropertyChangedCallback(GreyOpacity_Changed)));


        /// <summary>
        /// Initializes a new instance of the <see cref="AutoDisablingImage"/> class.
        /// </summary>
        static AutoDisablingImage()
        {
            // Override the metadata of the IsEnabled property.
            IsEnabledProperty.OverrideMetadata(typeof(AutoDisablingImage), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsEnabled_Changed)));
        }

        /// <summary>
        /// Called when [auto grey scale image is enabled property changed].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsEnabled_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AutoDisablingImage)d).pushOpacity();
        }

        private static void GreyOpacity_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AutoDisablingImage)d).pushOpacity();
        }


        private double lastOpacityValue;
        private void pushOpacity()
        {
            if (Visibility != Visibility.Visible)
            {
                return;
            }

            if (IsEnabled)
            {
                Opacity = lastOpacityValue;
            }
            else
            {
                lastOpacityValue = Opacity;
                Opacity = GreyOpacity;
            }
        }

        [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
        public double GreyOpacity
        {
            get => (double)GetValue(AutoDisablingImage.GreyOpacityProperty);
            set => SetValue(AutoDisablingImage.GreyOpacityProperty, value);
        }





    }
}
