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

        //this block just to print and check its doing work right ---------------------------------------------------------------
        Console.WriteLine("[12, 45]: " + 
        pixelValues[12][45][0] + " " + pixelValues[12][45][1] + " " + pixelValues[12][45][2] + " " + pixelValues[12][45][3]);
        Console.WriteLine("[45, 12]: " + 
        pixelValues[45][12][0] + " " + pixelValues[45][12][1] + " " + pixelValues[45][12][2] + " " + pixelValues[45][12][3]);
        for(int y = 0; y < pixelValues.Count; y ++) 
        {
            for(int x = 0; x < pixelValues[y].Count; x ++) 
            {
                Console.WriteLine("[" + y + ", " + x + "]: " + 
                pixelValues[y][x][0] + " " + pixelValues[y][x][1] + " " + pixelValues[y][x][2] + " " + pixelValues[y][x][3]);
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------
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