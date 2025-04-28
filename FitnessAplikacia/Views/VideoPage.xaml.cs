using Microsoft.Maui.Controls;

namespace FitnessAplikacia.Views
{
    [QueryProperty(nameof(VideoUrl), "videoUrl")]
    public partial class VideoPage : ContentPage
    {
        private string _videoUrl;
        public string VideoUrl
        {
            get => _videoUrl;
            set
            {
                _videoUrl = value;
                LoadVideo();
            }
        }

        public VideoPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private void LoadVideo()
        {
            if (!string.IsNullOrEmpty(VideoUrl))
            {
                YouTubeWebView.Source = new UrlWebViewSource { Url = VideoUrl };
            }
            else
            {
                DisplayAlert("Error", "No video available", "OK");
            }
        }
    }
}
