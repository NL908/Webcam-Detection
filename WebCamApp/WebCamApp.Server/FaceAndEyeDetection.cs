using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using OpenCvSharp;

namespace WebCamApp.Server
{
    public class DetectionData
    {
        public required string srcImageBase64Data { get; set; }
        public required string resImageBase64Data { get; set; }
    }

    public class FaceAndEyeDetection
    {
        private CascadeClassifier _faceCascade;
        private CascadeClassifier _eyeCascade;
        private Scalar _faceDetectColor;
        private Scalar _eyeDetectColor;

        public FaceAndEyeDetection()
        {
            _faceCascade = new CascadeClassifier(@"data\haarcascade_frontalface_default.xml");
            _eyeCascade = new CascadeClassifier(@"data\haarcascade_eye.xml");
            _faceDetectColor = Scalar.FromRgb(255, 0, 0);
            _eyeDetectColor = Scalar.FromRgb(0, 255, 0);
        }

        public void FaceDetect(Mat srcImage, Mat outImage, out int faceCount)
        {
            Detect(_faceCascade, 60, srcImage, _faceDetectColor, outImage, out faceCount);
        }

        public void EyeDetect(Mat srcImage, Mat outImage, out int eyeCount)
        {
            Detect(_eyeCascade, 60, srcImage, _eyeDetectColor, outImage, out eyeCount);
        }

        private void Detect(CascadeClassifier clascade, int minSize,  Mat srcImage, Scalar color, Mat outImage, out int count)
        {
            using (var greyImage = new Mat())
            {
                Cv2.CvtColor(srcImage, greyImage, ColorConversionCodes.RGBA2GRAY);
                Cv2.EqualizeHist(greyImage, greyImage);

                var faces = clascade.DetectMultiScale(
                    image: greyImage,
                    minSize: new Size(minSize, minSize)
                    );

                count = faces.Length;

                foreach (var faceRect in faces)
                {
                    var detectedFaceImage = new Mat(srcImage, faceRect);
                    Cv2.Rectangle(outImage, faceRect, color, 3);
                }
            }
        }

        public DetectionData MatsToBase64(Mat srcImage, Mat resImage)
        {
            byte[] srcImageBuffer;
            Cv2.ImEncode(".png", srcImage, out srcImageBuffer);
            byte[] resImageBuffer;
            Cv2.ImEncode(".png", resImage, out resImageBuffer);

            DetectionData data = new DetectionData
            {
                srcImageBase64Data = System.Convert.ToBase64String(srcImageBuffer),
                resImageBase64Data = System.Convert.ToBase64String(resImageBuffer)
            };

            return data;
        }

        public Mat Base64ToMat(string imageURL)
        {
            string encodedImage = imageURL.Split(',')[1];
            byte[] bytes = System.Convert.FromBase64String(encodedImage);
            return Cv2.ImDecode(bytes, ImreadModes.Color);
        }
    }
}
