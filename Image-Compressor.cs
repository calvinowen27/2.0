using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

class ImageCompressor {
    const string imagePath = @"../Image-Compressor/test.png";

    static void Main(string[] args) {
        // Image processing
        using Image<Rgba32> img = Image.Load<Rgba32>(imagePath);
        for(int y = 0; y < img.Height; y++) {
            for(int x = 0; x < img.Width; x++) {
                Rgba32 pixel = img[x, y];
                int[] rgba = new int[] { pixel.R, pixel.G, pixel.B, pixel.A };
                Console.WriteLine(rgba[0]);
            }
        }
    }
}