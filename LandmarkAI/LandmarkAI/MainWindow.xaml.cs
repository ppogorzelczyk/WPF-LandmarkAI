using LandmarkAI.Classes;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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

namespace LandmarkAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            //filter only images
            dialog.Filter = "Image files (*.png; *.jpg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";
            //set initial directory as My Pictures directory
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (dialog.ShowDialog() == true)
            {
                //get filePathName
                string fileName = dialog.FileName;
                //set source of image as filePathName
                selectedImage.Source = new BitmapImage(new Uri(fileName));

                MakePredictionAsync(fileName);
            }
        }

        private async void MakePredictionAsync(string fileName)
        {
            string url = "https://landmark.cognitiveservices.azure.com/customvision/v3.0/Prediction/e4826a73-2576-412a-bf41-f08faee80df9/classify/iterations/Iteration1/image";
            string predictionKey = "c47f7573563146ba8ee7207875be9f96";
            string contentType = "application/octet-stream";
            var file = File.ReadAllBytes(fileName);

            using (HttpClient client = new HttpClient())
            {
                //set specific header for our API (not always needed)
                client.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);

                using (var content = new ByteArrayContent(file))
                {
                    //set content type
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    //get response from API
                    var response = await client.PostAsync(url, content);
                    //read as jsonString
                    var responseString = await response.Content.ReadAsStringAsync();
                    //get deserialized object
                    List<Prediction> predictions = JsonConvert.DeserializeObject<CustomVision>(responseString).Predictions;

                    predictionsListView.ItemsSource = predictions;
                }
            }
        }
    }
}
