using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

class ImageCompressor
{
    const string imagePath = @"../Image-Compressor/test.png";

    static void Main(string[] args)
    {
        // Image processing
        using Image<Rgba32> img = Image.Load<Rgba32>(imagePath);

        //2d List of int[]s that store the RGBA of each pixel as an int[]
        List<List<int[]>> pixelValues = new List<List<int[]>>();

        //Parsing the entire image by rows top-down
        for(int y = 0; y < img.Height; y ++) 
        {
            //1d List of int[]s with pixel data, which will then be added to the 2d pixelValues array as a whole row at a time
            List<int[]> pixelRow = new List<int[]>();

            //Parsing through the row left to right
            for(int x = 0; x < img.Width; x ++) 
            {
                Rgba32 pixel = img[y, x];

                //Creates an int[] that stores seperate RGBA of the pixel, which is then added to the 1d pixelRow List
                int[] yxRGBA = new int[] {pixel.R, pixel.G, pixel.B, pixel.A};
                pixelRow.Add(yxRGBA);
            }

            //Adds the entire row of int[]s to the 2d pixelValues as mentioned before in line 20
            pixelValues.Add(pixelRow);
        }

        //4d list where the initial dimension has indeces 0-3 to seperate into RGBA channels
        //Within each channel is a 2d array of int[]s, where the int[] holds refernce values in indeces 16-19 and relative pixel values in 0-15
        List<List<List<int[]>>> pixelsBlocked = new List<List<List<int[]>>>();
        
        //might not need the other loop with this one
        for(int y = 0; y < pixelValues.Count - 8; y += 4)
        {
            for(int x = 0; x < pixelValues[y].Count - 8; x += 4)
            {
                int[] blockValues = new int[20];
                for(int i = y; i < y + 4; i ++)
                {
                    for(int j = x; j < x + 4; j ++)
                    {
                        for(int channel = 0; channel < 4; channel ++)
                        {
                            //Inputs the next value in the current block into the blockValues int[]
                            blockValues.SetValue(pixelValues[i][j][channel], ((i - y) * 4) + j);    //something is messed up and my head hurts
                        }
                    }
                }
                int blockMax = 0;
                int blockMin = 255;
                for(int k = 0; k < 16; k ++)
                {
                    if(blockValues[k] > blockMax)
                        blockMax = blockValues[k];
                    if(blockValues[k] < blockMin)
                        blockMin = blockValues[k];
                }
                int blockMidLow = (((blockMax + blockMin) / 2) + blockMin) / 2;
                int blockMidHigh = (((blockMax + blockMin) / 2) + blockMax) / 2;
                blockValues[16] = blockMin;
                blockValues[17] = blockMidLow;
                blockValues[18] = blockMidHigh;
                blockValues[19] = blockMax;
                Console.WriteLine(String.Join(", ", blockValues));
            }
        }
        //this block just to print and check its doing work right ---------------------------------------------------------------

        /*
        Console.WriteLine("[12, 45]: " + 
        pixelValues[12][45][0] + " " + pixelValues[12][45][1] + " " + pixelValues[12][45][2] + " " + pixelValues[12][45][3]);

        Console.WriteLine("[45, 412]: " + 
        pixelValues[45][12][0] + " " + pixelValues[45][12][1] + " " + pixelValues[45][12][2] + " " + pixelValues[45][12][3]);

        for(int y = 0; y < img.Height; y ++) 
        {
            for(int x = 0; x < img.Width; x ++) 
            {
                Console.WriteLine("[" + y + ", " + x + "]: " + 
                pixelValues[y][x][0] + " " + pixelValues[y][x][1] + " " + pixelValues[y][x][2] + " " + pixelValues[y][x][3]);
            }
        }
        */

        //----------------------------------------------------------------------------------------------------------------------

        
    }
}