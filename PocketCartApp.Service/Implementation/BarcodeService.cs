using ZXing;
using ZXing.Common;
using SkiaSharp;
using PocketCartApp.Service.Interface;

namespace PocketCartApp.Service.Implementation
{
    public class BarcodeService : IBarcodeService
    {
        public string GenerateEAN13()
        {
            Random random = new Random();
            string first12 = "";

            for (int i = 0; i < 12; i++)
                first12 += random.Next(0, 10);

            int checksum = CalculateChecksum(first12);
            return first12 + checksum;
        }

        private int CalculateChecksum(string code)
        {
            int sum = 0;

            for (int i = 0; i < 12; i++)
            {
                int digit = int.Parse(code[i].ToString());
                sum += (i + 1) % 2 == 0 ? digit * 3 : digit;
            }

            int mod = sum % 10;
            return mod == 0 ? 0 : 10 - mod;
        }

        public string GenerateBarcodeImage(string barcode, string webRootPath)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.EAN_13,
                Options = new EncodingOptions
                {
                    Width = 300,
                    Height = 120,
                    Margin = 10
                }
            };

            var pixelData = writer.Write(barcode);

            var imageInfo = new SKImageInfo(
                pixelData.Width,
                pixelData.Height,
                SKColorType.Rgba8888,
                SKAlphaType.Premul
            );

            using var bitmap = new SKBitmap(imageInfo);

            IntPtr pixelsPtr = bitmap.GetPixels();

            System.Runtime.InteropServices.Marshal.Copy(
                pixelData.Pixels,
                0,
                pixelsPtr,
                pixelData.Pixels.Length
            );

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            string folder = Path.Combine(webRootPath, "barcodes");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = $"{barcode}.png";
            string fullPath = Path.Combine(folder, fileName);

            using var stream = File.OpenWrite(fullPath);
            data.SaveTo(stream);

            return "/barcodes/" + fileName;
        }
    }
}