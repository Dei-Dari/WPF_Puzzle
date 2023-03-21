using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace Puzzle_3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        EventLog myLog = new EventLog();
        /// <summary>
        /// Обработка исключений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //https://docs.microsoft.com/ru-ru/dotnet/standard/io/how-to-write-text-to-a-file
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Exception.txt");
            

            //https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
            MessageBox.Show("Ошибка при выполнении:\n" + e.Exception.Message + "\n\nПодробности:\n" + path, "Ошибка при выполнении", MessageBoxButton.OK, MessageBoxImage.Warning);
            //скопировать в буфер обмена
            //Clipboard.SetText(path);

            //перезаписать файл
            //File.WriteAllText(path, e.Exception.Message);

            //добавление строк на запись            
            string[] lines = { DateTime.Now.ToString(), e.Exception.Message, e.Exception.StackTrace, "" };

            //дозапись строк в файл
            File.AppendAllLines(path, lines);

            //отметка обработки исключения
            e.Handled = true;
        }

        //отобажение предыдущего метода, источника или параметров??
    }
}
