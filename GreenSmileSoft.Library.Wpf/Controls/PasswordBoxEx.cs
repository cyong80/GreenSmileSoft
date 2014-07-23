using System;
using System.Windows;
using System.Windows.Controls;

namespace GreenSmileSoft.Library.Wpf.Controls
{
    public class PasswordBoxEx : TextBoxEx
    {
        private PasswordBox passwordBox;
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(String), typeof(PasswordBoxEx), new PropertyMetadata(""));
        public String Password
        {
            get { return (String)GetValue(PasswordProperty); }
            set
            {
                if (passwordBox != null && passwordBox.Password != value)
                {
                    passwordBox.Password = value;
                }
                SetValue(PasswordProperty, value);
            }
        }

        static PasswordBoxEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordBoxEx), new FrameworkPropertyMetadata(typeof(PasswordBoxEx)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Focusable = false;
            passwordBox = (PasswordBox)Template.FindName("PART_TEXTBOX", this);

            passwordBox.PasswordChanged += (obj, e) =>
            {
                this.Password = passwordBox.Password;
            };
            Password = "";
        }
    }
}
