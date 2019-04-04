using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GPUImageResizer
{
    public class GPUImageResizer
    {
        public void ResizeImage(string imageName, string pathToImages, int width = 400, int height = 299)
        {
            var newFileName = imageName + "_AfterResize.jpg";
            var imageFileNamePath = Path.Combine(pathToImages, imageName);
            var newImageFilePathName = Path.Combine(pathToImages, newFileName);

            using (var image = new Bitmap(imageFileNamePath))
            {
                var img = new Image<Bgr, Byte>(image);
                var newSize = new Size(width, height);
                if (new CudaDeviceInfo().IsCompatible)
                {
                    var cudaImg = new CudaImage<Bgr, Byte>(img);
                    cudaImg.Resize(newSize, Inter.Linear).Bitmap.Save(newFileName, ImageFormat.Jpeg);
                }
                else
                {
                    img.Resize(400, 299, Emgu.CV.CvEnum.Inter.Nearest).ToBitmap().Save(newFileName, ImageFormat.Jpeg);
                }
            }
        }
    }
}
