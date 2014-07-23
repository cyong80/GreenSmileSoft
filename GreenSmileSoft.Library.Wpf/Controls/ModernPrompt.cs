using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GreenSmileSoft.Library.Wpf.Controls
{
    public class ModernPrompt : ModernDialog
    {
        private ICommand closeCommand;
        private MessageBoxResult dialogResult = MessageBoxResult.None;
        private TextBox textbox;

        public ModernPrompt()
            : base()
        {
            this.closeCommand = new RelayCommand(o =>
            {
                var result = o as MessageBoxResult?;
                if (result.HasValue)
                {
                    this.dialogResult = result.Value;
                }
                Close();
            });
            this.OkButton.Command = this.CloseCommand;
            this.CancelButton.Command = this.CloseCommand;
            this.Buttons = new Button[] { this.OkButton, this.CancelButton };
            textbox = new TextBox();
            textbox.TextChanged += textbox_TextChanged;
            this.OkButton.IsEnabled = false;
        }

        void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OkButton.IsEnabled = !string.IsNullOrWhiteSpace(textbox.Text);
        }
        public ICommand CloseCommand
        {
            get { return this.closeCommand; }
        }
        public static MessageBoxResult ShowMessage(string text, string title, out string message)
        {
            var dlg = new ModernPrompt
            {
                Title = title,
                MinHeight = 0,
                MinWidth = 0,
                MaxHeight = 480,
                MaxWidth = 640
            };
            StackPanel sp = new StackPanel();
            BBCodeBlock bcb = new BBCodeBlock { BBCode = text, Margin = new Thickness(0, 0, 0, 8) };
            sp.Children.Add(bcb);
            sp.Children.Add(dlg.textbox);
            dlg.Content = sp;
            dlg.textbox.Focus();
            dlg.ShowDialog();
            message = dlg.textbox.Text;
            return dlg.dialogResult;
        }
    }
}
