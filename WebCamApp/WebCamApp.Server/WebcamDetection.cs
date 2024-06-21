using OpenCvSharp;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebCamApp.Server;

var builder = WebApplication.CreateBuilder(args);

//var port = 8080;
//builder.WebHost.UseUrls("http://localhost:" + port);
builder.WebHost.UseUrls("http://localhost:8080");

var app = builder.Build();
app.UseWebSockets();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        using (VideoCapture capture = new(0))
        {
            while (true)
            {
                // Get the camera mat
                if (capture.IsOpened())
                using (Mat srcImage = capture.RetrieveMat())
                    if (!srcImage.Empty())
                        // Clone the source image to the output image
                        using (Mat afterImage = srcImage.Clone())
                        {
                            FaceAndEyeDetection detection = new();
                            // Apply face & eye detection and draw results on the output image
                            detection.FaceDetect(srcImage, afterImage, out _);
                            detection.EyeDetect(srcImage, afterImage, out _);

                            // Store the source image and result into a class for json parsing
                            DetectionData data = detection.MatsToBase64(srcImage, afterImage);
                            string jsonMessage = JsonSerializer.Serialize<DetectionData>(data);

                            // Send the string json data through websocket
                            byte[] bytes = Encoding.UTF8.GetBytes(jsonMessage);
                            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                            if (webSocket.State == WebSocketState.Open)
                            {
                                await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                            else if (webSocket.State == WebSocketState.Closed || webSocket.State == WebSocketState.Aborted)
                            {
                                break;
                            }
                        }
                // Wait for 0.1 second between each frame
                Thread.Sleep(100);
            }
        }
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});

await app.RunAsync();