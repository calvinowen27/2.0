using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

class ImageCompressor {
        static void Main(string[] args) {
            var path = @"../test.png";
            var img = Image.Load<Rgba32>(path);
            
            var height = img.Height;
            var width = img.Width;

            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    var px = img[x, y];
                    
                    //var px = img[x, y];
                    var rgb = new int[] { px.R, px.G, px.B };
                    Console.WriteLine(rgb[0]);
                }
            }
        }
    }