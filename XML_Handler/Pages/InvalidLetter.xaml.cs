using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using DataFormats = System.Windows.DataFormats;
using XML_Handler.DB.UnitOfWork;
using XML_Handler.Xml;

namespace XML_Handler
{
    public partial class InvalidLetter : Page, INotifyPropertyChanged
    {
        UnitOfWork _unitOfWork = new(new AppContext());
        string storagePath = "";
        bool extraFolderNeeded = true;
        List<XmlPerson> personList = [];

        string _savePath = MainWindow.ReadConfig("savePath") ?? "C:\\";

        public event PropertyChangedEventHandler? PropertyChanged;

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

        public InvalidLetter()
        {
            InitializeComponent();

            textBoxPath.DataContext = this;
        }

        private void FileDrop(object sender, System.Windows.DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                    throw new Exception();

                var fileArr = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in fileArr)
                {
                    if (Path.GetExtension(file) != ".xml")
                        throw new Exception();

                    ProcessFile(file);
                }
            }
            catch
            {
                MessageBox.Show(
                    "Некорректный файл",
                    "Предупреждение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        async void ProcessFile(string xmlPath)
        {
            try
            {
                XmlPerson person = GetPersonFromXml(xmlPath); // обрабатываем xml файл, забираем необходимые данные

                if (person.AllFieldsHasValue())               // проверяем, всё ли удалось извлечь
                {
                    person.fullName!.FixCaps();               // исправляем форматирование (ФИО приходит в верхнем регистре)

                    var district = await _unitOfWork.DistrictRepository.GetByNameAsync(person.district!.name);
                    person.department = district.Department;

                    personList.Add(person);                   // добавляем к общему списку
                    Log(person.fullName.surname);             // отображаем фамилию в интерфейсе
                }
                else throw new Exception("Некоторые данные не собраны");
            }

            catch (Exception e)
            {
                MessageBox.Show(
                    $"Не получилось извлечь данные из xml файла:\n{e.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Log(string surname)
        {
            textBlockLog.Text += $"{personList.Count}. {surname}\n";
        }

        private async Task<InvalidLetterData> SetLetterDataAsync()
        {
            string signatoryName = (comboBoxSignatory.SelectedItem as string)!;
            Signatory signatory = await _unitOfWork.SignatoryRepository.GetByNameAsync(signatoryName);

            string executorName  = (comboBoxExecutor.SelectedItem as string)!;
            Executor executor = await _unitOfWork.ExecutorRepository.GetByNameAsync(executorName);

            return new InvalidLetterData(signatory, executor);
        }

        private XmlPerson GetPersonFromXml(string xmlPath)
        {
            Deserializer deserializer = new(xmlPath);

            string? address = deserializer.GetOneValue(Deserializer.mt, "СтрокаАдреса");
            string? date    = deserializer.GetOneValue(Deserializer.mt, "ДатаРождения");

            var fullName    = deserializer.Deserialize<XmlFullName>(Deserializer.mt, "ФИО")   as XmlFullName;
            var district    = deserializer.Deserialize<XmlDistrict>(Deserializer.ut, "Район") as XmlDistrict;

            return new XmlPerson(fullName, address, district, date);
        }

        private void ResetData()
        {
            personList = [];
            textBlockLog.Text = "       Письма для:\n";
        }

        private async void CreateInvalidLetter_Click(object sender, RoutedEventArgs e)
        {
            if(personList.Count == 0)
            {
                MessageBox.Show(
                    "Не загружен xml файл",
                    "Предупреждение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                    );
                return;
            }

            try
            {
                await CreateInvalidLettersAsync();

                MessageBox.Show(
                    "Письма инвалидам успешно созданы",
                    "Сообщение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                    );
            }
            catch(Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при заполнении шаблона: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
            }
            finally
            {
                ResetData(); // очищаем переменные для нового сеанса
            }
        }

        private async Task CreateInvalidLettersAsync()
        {
            string savePath = MainWindow.ReadConfig("savePath") ?? "C:\\";
            if (extraFolderNeeded)
                savePath = CreateExtraFolder(savePath);

            InvalidLetterData letterData = await SetLetterDataAsync();

            foreach (XmlPerson person in personList)
            {
                // создаем письмо для инвалида в указанной директории
                DocBuilder.CreateLetterToDisabled(savePath, person, letterData);

                var invalid = new InvalidsToHead()
                {
                    Name = person.fullName!.surname + " " + person.fullName.name + " " + person.fullName.patronymic,
                    BirthDate = person.date,
                    District = person.district!.name
                };

                // добавляем данные об обработанном инвалиде в базу данных 
                await _unitOfWork.InvalidsToHeadRepository.AddAsync(invalid);
                await _unitOfWork.SaveAsync();
            }
        }

        private string CreateExtraFolder(string path)
        {
            path += "\\" + DateTime.Now.ToString("yyyy-MM-dd");

            Directory.CreateDirectory(path);

            return path;
        }

        private void ChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                storagePath = dialog.FileName;
                textBoxPath.Text = storagePath;
            }
        }

        private async void InvalidLetter_Loaded(object sender, RoutedEventArgs e)
        {
            // заполняем ComboBox подписантов
            var signatories = (List<string?>) await _unitOfWork.SignatoryRepository.GetNameList();
            comboBoxSignatory.ItemsSource  = signatories;
            comboBoxSignatory.SelectedItem = MainWindow.ReadConfig("selectedSignatory") ?? signatories[0];

            // заполняем ComboBox исполнителей
            var executors = (List<string?>) await _unitOfWork.ExecutorRepository.GetNameList();
            comboBoxExecutor.ItemsSource = executors;
            comboBoxExecutor.SelectedItem = MainWindow.ReadConfig("selectedExecutor") ?? executors[0];
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            extraFolderNeeded = !extraFolderNeeded;
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
