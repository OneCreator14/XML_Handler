using System.Windows;
using System.Windows.Controls;

using XML_Handler.DB.UnitOfWork;



namespace XML_Handler
{
    public partial class Settings : Page
    {
        //private DataTable dataTable = new();

        private UnitOfWork _unitOfWork = new(new AppContext());
        public Settings()
        {
            InitializeComponent();
            Loaded += MyPage_Loaded;
        }

        private async void MyPage_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateInterfaceData();
        }

        private void dataGridDistricts_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            scrollViewerDistricts.ScrollToVerticalOffset(scrollViewerDistricts.VerticalOffset - e.Delta/2);
        }

        private async void comboBoxDistrict_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            string districtName = comboBoxDistrict.SelectedItem.ToString()!;
            var district = await _unitOfWork.DistrictRepository.GetByNameAsync(districtName);

            textBoxHead.Text       = district.Head;
            textBoxDistrictGC.Text = district.DistrictGc;
            textBoxHeadDC.Text     = district.HeadDc;
            textBoxDepartment.Text = district.Department;

            comboBoxHeadGender.SelectedItem = (district.Gender == "М") ? "Мужской" : "Женский";
        }

        private async void ComboBoxEditSignatoryName_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxEditSignatoryName.SelectedItem != null)
            {
                string signatoryName = ComboBoxEditSignatoryName.SelectedItem.ToString()!;
                var signatory = await _unitOfWork.SignatoryRepository.GetByNameAsync(signatoryName);

                TextBoxEditSignatoryPost.Text = signatory.Post;
            }
        }
        private async void ComboBoxEditExecutorName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxEditExecutorName.SelectedItem != null)
            {
                string executorName = ComboBoxEditExecutorName.SelectedItem.ToString()!;
                var executor = await _unitOfWork.ExecutorRepository.GetByNameAsync(executorName);

                TextBoxEditExecutorPhone.Text = executor.Phone;
            }
        }

        // работа с интерфейсом районов
        private async void districtUpd_Click(object sender, RoutedEventArgs e)
        {
            string districtName = comboBoxDistrict.SelectedItem.ToString()!;
            var district = await _unitOfWork.DistrictRepository.GetByNameAsync(districtName);

            district.Name       = comboBoxDistrict.SelectedItem.ToString();
            district.DistrictGc = textBoxDistrictGC.Text;
            district.Gender     = (comboBoxHeadGender.SelectedItem.ToString() == "Мужской") ? "М" : "Ж";
            district.Head       = textBoxHead.Text;
            district.HeadDc     = textBoxHeadDC.Text;
            district.Department = textBoxDepartment.Text;

            await _unitOfWork.SaveAsync();

            await UpdateInterfaceData();
        }

        private async void signatoryAdd_ClickAsync(object sender, RoutedEventArgs e)
        {
            Signatory signatory = new() { 
                Name = TextBoxAddSignatoryName.Text, 
                Post = TextBoxAddSignatoryPost.Text 
            };

            await _unitOfWork.SignatoryRepository.AddAsync(signatory);
            await _unitOfWork.SaveAsync();

            await UpdateInterfaceData();
        }

        // работа с интерфейсом подписантов
        private async void signatoryUpd_ClickAsync(object sender, RoutedEventArgs e)
        {
            string signatoryName = ComboBoxEditSignatoryName.SelectedItem.ToString()!;
            var signatory = await _unitOfWork.SignatoryRepository.GetByNameAsync(signatoryName);

            signatory.Post = TextBoxEditSignatoryPost.Text;

            await _unitOfWork.SaveAsync();

            await UpdateInterfaceData();
        }
        private async void signatoryDel_ClickAsync(object sender, RoutedEventArgs e)
        {
            string signatoryName = ComboBoxDelSignatoryName.SelectedItem.ToString()!;
            Signatory signatory = await _unitOfWork.SignatoryRepository.GetByNameAsync(signatoryName);

            await _unitOfWork.SignatoryRepository.DeleteAsync(signatory);
            await _unitOfWork.SaveAsync();

            await UpdateInterfaceData();
        }

        // работа с интерфейсом исполнителей
        private async void executorAdd_Click(object sender, RoutedEventArgs e)
        {
            Executor executor = new() { 
                Name  = TextBoxExecutorAddName.Text, 
                Phone = TextBoxExecutorAddPhone.Text 
            };

            await _unitOfWork.ExecutorRepository.AddAsync(executor);
            await _unitOfWork.SaveAsync();

            await UpdateInterfaceData();
        }

        private async void executorUpd_Click(object sender, RoutedEventArgs e)
        {
            string executorName = ComboBoxEditExecutorName.SelectedItem.ToString()!;
            Executor executor = await _unitOfWork.ExecutorRepository.GetByNameAsync(executorName);
            executor.Phone = TextBoxEditExecutorPhone.Text;

            await _unitOfWork.SaveAsync();

            await UpdateInterfaceData();
        }

        private async void executorDel_Click(object sender, RoutedEventArgs e)
        {
            string executorName = ComboBoxDelExecutorName.SelectedItem.ToString()!;
            Executor executor = await _unitOfWork.ExecutorRepository.GetByNameAsync(executorName);

            await _unitOfWork.ExecutorRepository.DeleteAsync(executor);
            await _unitOfWork.SaveAsync();

            await UpdateInterfaceData();
        }

        private async Task UpdateInterfaceData()
        {
            dataGridDistricts.  ItemsSource = await _unitOfWork.DistrictRepository .GetAllAsync();
            dataGridSignatories.ItemsSource = await _unitOfWork.SignatoryRepository.GetAllAsync();
            dataGridExecutors.  ItemsSource = await _unitOfWork.ExecutorRepository .GetAllAsync();

            //SetSignatoriesHeaders(dataGridSignatories);
            SetDistrictsHeaders  (dataGridDistricts);
            //SetExecutorsHeaders  (dataGridExecutors);

            List<string> genderList = ["Мужской", "Женский"];

            FillComboBox(comboBoxHeadGender, genderList);                                                                                        // заполняем интерфейс для районов
            FillComboBox(comboBoxDistrict,          (await _unitOfWork.DistrictRepository .GetColumn(selector: t => t.Name!) as List<string>)!); // заполняем интерфейс для районов
            FillComboBox(ComboBoxEditSignatoryName, (await _unitOfWork.SignatoryRepository.GetColumn(selector: t => t.Name!) as List<string>)!); // заполняем интерфейс для подписантов
            FillComboBox(ComboBoxDelSignatoryName,  (await _unitOfWork.SignatoryRepository.GetColumn(selector: t => t.Name!) as List<string>)!); // заполняем интерфейс для подписантов
            FillComboBox(ComboBoxEditExecutorName,  (await _unitOfWork.ExecutorRepository .GetColumn(selector: t => t.Name!) as List<string>)!); // заполняем интерфейс для исполнителей
            FillComboBox(ComboBoxDelExecutorName,   (await _unitOfWork.ExecutorRepository .GetColumn(selector: t => t.Name!) as List<string>)!); // заполняем интерфейс для исполнителей
        }
        private void FillComboBox(ComboBox comboBox, List<string> data)
        {
            comboBox.ItemsSource = data;
            comboBox.SelectedItem = data[0];
        }

        private void SetDistrictsHeaders(DataGrid dataGrid)
        {
            dataGrid.Columns[0].Visibility = Visibility.Collapsed;

            dataGrid.Columns[1].Header = "Район";
            dataGrid.Columns[2].Header = "Район (Р.п.)";
            dataGrid.Columns[3].Header = "Пол главы";
            dataGrid.Columns[4].Header = "ФИО главы";
            dataGrid.Columns[5].Header = "ФИО главы (Д.п.)";
            dataGrid.Columns[6].Header = "Отдел обращения";
        }

        private void SetSignatoriesHeaders(DataGrid dataGrid)
        {
            //dataGrid.Columns[0].Visibility = Visibility.Collapsed;

            //dataGrid.Columns[1].Header = "ФИО";
            //dataGrid.Columns[2].Header = "Должность";
        }

        private void SetExecutorsHeaders(DataGrid dataGrid)
        {
            //dataGrid.Columns[0].Visibility = Visibility.Collapsed;

            //dataGrid.Columns[1].Header = "ФИО";
            //dataGrid.Columns[2].Header = "Телефон";
        }

    }
}
