using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;


namespace Puzzle_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string pathPuzzle;
        int anglePuzzle;
        string pathPicture; //выбранная картинка, путь
        Transform rotatePuzzle;
        Storyboard myStoryboardFade = new();
        DoubleAnimation myDoubleAnimationFade = new(0, TimeSpan.FromSeconds(10), FillBehavior.HoldEnd);

        //public MainWindow(string pathPicture)
        //{
        //    this.pathPicture = pathPicture;
        //}

        public MainWindow(string pathPictureSelect)
        {
            this.pathPicture = pathPictureSelect;
            //            _ = MessageBox.Show(@"Выбор пазлов, предпросмотр, поворот, размещение на поле, возврат с поля.

            //Правый блок - доступные пазлы, выбор пазла для предпросмотра - левая кнопка мыши;
            //Средний блок снизу - предпросмотр картины;
            //Средний блок сверху - предпросмотр выбранного пазла, поворот пазла - правая кнопка мыши,
            //для перемещения на поле, необходимо удерживать левую кнопку мыши на 
            //предпросмотре выбранного пазла до нужного расположения на поле;
            //Левый блок - поле для сбора картины, возврат с поля в доступные пазлы - правая кнопка мыши.");

            InitializeComponent();

            PreviewField();
            PreviewPicture();
            PreviewCutPicture();

            CompletedPuzzle();


            


            //if (flag)
            //{
            //    //if (!myDoubleAnimationFade.IsSealed)
            //    {
            //        AnimationPuzzleSpawn();
            //    }
            //}
        }

        /// <summary>
        /// Просмотр картинки (итоговой)
        /// </summary>
        private void PreviewPicture()
        {
            //PicturePreview.Source = new BitmapImage(new Uri("pack://application:,,,/cut_images/forest.jpg"));   //https://docs.microsoft.com/ru-ru/windows/uwp/app-resources/images-tailored-for-scale-theme-contrast
            PicturePreview.Source = new BitmapImage(new Uri(pathPicture, UriKind.Relative));      //https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.controls.image.source?view=windowsdesktop-6.0
        }


        /// <summary>
        /// Поле (для собирания пазла)
        /// </summary>
        private void PreviewField()
        {
            //string path = @"/cut_images/anime_part_000";  //?? проверка перемещения на имени с пустыми значениями
            //string path = pathPicture[0..^4] + "_part_000"; 
            string path = @"/cut_images/part_000";

            for (int i = 1; i <= 9; i++)
            {

                Image image = new() { Source = new BitmapImage(new Uri(path + ".jpg", UriKind.Relative)) };

                image.Width = 200;
                image.Height = 133;
                image.MaxWidth = 200;
                image.MaxHeight = 133;  

                Panel.SetZIndex(image, 2);          //из метаданных, порядок отображения по Z ("слой")
                image.Margin = new Thickness(1);    //https://docs.microsoft.com/ru-ru/dotnet/desktop/wpf/advanced/alignment-margins-and-padding-overview?view=netframeworkdesktop-4.8
                //image.ToolTip = i;  //для отладки
                image.Name = (path.Substring(path.Length - 8, 7) + i).ToString();  //номер картинки на поле

                PuzzleGrid.Children.Add(image);                

                Grid.SetColumn(image, (i - 1) % 3);
                Grid.SetRow(image, (i - 1) / 3);

            }

        }

        /// <summary>
        /// Просмотр пазла
        /// </summary>
        private void PreviewPuzzle(string path, int angle)
        {
            PuzzlePreview.Source = new BitmapImage(new Uri(path, UriKind.Relative));    //https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.controls.image.source?view=windowsdesktop-6.0

            RotateTransform rotateTransform = new RotateTransform(angle);
            rotateTransform.Angle = angle;
            TransformGroup group = new();
            group.Children.Add(rotateTransform);
            PuzzlePreview.RenderTransform = rotateTransform;

        }

        /// <summary>
        /// Выбор пазла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mouseBtnPreviewPuzzle(object sender, MouseButtonEventArgs e)
        {
            string path = ((System.Windows.Media.Imaging.BitmapImage)((System.Windows.Controls.Image)e.Source).Source).UriSource.OriginalString; //подсказка через дерево wpf для e.Sourse, затем в компиляторе этот же путь подсказка текстовое представление (VS) + быстрые действия по сокращению имени            
            Transform rotate = ((UIElement)((FrameworkElement)e.Source)).RenderTransform;
            int angle = (int)((RotateTransform)(rotate)).Angle;
            PreviewPuzzle(path, angle);
            //??убрать подсветку в списке
            //StatusGameControls.Text = "Повернуть - правая кнопка мыши. Раcположить на поле - удерживать левую кнопку мыши";
        }
        /// <summary>
        /// Пазлы
        /// </summary>
        private void PreviewCutPicture()
        {
            //string path = @"/cut_images/anime_part_00";
            string path = pathPicture[0..^4] + "_part_00";   //пазлы в соответствии с выбранной картинкой //быстрые действия оператор диапазона
            Random rnd = new();  //new Random();
            Point point = new(0.5, 0.5); //new Point(0.5, 0.5);
            //CutPicture.FocusVisualStyle = null;     //??голубая подсветка       

            int[] numberCutPicture = new int[9];      //пазлы в случайном порядке      
            for (int i = 0; i < 9; i++)
            {
                numberCutPicture[i] = i + 1;
            }
            Random rand = new();
            for (int i = numberCutPicture.Length - 1; i >= 1; i--)  //перетасовка Фишера-Йейтса
            {
                int j = rand.Next(i + 1);
                int tmp = numberCutPicture[j];
                numberCutPicture[j] = numberCutPicture[i];
                numberCutPicture[i] = tmp;
            }
            for (int i = 0; i < 9; i++)
            {
                Image image = new() { Source = new BitmapImage(new Uri(path + numberCutPicture[i] + ".jpg", UriKind.Relative)) };   //быстрые действия
                int angle = rnd.Next(0, 4);
                angle = angle * 90;
                RotateTransform rotateTransform = new RotateTransform(angle);    //https://docs.microsoft.com/ru-ru/dotnet/desktop/wpf/graphics-multimedia/how-to-specify-the-origin-of-a-transform-by-using-relative-values?view=netframeworkdesktop-4.8
                image.RenderTransformOrigin = point;        //+VS переходы по метаданным
                image.RenderTransform = rotateTransform;
                image.MaxHeight = 100;
                image.MaxWidth = 100;
                image.Margin = new Thickness(20);
                image.Cursor = Cursors.Hand;    //https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.frameworkelement.cursor?view=windowsdesktop-6.0
                image.MouseLeftButtonDown += mouseBtnPreviewPuzzle;
                image.Name = (path[^7..] + numberCutPicture[i]).ToString();  //оператор диапазона //path.Substring(path.Length - 7) + i).ToString();
                //image.FocusVisualStyle = null;                
                CutPicture.Items.Add(image);
                
            }

        }

        /// <summary>
        /// Предпросмотр и поворот выбранного пазла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mouseBtnPuzzlePreview(object sender, MouseButtonEventArgs e)
        {
            Transform rotate = ((UIElement)((FrameworkElement)e.Source)).RenderTransform;
            int angle = (int)((RotateTransform)(rotate)).Angle;
            angle += 90;
            ((RotateTransform)(((UIElement)((FrameworkElement)e.Source)).RenderTransform)).Angle = angle;
            //https://docs.microsoft.com/ru-ru/dotnet/desktop/wpf/graphics-multimedia/transforms-overview?view=netframeworkdesktop-4.8            
            if(StatusGameControls.Text=="")
            {
                StatusGameControls.Text = "Повернуть - правая кнопка мыши. Раcположить на поле - удерживать левую кнопку мыши";
            }
            
        }


        /// <summary>
        /// Перемещение пазла на поле
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mouseBtnPuzzleMoving(object sender, MouseButtonEventArgs e)
        {
            //??анимация перелета на ячейку //E:\Академия ШАГ\Семестр 2\Домашние задания\Урок 67 (9)_261121\2048\2048.csproj
            //https://docs.microsoft.com/ru-ru/dotnet/csharp/programming-guide/events/how-to-subscribe-to-and-unsubscribe-from-events

            pathPuzzle = ((System.Windows.Media.Imaging.BitmapImage)PuzzlePreview.Source).UriSource.OriginalString;
            rotatePuzzle = ((UIElement)((FrameworkElement)e.Source)).RenderTransform;
            anglePuzzle = (int)((RotateTransform)(rotatePuzzle)).Angle;
            PuzzlePreview.Cursor = Cursors.Hand;
            StatusGameControls.Text = "Раположить на поле - удерживать левую кнопку мыши";
            this.MouseLeftButtonUp += mouseBtnPuzzleMovingEnd;    //отпускание кнопки левой над полем
        }

        /// <summary>
        /// Выбранный пазл на поле
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mouseBtnPuzzleMovingEnd(object sender, MouseButtonEventArgs e)
        {
            //отпускание срабатывает только на поле
            if ((((System.Windows.FrameworkElement)((System.Windows.FrameworkElement)e.Source).Parent) != null) && (((System.Windows.FrameworkElement)((System.Windows.FrameworkElement)e.Source).Parent).Name == "PuzzleGrid"))
            {
                string source = ((System.Windows.Media.Imaging.BitmapImage)((System.Windows.Controls.Image)e.Source).Source).UriSource.OriginalString.ToString();
                //source = (source.Substring(pathPuzzle.Length - 12, 8)).ToString();  //только на "пустых"
                source = (source.Substring(source.Length - 12, 8)).ToString();  //только на "пустых"
                if (source == "part_000")
                {
                    Image image = e.Source as Image;
                    RotateTransform rotateTransform = new();
                    rotateTransform.Angle = anglePuzzle;
                    Point point = new Point(0.5, 0.5);  //вращение вокруг центра, для наклонных                
                    ((Image)(e.Source)).RenderTransform = rotatePuzzle;
                    ((Image)(e.Source)).RenderTransformOrigin = point;
                    int col = Grid.GetColumn((Image)(e.Source));
                    int row = Grid.GetRow((Image)(e.Source));
                    ((Image)(e.Source)).Source = new BitmapImage(new Uri(pathPuzzle, UriKind.Relative));
                    ((Image)(e.Source)).MaxHeight = 133;
                    ((Image)(e.Source)).MaxWidth = 200;
                    //StatusGameControls.Text = @"Выбранный пазл на поле";

                    PreviewPuzzle("", 0);   //очистка предпросмотра пазла

                    foreach (Image eachItem in CutPicture.SelectedItems)    //https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.controls.listbox.selecteditems?view=windowsdesktop-6.0
                    {
                        CutPicture.Items.Remove(eachItem);
                        break;      //
                    }
                }
                this.MouseLeftButtonUp -= mouseBtnPuzzleMovingEnd;
                CompletedPuzzle();

            }

            //this.MouseLeftButtonUp -= mouseBtnPuzzleMovingEnd;

        }

        /// <summary>
        /// Убрать пазл с поля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removePuzzleField(object sender, MouseButtonEventArgs e)
        {
            //??иногда дублируется пазл убранный со списком
            string source = ((System.Windows.Media.Imaging.BitmapImage)((System.Windows.Controls.Image)e.Source).Source).UriSource.OriginalString.ToString();
            //source = (source.Substring(source.Length - 12, 8)).ToString();  //только на "пустых"
            source = (source.Substring(source.Length - 12, 8)).ToString();  //только на "пустых"
            if (source != "part_000")
            {
                int col = Grid.GetColumn((Image)(e.Source));
                int row = Grid.GetRow((Image)(e.Source));

                Image image = new();
                image.Source = ((Image)(e.Source)).Source;
                RotateTransform rotateTransform = new();
                rotateTransform.Angle = anglePuzzle;
                Point point = new Point(0.5, 0.5);  //вращение вокруг центра, для наклонных            
                image.RenderTransform = rotatePuzzle;
                image.RenderTransformOrigin = point;
                string removePuzzle = ((System.Windows.Media.Imaging.BitmapImage)image.Source).UriSource.OriginalString;
                image.Name = (removePuzzle.Substring(pathPuzzle.Length - 12, 8)).ToString();
                image.MaxHeight = 100;
                image.MaxWidth = 100;
                image.Margin = new Thickness(20);
                image.Cursor = Cursors.Hand;    //https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.frameworkelement.cursor?view=windowsdesktop-6.0
                image.MouseLeftButtonDown += mouseBtnPreviewPuzzle;
                RotateTransform rotateTransformField = new();
                rotateTransformField.Angle = 0;
                ((Image)(e.Source)).RenderTransform = rotateTransformField;
                //((Image)(e.Source)).Source = new BitmapImage(new Uri((removePuzzle.Remove(pathPuzzle.Length - 5) + 0 + ".jpg").ToString(), UriKind.Relative));
                ((Image)(e.Source)).Source = new BitmapImage(new Uri(@"/cut_images/part_000.jpg", UriKind.Relative));
                CutPicture.Items.Add(image);
            }
        }

        /// <summary>
        /// Собранный пазл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompletedPuzzle()
        {
            int count = 0;  //счетчик правильно расположенных пазлов
            foreach (Image eachItem in PuzzleGrid.Children)    //https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.controls.listbox.selecteditems?view=windowsdesktop-6.0
            {
                string source = ((eachItem).Source).ToString();
                source = (source.Substring(source.Length - 12, 8)).ToString();
                if (eachItem.Name == source && source != "part_000")
                {
                    Transform rotate = eachItem.RenderTransform;
                    int angle = (int)((RotateTransform)(rotate)).Angle;

                    if (angle % 360 == 0)
                    {
                        count++;
                    }
                }
            }
            if (count == 9)
            {
                //string path = @"/cut_images/anime.jpg";
                string path = pathPicture;
                Image image = new() { Source = new BitmapImage(new Uri(path, UriKind.Relative)) };

                AnimationPuzzleFade();
            }
        }


        /// <summary>
        /// Анимация собранного пазла (исчезновение)
        /// </summary>
        private void AnimationPuzzleFade()
        {
            //Storyboard myStoryboardFade = new();
            //DoubleAnimation myDoubleAnimationFade = new(0, TimeSpan.FromSeconds(10), FillBehavior.HoldEnd);

            myDoubleAnimationFade.From = 1.0;
            myDoubleAnimationFade.To = 0.5;
            myDoubleAnimationFade.Duration = new Duration(TimeSpan.FromSeconds(2)); //https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.media.animation.timeline?view=windowsdesktop-6.0
            myDoubleAnimationFade.Completed += delegate //завершение текущей анимации
            {
                AnimationPuzzleSpawn(); //запуск второй анимации после завершения первой, из метаданных + https://docs.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/from-to-by-animations-overview?view=netframeworkdesktop-4.8
            };

            PuzzleGrid.BeginAnimation(OpacityProperty, myDoubleAnimationFade);
        }


        /// <summary>
        /// Анимация собранного пазла (появление)
        /// </summary>
        private void AnimationPuzzleSpawn()
        {
            //string path = @"/cut_images/anime.jpg";
            string path = pathPicture;
            Image image = new() { Source = new BitmapImage(new Uri(path, UriKind.Relative)) };  
            Panel.SetZIndex(image, 1);
            PuzzleAssembled.Children.Add(image);
            Storyboard myStoryboardSpawn = new();
            DoubleAnimation myDoubleAnimationSpawn = new();
            myDoubleAnimationSpawn.From = 0.5;
            myDoubleAnimationSpawn.To = 1.0;
            myDoubleAnimationSpawn.Duration = new Duration(TimeSpan.FromSeconds(2));            
            myDoubleAnimationSpawn.Completed += delegate    //завершение анимации
            {
                
                Menu();     //вызов диалогово меню
            };

            PuzzleAssembled.BeginAnimation(OpacityProperty, myDoubleAnimationSpawn);  

        }

        /// <summary>
        /// Диалоговое меню, продолжить/завершить
        /// </summary>
        private void Menu()
        {
            //пауза, расположение окна
            Thread.Sleep(4000);

            //this.Hide();

            //Owner.Show();   //вызов основного меню
            ////this.Close();

            MessageBoxResult result = MessageBox.Show("Попробовать другой?", "Пазл собран", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {

                this.Hide();
                //this.Close();
                Owner.Show();   //вызов основного меню
            }
            else
            {
                this.Close();
                Owner.Close();  //закрытие окна
            }
        }

        /// <summary>
        /// Закрытие окна меню, при раннем закрытии окна игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Owner.Close();  //закрытие окна, освобождение из памяти диспетчера задач
        }

               

        /// <summary>
        /// Отображение подсказки для поля под указателем мыши в status bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PuzzleGrid_MouseMove(object sender, MouseEventArgs e)
        {
            StatusGameControls.Text = (string)((System.Windows.FrameworkElement)sender).ToolTip;
        }

        /// <summary>
        /// Скрытие подсказки в status bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PuzzleGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            StatusGameControls.Text = "";
        }

    }

}


//доработка/тестирование/отладка

//+картинка на картинку
//+анимация после предыдущей!! +только вторая срабатывает либо наоборот первая
//+-вывод подсказки в "строку состояния"
//подписи блоков
//+начальное меню выбор правила игры
//+кнопка правила - подсказка игры на поле
//+фон
//анимация перетаскивания либо перелета
//+заставка с меню
//убрать подсветку в list
//+рандом пазлов
//+design Windows Paint 3D//ВНЕШНИЙ ВИД ПАЗЛОВ/ЗАСТАВКА 
//+передача названия картинки
//+выбранная картинка перед стартом
//+кнопка управления сразу после запуска?? не выбрана картинка нажатие несколько раз
//+после завершения пауза и/или меню?? +выдержать паузу, продолжение через диалоговое окно?
//+-скрытия окна меню и вызов его обратно?? //метод Hide //на win 7 некоторых не срабатывает повторное развертывание окна меню
//+остается в памяти, в подробностях диспетчера при нажатии закрытия окна без сбора пазла 
//+сломался выбор картинки, после различного умолчания
//+кнопки меню
//+подсветка кнопки
//нарезка картины
//загрузка картины
//+обработка исключений
//+-запись исключений в журнал
//+exception при нажатии мышей на фон, в изображениях выбирается источник фона, при этом при первом запуске кнопки управления источник не отрабатывает, только после кнопки изображения в управлении такая же ошибка
//+-на border button срабатывает IsMouseOver //шаблон стиля для всех кнопок
//+-exception при попытке удалить с поля сбора и попадании мышей на сетку поля -> серый фон Grid //ShowGridLines для отладки а не отображеня //https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.controls.grid.showgridlines?view=windowsdesktop-6.0
//меню окна
//закрытие окна (из памяти) при раннем исключении, без начала игры
//салют при выигрыше (фокус-группа)
//зависимость от разрешения экрана