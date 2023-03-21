using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Puzzle_3
{
    /// <summary>
    /// Логика взаимодействия для MenuPuzzle.xaml
    /// </summary>
    public partial class MenuPuzzle : Window
    {
        static string[] NamePicture = new[] { "forest", "elf", "dragon", "anime" , "witcher" };
        //string pathPicture; //выбранная картинка, путь
        static Random rnd = new();
        string pathPicture; //="/cut_images/" + NamePicture[rnd.Next(5)] + ".jpg"; //выбранная картинка, путь, произвольная по умолчанию
        public MenuPuzzle()
        {
            InitializeComponent();
            //MessageBox.Show("oh");
        }

        /// <summary>
        /// Старт, запуск игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClickStart(object sender, RoutedEventArgs e)
        {
            if(pathPicture==null)
            {
                pathPicture = "/cut_images/" + NamePicture[rnd.Next(5)] + ".jpg"; //выбранная картинка, путь, произвольная по умолчанию
            }
            
            this.Hide();
            MainWindow window = new MainWindow(pathPicture);
            pathPicture = null; //для рандом при еще одном старте

            window.Owner = this;    //https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.window.owner?view=windowsdesktop-6.0
            //MessageBox.Show(this.WindowStartupLocation.ToString());
            window.WindowStartupLocation = this.WindowStartupLocation;  //оставить внтури окна??

            //window.WindowStyle = 0; //убрать эффект окна
            window.Show();  //расположить в рамках текущего, или вкладка??  
            //this.Show();
            //UserControl window1 = new();
            //window1 = (UserControl)window;
            //window1.Content = window.Content;
            //MenuPuzzle window2 = new();
            //window2.Content = window.Content;


        }

        /// <summary>
        /// Управление в игре, инструкция
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClickGameControls(object sender, RoutedEventArgs e)
        {
            if (Menu.Children != null)
            {
                ButtonClickClear();
            }
            TextBlock gameControls = new TextBlock();
            gameControls.Text = @"

Правый блок - доступные пазлы, выбор пазла для предпросмотра - левая кнопка мыши;
Средний блок снизу - предпросмотр картины;
Средний блок сверху - предпросмотр выбранного пазла, поворот пазла - правая кнопка мыши,
для перемещения на поле, необходимо удерживать левую кнопку мыши на 
предпросмотре выбранного пазла до нужного расположения на поле;
Левый блок - поле для сбора картины, возврат с поля в доступные пазлы - правая кнопка мыши.";

            gameControls.Margin = new Thickness(50);
            gameControls.FontFamily = new FontFamily("Segoe Print");
            gameControls.FontSize = 20;
            gameControls.TextAlignment = TextAlignment.Justify; //по ширине //https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.controls.textblock.textalignment?view=windowsdesktop-6.0
            gameControls.TextWrapping = TextWrapping.WrapWithOverflow;  //перенос по словам //https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.textwrapping?view=windowsdesktop-6.0
            Menu.Children.Add(gameControls);
            Grid.SetColumn(gameControls, 1);
            Grid.SetColumnSpan(gameControls, 3);
            Panel.SetZIndex(StartMenu, 1);
            StartMenu.Opacity = 0.3;           //прозрачность экрана меню 
            Panel.SetZIndex(gameControls, 2);    //на передний план            
        }

        /// <summary>
        /// очистка предыдущего выбора меню
        /// </summary>
        private void ButtonClickClear()
        {
            foreach (FrameworkElement eachItem in Menu.Children)
            {
                if (eachItem is TextBlock)
                {
                    Menu.Children.Remove((TextBlock)eachItem);
                    break;
                }
                if (eachItem is WrapPanel)
                {
                    Menu.Children.Remove((WrapPanel)eachItem);
                    break;
                }
            }
        }

        /// <summary>
        /// выбор картинок (в наборе)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClickPicture(object sender, RoutedEventArgs e)
        {
            if (Menu.Children != null)
            {
                ButtonClickClear();
            }
            WrapPanel pictureGame = new WrapPanel();

            pictureGame.Margin = new Thickness(50);

            Menu.Children.Add(pictureGame);
            Grid.SetColumn(pictureGame, 1);
            Grid.SetColumnSpan(pictureGame, 3);

            StartMenu.Opacity = 0.3;

            Panel.SetZIndex(pictureGame, 3);    //на передний план
            foreach (String name in NamePicture)
            {
                
                Image image = new Image();
                image.Source = new BitmapImage(new Uri(@"/cut_images/" + name + ".jpg", UriKind.Relative));
                image.Width = 200;
                image.Margin = new Thickness(20);
                pictureGame.Children.Add(image);
            }

            //pictureGame.Children.Add(image);

            //только из списка картинок, для исключения ошибки получения иточника изображения с фона
            pictureGame.MouseLeftButtonUp += mouseBtnPictureSelect;
            




        }

        private void mouseBtnPictureSelect(object sender, MouseButtonEventArgs e)
        {
            //Image imageSelect = new Image();
            //imageSelect.Source = ((Image)(e.Source)).Source;

            //Panel.SetZIndex(e, 0);  //на задний план
            //e.Opacity = 0;
            //if (Menu.Children != null)
            //{
            //    ButtonClickClear();
            //}

            pathPicture = ((System.Windows.Media.Imaging.BitmapImage)((System.Windows.Controls.Image)e.Source).Source).UriSource.OriginalString;    
            

            //((Image)StartMenu.Children).Source= new BitmapImage(new Uri(pathPicture, UriKind.Relative));
            StartPicture.Source = new BitmapImage(new Uri(pathPicture, UriKind.Relative));
            StartPicture.Opacity = 1;
            StartMenu.Opacity = 1;
            this.MouseLeftButtonUp-= mouseBtnPictureSelect;
            ButtonClickClear();


            //Panel.SetZIndex(StartPicture, 1);    //на передний план
            //Panel.SetZIndex(StartMenu, 6);

        }
    }
}
