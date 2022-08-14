using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Huffman;

class ImageCompressor
{
    const string imagePath = @"../Image-Compressor/test.png";

    static void Main(string[] args)
    {
        // Image processing
        using Image<Rgba32> img = Image.Load<Rgba32>(imagePath);
        List<List<List<int[]>>> blockedPixels = blockPixels(img);


        Console.WriteLine(blockedPixels.Count);
        for(int y = 0; y < blockedPixels.Count; y ++)
        {
            for(int x = 0; x < blockedPixels[y].Count; x ++)
            {
                for(int i = 0; i < blockedPixels[y][x].Count; i ++)
                {
                    Console.WriteLine($"In block {x}, {y}: {String.Join(", ",blockedPixels[y][x][i])}");
                }
            }
        }
        /*
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

            //Adding "null values" at the end of the row if needed to make the row length an even division of 4---------
            while(pixelRow.Count % 4 != 0)
            {
                Console.WriteLine("Added null value to row " + y);
                pixelRow.Add(new int[] {-1, -1, -1, -1});
            }
            //-----------------------------------------------------------------------------------------------------------

            //Adds the entire row of int[]s to the 2d pixelValues as mentioned before in line 20
            pixelValues.Add(pixelRow);
        }

        //Adding rows full of "null values" until the image size is a perfect multiple of 4s in both dirs ------
        if(pixelValues.Count % 4 != 0)
        {
            List<int[]> nullRow = new List<int[]>();
            for(int i = 0; i < pixelValues[0].Count; i ++)
            {
                nullRow.Add(new int[] {-1, -1, -1, -1});
            }
            while(pixelValues.Count % 4 != 0)
            {
                Console.WriteLine("Added null row");
                pixelValues.Add(nullRow);
            }
        }
        //-------------------------------------------------------------------------------------------------------

        //this block just to print and check pixelValues is being created properly ----------------------------------------------
        Console.WriteLine("[12, 45]: " + String.Join(" ", pixelValues[12][45]));
        Console.WriteLine("[45, 12]: " + String.Join(" ", pixelValues[45][12]));
        for(int y = 0; y < pixelValues.Count; y ++) 
        {
            for(int x = 0; x < pixelValues[y].Count; x ++) 
            {
                Console.WriteLine("[" + y + ", " + x + "]: " + String.Join(" ", pixelValues[y][x]));
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------
        */

        /*int[] data = new int[500];
        Random rand = new Random();
        for(int i = 0; i < data.Length; i++) {
            data[i] = rand.Next(0, 26);
        }*/

        //var huff = new HuffmanTree(new int[] {0, 0, 0, 0, 0, 0, 0, 2, 1, 3, 3, 3, 3, 3, 3});
    }

    
    //need to implement either 1: another for and another dimension in blockedPixels loop to do channels in this method
    //or 2: have separate blockPixels 3d Lists in main for each channel
    static List<List<List<int[]>>> blockPixels(Image<Rgba32> img)
    {
        List<List<List<int[]>>> pixelBlocks = new List<List<List<int[]>>>();

        for(int y = 0; y < img.Height; y += 4)
        {
            List<List<int[]>> pixelBlockRow = new List<List<int[]>>();
            for(int x = 0; x < img.Width; x += 4)       //x,y is the index of the top left of the block in the image
            {                                           //bx, by is the index of each pixel within the block
                List<int[]> pixelBlock = new List<int[]>();
                for(int by = 0; by < 4; by ++)
                {
                    for(int bx = 0; bx < 4; bx ++)
                    {
                        if(y + by > img.Height || x + bx > img.Width)
                        {
                            pixelBlock.Add(new int[] {-1, -1, -1, -1});
                        }
                        else
                        {
                            Rgba32 pixel = img[x, y];
                            int[] pixelRGBA = new int[] {pixel.R, pixel.G, pixel.B, pixel.A};
                            pixelBlock.Add(pixelRGBA);
                        }
                    }
                }
                pixelBlockRow.Add(pixelBlock);
            }
            pixelBlocks.Add(pixelBlockRow);
        }
        return pixelBlocks;
    }
   
    static List<List<List<int[]>>> blockPixels(List<List<int[]>> pixelValues)
    {
        List<List<List<int[]>>> blockedPixels = new List<List<List<int[]>>>();
        
        for(int y = 0; y < pixelValues.Count; y ++)
        {
            for(int x = 0; x < pixelValues[y].Count; x ++)
            {
                //pixel from pixelValues at y, x goes into block (int[]) blockedPixels[y / 4][x / 4]
            }
        }
        
        
        
        
        return blockedPixels;
    }
    
    //inputted block should begin with the top left [y, x] coordinate in indeces 0, 1, followed 
    //return will house top-left [y, x] coordinate in 0-1, binary pixel data in 2-17, channel data mean in 18, channel data standard deviation in 19
    static int[] processBlockChannel16(List<int[]> blockData, int channel)
    {        
        //Math ---------------------------------------------------------------------------------------------
        //Finding mean
        int totalPixelValue = 0;
        for(int i = 2; i < blockData[channel].Length; i ++)
        {
            totalPixelValue += blockData[channel][i];
        }
        double blockMean = totalPixelValue / (blockData[channel].Length - 2);
        //Finding standard dev ------------------------------------------------------
        double standardDevTotal = 0;
        for(int i = 2; i < blockData[channel].Length; i ++)
        {
            standardDevTotal += (blockData[channel][i] - blockMean) * (blockData[channel][i] - blockMean);
        }
        double standardDev = Math.Sqrt(standardDevTotal / (blockData[channel].Length - 2));
        //--------------------------------------------------------------------------------------------------
        
        int[] compressedBlock = new int[20];
        compressedBlock[0] = blockData[channel][0];
        compressedBlock[1] = blockData[channel][1];
        for(int i = 2; i < blockData[channel].Length; i ++)
        {
            if(blockData[channel][i] < blockMean)
                compressedBlock[i] = 0;
            else
                compressedBlock[i] = 1;
        }
        compressedBlock[18] = Convert.ToInt32(Math.Round(blockMean));
        compressedBlock[19] = Convert.ToInt32(Math.Round(standardDev));
        return compressedBlock;
    }
}