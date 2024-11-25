using System.Configuration;
using System.Windows;

namespace XML_Handler
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MyWindow_Loaded;

            AuxiliaryWindow firstBtn = new AuxiliaryWindow(this);
            firstBtn.Show();
        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AppContext ap = new();
        }

        static public string? ReadConfig(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key];
            }
            catch (ConfigurationErrorsException)
            {
                MessageBox.Show("Error reading app settings");
                return null;
            }
        }

        static public void UpdateConfig(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Ошибка при чтении файла конфигурации");
            }
        }
    }
}