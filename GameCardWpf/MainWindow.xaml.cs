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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameCardWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string imgPath = @"images\TwoClubs — копия.png";

            var webImage = new BitmapImage(new Uri(imgPath));
            var imageControl = new Image();
            imageControl.Source = webImage;
            ContentRoot.Children.Add(imageControl);

            //var aPic = new Image()
            //{
            //    Source= new BitmapImage() { BaseUri = new Uri(imgPath,UriKind.RelativeOrAbsolute)},
            //    Width = 100,
            //    Height = 150, 
            //};
            Image simpleImage = new Image();
            simpleImage.Width = 200;
            simpleImage.Margin = new Thickness(5);

            // Create source.
            BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(imgPath, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            // Set the image source.
            simpleImage.Source = bi;
          //Panel.Children.Add(simpleImage);
        }
    }
}
