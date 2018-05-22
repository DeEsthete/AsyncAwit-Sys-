using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using Newtonsoft.Json;

namespace ApodApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApodObject mainApod;
        public MainWindow()
        {
            InitializeComponent();
            downloadProgressBar.Visibility = Visibility.Collapsed;
            imageApod.Visibility = Visibility.Collapsed;
        }

        private Task<ApodObject> DownloadJson(DateTime? dt)
        {
            return Task.Run(() =>
            {
                if (dt != null)
                {
                    string ValueJson;
                    using (var webClient = new WebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        ValueJson = webClient.DownloadString("https://api.nasa.gov/planetary/apod?api_key=qSotOr0pVzOJwzGJr62vXfp9QgURsWpKc8ngOzdQ" + "&date=" + dt.Value.Date.ToString("yyyy-MM-dd"));
                        return JsonConvert.DeserializeObject<ApodObject>(ValueJson);
                    }
                }
                else
                {
                    string ValueJson;
                    using (var webClient = new WebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        ValueJson = webClient.DownloadString("https://api.nasa.gov/planetary/apod?api_key=qSotOr0pVzOJwzGJr62vXfp9QgURsWpKc8ngOzdQ");
                        return JsonConvert.DeserializeObject<ApodObject>(ValueJson);
                    }
                }
            });
        }

        private Task<MemoryStream> GetImage(ApodObject obj)
        {
            return Task.Run(() =>
            {
                // var c = new WebClient();
                using (var c = new WebClient())
                {
                    var bytes = c.DownloadData(obj.hdurl);
                    var ms = new MemoryStream(bytes);
                    return ms;
                }
            });
        }

        private void Apply(BitmapImage biImage)
        {
            imageApod.Visibility = Visibility.Visible;
            imageApod.Source = biImage;
            explanationTextBlock.Text = mainApod.explanation;
            downloadProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void DownloadButtonClick(object sender, RoutedEventArgs e)
        {
            downloadProgressBar.Visibility = Visibility.Visible;
            downloadProgressBar.IsIndeterminate = true;
            mainApod = await DownloadJson(pictureDatePicker.SelectedDate);
            var source = await GetImage(mainApod);
            if (mainApod.media_type == "image")
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = source;
                bitmap.EndInit();

                Apply(bitmap);
            }
        }
    }
}

#region trash
//imageApod.Visibility = Visibility.Collapsed;
//downloadProgressBas.Visibility = Visibility.Visible;
//mainApod = await DownloadJson();
//var jhg = await GetPicture(mainApod.url);
//imageApod.Source = jhg;
//downloadProgressBas.Visibility = Visibility.Collapsed;
//imageApod.Visibility = Visibility.Visible;



//private Task<ImageSource> GetPicture(string url)
//{
//    return Task.Run(() =>
//    {
//        using (WebClient client = new WebClient())
//        {
//            client.DownloadFile(url, AppDomain.CurrentDomain.BaseDirectory + "apod.jpg");
//        }

//        string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.jpg", SearchOption.AllDirectories);
//        //Image image = new Image
//        //{
//        //    Source = new BitmapImage(new Uri(files[0]))
//        //};
//        return (ImageSource)new BitmapImage(new Uri(files[0]));
//    });

//}
#endregion