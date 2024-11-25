using Microsoft.WindowsAPICodePack.Dialogs;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using XML_Handler.DB.UnitOfWork;
using MessageBox = System.Windows.Forms.MessageBox;

namespace XML_Handler
{
    public partial class HeadLetter : Page
    {
        string _savePath = MainWindow.ReadConfig("savePath") ?? "C:\\";
        string storagePath = "";

        public event PropertyChangedEventHandler PropertyChanged;

        private UnitOfWork _unitOfWork = new(new AppContext());
        public string savePath
        {
            get { return _savePath; }
            set
            {
                _savePath = value;
                MainWindow.UpdateConfig("savePath", _savePath);
                NotifyPropertyChanged("savePath");
            }
        }
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public HeadLetter()
        {
            InitializeComponent();

            textBoxSavePath.DataContext = this;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string savePath = MainWindow.ReadConfig("savePath") ?? "C:\\";

                var districts = await _unitOfWork.InvalidsToHeadRepository.GetDistrictWithProcessedInvalids() as List<string?>;

                if (districts == null || districts.Count == 0)
                {
                    MessageBox.Show(
                        "Нет обработанных инвалидов",
                        "Предупреждение",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                        );
                    return;
                }

                foreach (var district in districts)
                {
                    HeadLetterData letterData = await SetLetterDataAsync(district!);

                    DocBuilder.CreateLetterToHead(savePath, letterData);
                }

                ResetData();
                MessageBox.Show(
                    "Письма главам успешно созданы",
                    "Сообщение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при заполнении шаблона: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
            }
        }

        private async void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            FillLog();

            // заполняем ComboBox-ы значениями из бд 
            var signatories = await _unitOfWork.SignatoryRepository.GetNameList() as List<string?>;
            comboBoxSignatory.ItemsSource = signatories;
            comboBoxSignatory.SelectedItem = MainWindow.ReadConfig("selectedSignatory") ?? signatories![0];

            // заполняем ComboBox-ы значениями из бд 
            var executors   = await _unitOfWork.ExecutorRepository. GetNameList() as List<string?>;
            comboBoxExecutor.ItemsSource = executors;
            comboBoxExecutor.SelectedItem  = MainWindow.ReadConfig("selectedExecutor")  ?? executors![0];
        }

        private async void FillLog()
        {
            // районы, содержащие обработанных инвалидов для писем главам этих районов
            var districts = await _unitOfWork.InvalidsToHeadRepository.GetDistrictWithProcessedInvalids() as List<string>;

            textBlockHeadLog.Text += "Список граждан для отправки писем главам районов:\n";

            // для каждого района выводим количеств обработанных инвалидов в нём
            foreach (var district in districts!.Select((value, i) => (value, i)))
            {
                textBlockHeadLog.Text += $"\n    {district.i + 1}. {district.value}\n";

                var invalids = await _unitOfWork.InvalidsToHeadRepository.GetInvalidsByDistrict(district.value) as List<string>;

                foreach (var invalid in invalids!.Select((value, j) => (value, j)))
                {
                    textBlockHeadLog.Text += $"        {invalid.j + 1}) {invalid.value}\n";
                }

            }
        }


        private async Task<HeadLetterData> SetLetterDataAsync(string districtName)
        {
            Signatory signatory = await _unitOfWork.SignatoryRepository.GetByNameAsync(comboBoxSignatory.Text);
            Executor  executor  = await _unitOfWork.ExecutorRepository. GetByNameAsync((comboBoxExecutor.SelectedItem as string)!);
            District  district  = await _unitOfWork.DistrictRepository. GetByNameAsync(districtName);

            // список обработанных инвалидов, принадлежащих к данному району
            var personList = await _unitOfWork.InvalidsToHeadRepository.GetListByDistrictAsync(districtName) as List<InvalidsToHead>;

            HeadLetterData res = new(signatory, executor, district, personList!);

            return res;
        }

        private void ChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                storagePath          = dialog.FileName;
                textBoxSavePath.Text = storagePath;
            }
        }

        private async void ResetData()
        {
            await _unitOfWork.InvalidsToHeadRepository.DelTable();
            await _unitOfWork.SaveAsync();
            textBlockHeadLog.Text = "Нет граждан для отправки";
        }

        private void comboBoxSignatory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainWindow.UpdateConfig("selectedSignatory", comboBoxSignatory.SelectedItem.ToString()!);
        }

        private void comboBoxExecutor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainWindow.UpdateConfig("selectedExecutor", comboBoxExecutor.SelectedItem.ToString()!);
        }
    }
}
