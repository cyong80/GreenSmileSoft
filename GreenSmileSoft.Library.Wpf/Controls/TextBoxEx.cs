using System;
using System.Windows;
using System.Windows.Controls;

namespace GreenSmileSoft.Library.Wpf.Controls
{
    public class TextBoxEx : TextBox
    {
        public static readonly DependencyProperty WalterMarkProperty = DependencyProperty.Register("WalterMark", typeof(String), typeof(TextBoxEx), new PropertyMetadata("WalterMark"));
        public String WalterMark
        {
            get { return (String)GetValue(WalterMarkProperty); }
            set { SetValue(WalterMarkProperty, value); }
        }

        static TextBoxEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxEx), new FrameworkPropertyMetadata(typeof(TextBoxEx)));
        }
    }
}
