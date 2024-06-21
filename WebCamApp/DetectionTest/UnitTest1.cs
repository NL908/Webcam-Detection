using OpenCvSharp;
using OpenCvSharp.Extensions;
using WebCamApp.Server;

namespace DetectionTest
{
    public class Tests
    {
        FaceAndEyeDetection detection;
        [SetUp]
        public void Setup()
        {
            detection = new();
        }

        [Test]
        public void TestFace()
        {
            int faceCount = 0;
            Mat srcImage = new Mat(@"Images\face.jpg");
            Mat outImage = srcImage.Clone();
            detection.FaceDetect(srcImage, outImage, out faceCount);

            Cv2.HConcat(srcImage, outImage, outImage);
            Cv2.ImShow("Source Image", outImage);
            int key = Cv2.WaitKey(0);
            srcImage.Release();
            outImage.Release();

            Assert.That(faceCount, Is.EqualTo(1));
        }

        [Test]
        public void TestEyes()
        {
            int eyeCount = 0;
            Mat srcImage = new Mat(@"Images\face.jpg");
            Mat outImage = srcImage.Clone();
            detection.EyeDetect(srcImage, outImage, out eyeCount);

            Cv2.HConcat(srcImage, outImage, outImage);
            Cv2.ImShow("Source Image", outImage);
            int key = Cv2.WaitKey(0);
            srcImage.Release();
            outImage.Release();

            Assert.That(eyeCount, Is.EqualTo(2));
        }

        [Test]
        public void TestFaceAndEye()
        {
            int faceCount = 0;
            int eyeCount = 0;
            Mat srcImage = new Mat(@"Images\face.jpg");
            Mat outImage = srcImage.Clone();

            //var bitmap = outImage.ToBitmap();

            detection.EyeDetect(srcImage, outImage, out faceCount);
            detection.FaceDetect(srcImage, outImage, out eyeCount);

            Cv2.HConcat(srcImage, outImage, outImage);
            Cv2.ImShow("Source Image", outImage);
            int key = Cv2.WaitKey(0);

            srcImage.Release();
            outImage.Release();
        }
    }
}