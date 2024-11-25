using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace XML_Handler
{
    /// <summary>
    /// Логика взаимодействия для auxiliaryWindow.xaml
    /// </summary>
    public partial class AuxiliaryWindow : Window
    {
        private Window _owner;
        public AuxiliaryWindow()
        {
            InitializeComponent();
        }

        public AuxiliaryWindow(Window owner)
        {
            InitializeComponent();

            _owner = owner;
            _owner.Loaded += owner_Loaded;
            _owner.LocationChanged += owner_LocationChanged;
            _owner.StateChanged += owner_StateChanged;
            //_owner.Deactivated += _owner_Deactivated;
            //_owner.Activated += _owner_Activated;
            //Activated += WindowHeaderButton_Activated;
            Background = new SolidColorBrush(Colors.Transparent);
            ShowInTaskbar = false;
        }

        private void owner_Loaded(object sender, RoutedEventArgs e)
        {
            Owner = _owner;
            Show();
            UpdatePosition();
        }

        private void owner_StateChanged(object sender, System.EventArgs e)
        {
            UpdatePosition();
        }

        private void owner_LocationChanged(object sender, System.EventArgs e)
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            Top = _owner.Top + 30;
            Left = _owner.Left + 612;
        }

        //void WindowHeaderButton_Activated(object sender, System.EventArgs e)
        //{
        //    Opacity = 1;
        //}
        //void _owner_Activated(object sender, System.EventArgs e)
        //{
        //    Opacity = 1;
        //}
        //void _owner_Deactivated(object sender, System.EventArgs e)
        //{
        //    Opacity = 0.75;
        //}

        private void disabledBtn_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).frame.NavigationService.Navigate(new Uri("/Pages/InvalidLetter.xaml", UriKind.Relative));
            LightenBtn(disabledBtn);
            DarkenBtn(headBtn);
            DarkenBtn(SettingsBtn);
        }

        private void headBtn_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).frame.NavigationService.Navigate(new Uri("/Pages/HeadLetter.xaml", UriKind.Relative));
            DarkenBtn(disabledBtn);
            LightenBtn(headBtn);
            DarkenBtn(SettingsBtn);
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).frame.NavigationService.Navigate(new Uri("/Pages/Settings.xaml", UriKind.Relative));
            DarkenBtn(disabledBtn);
            DarkenBtn(headBtn);
            LightenBtn(SettingsBtn);
        }



        void DarkenBtn(Button btn)
        {
            btn.Background = new SolidColorBrush(Colors.Gray);
            btn.BorderThickness = new Thickness(1, 1, 1, 1);
        }

        void LightenBtn(Button btn)
        {
            btn.Background = new SolidColorBrush(Colors.White);
            btn.BorderThickness = new Thickness(0, 1, 1, 1);
        }
    }
}
