
using Avalonia.Controls;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace RestaurantApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Настройка конфигурации и получение строки подключения
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("gr624_veoal");

            // Пример использования строки подключения (здесь можно вызвать сервисы или инициализировать базу данных)
            Task.Run(async () =>
            {
                // Подключение к базе данных и получение данных
                var employeeService = new EmployeeService(connectionString);
                var employees = await employeeService.GetAllEmployeesAsync();

                // Здесь можно назначить данные в DataContext для отображения в UI
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    DataContext = employees;
                });
            });
        }
    }
}