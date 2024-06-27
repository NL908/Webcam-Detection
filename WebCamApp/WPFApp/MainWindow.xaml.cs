using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Microsoft.Web.WebView2.Core;
using System.Text.Json;
using OpenCvSharp;
using WebCamApp.Server;
using OpenCvSharp.Extensions;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private FaceAndEyeDetection detection = new ();
        public MainWindow()
        {
            InitializeComponent();
            InitializeWebview();
        }
        class AnswerSDP
        {
            public string? type { get; set; }
            public string? sdp { get; set; }
        }

        private async void InitializeWebview()
        {
            var environment = await CoreWebView2Environment.CreateAsync();
            await wpfWebview.EnsureCoreWebView2Async(environment);

            string htmlPath = System.IO.Path.GetFullPath(@"Assets\webpage.html");
            wpfWebview.Source = new Uri(htmlPath);
            /*
            string htmlContent = @"
                <body>
                    <script type='text/javascript'>
                        let peerConnection = new RTCPeerConnection();
                        let remoteStream;
                        let offer;
                        let answer;
                        let imageCapture;
                        let canvas;

                        let init = async () => {
                            remoteStream = new MediaStream();

                            peerConnection.ontrack = (event) => {
                                event.streams[0].getTracks().forEach((track) => {
                                    remoteStream.addTrack(track);
                                });
                            };

                            peerConnection.onconnectionstatechange = () => {
                                if (peerConnection.connectionState === 'connected') {
                                    startCaptureFrame();
                                }
                            };
                        }

                        let createAnswer = async (message) => {
                            offer = message

                            peerConnection.onicecandidate = async (event) => {
                                //Event that fires off when a new answer ICE candidate is created
                                if (event.candidate) {
                                    window.chrome.webview.postMessage(peerConnection.localDescription);
                                }
                            };

                            await peerConnection.setRemoteDescription(offer);

                            answer = await peerConnection.createAnswer();
                            await peerConnection.setLocalDescription(answer);
                        }

                        let startCaptureFrame = async () => {
                            const tracks = remoteStream.getVideoTracks()
                            imageCapture = new ImageCapture(tracks[0])
                            canvas = document.createElement('canvas');
                            setInterval(captureFrame, 33)
                        }

                        let captureFrame = async () => {
                            imageCapture.grabFrame().then((imageBitmap) => {
                                console.log(""Grabbed frame:"", imageBitmap);
                                canvas.width = imageBitmap.width;
                                canvas.height = imageBitmap.height;
                                canvas.getContext(""2d"").drawImage(imageBitmap, 0, 0);
                                const data = {}
                                data[""image""] = canvas.toDataURL('image/png');
                                window.chrome.webview.postMessage(data);
                            })

                        }

                        init();
                    </script>
                </body>";

            wpfWebview.NavigateToString(htmlContent);
            */
            wpfWebview.WebMessageReceived += WpfWebview_WebMessageReceived;

            createAnswerBtn.Click += CreateAnswerBtn_Click;
        }

        private void WpfWebview_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string message = e.WebMessageAsJson;
            if (message != null)
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(message);
                if (data != null && data.ContainsKey("type") && data["type"] == "answer")
                {
                    Clipboard.SetText(message);
                }
                else if (data != null && data.ContainsKey("image"))
                {
                    DetectionProcessing(data);
                }
                else if (data != null && data.ContainsKey("message"))
                {
                    MessageBox.Show(data["message"]);
                }
            }
        }

        private void DetectionProcessing(Dictionary<string, string> data)
        {
            Mat sourceImage = detection.Base64ToMat(data["image"]);
            using (Mat outImage = sourceImage.Clone())
            {
                detection.FaceDetect(sourceImage, outImage, out _);
                detection.EyeDetect(sourceImage, outImage, out _);
                
                BitmapImage image = BitmapToImageSource(outImage.ToBitmap());
                videoImage.Source = image;
            }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private async void CreateAnswerBtn_Click(object sender, RoutedEventArgs e)
        {
            string message = Clipboard.GetText(TextDataFormat.Text);
            await wpfWebview.ExecuteScriptAsync($"createAnswer({message})");
        }
    }
}