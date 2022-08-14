using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Huffman;

class ImageCompressor
{
    const string imagePath = @"../Image-Compressor/test12.png";

    static void Main(string[] args)
    {
        // Image processing
        using Image<Rgba32> img = Image.Load<Rgba32>(imagePath);
        List<List<List<int?[]>>> blockedPixels = blockPixels(img);

        //Code used to test the functionality of blockPixels()
        /*
        for(int y = 0; y < blockedPixels.Count; y ++)
        {
            for(int x = 0; x < blockedPixels[y].Count; x ++)
            {
                for(int i = 0; i < blockedPixels[y][x].Count; i ++)
                {
                    Console.WriteLine($"Block {x}, {y}[{i}]: {String.Join(", ",blockedPixels[x][y][i])}");
                }
            }
        }
        */

        List<List<List<int?[]>>> compressedBlocks = compressBlocks(blockedPixels);


        Console.WriteLine(compressedBlocks.Count);
        for(int y = 0; y < compressedBlocks.Count; y ++)
        {
            for(int x = 0; x < compressedBlocks[y].Count; x ++)
            {
                for(int channel = 0; channel < 4; channel ++)
                {
                    Console.WriteLine($"Block {x}, {y}[{channel}]: {String.Join(", ",compressedBlocks[x][y][channel])}");
                }
            }
        }

        /*int[] data = new int[500];
        Random rand = new Random();
        for(int i = 0; i < data.Length; i++) {
            data[i] = rand.Next(0, 26);
        }*/

        //var huff = new HuffmanTree(new int[] {0, 0, 0, 0, 0, 0, 0, 2, 1, 3, 3, 3, 3, 3, 3});
    }

    
    /*
    Uses and returns a 3d list, where each entry is an int[] that contains the different RGBA channel values
    List<List<List<int[]>>> - A list of rows of blocks of int[]
         List<List<int[]>> - A row of blocks of int[]
              List<int[]> - A "block" of 16 int[] arrays
                   int[] - A set of 4 integers with R, G, B, and A values for a pixel
    */
    static List<List<List<int?[]>>> blockPixels(Image<Rgba32> img)
    {
        List<List<List<int?[]>>> pixelBlocks = new List<List<List<int?[]>>>();

        /*
        Parses through every 4 pixels in the image by adding 4 to the index
        Within each loop that passes 4 pixels is another loop that parses each pixel within the next 4 pixels
        Within the second loop where it looks at an individual pixel, it adds the pixel data to a block, which is then added to a row, which is then added to the larger List
        */
        for(int y = 0; y < img.Height; y += 4)
        {
            List<List<int?[]>> pixelBlockRow = new List<List<int?[]>>();
            
            for(int x = 0; x < img.Width; x += 4)       //x,y is the index of the top left of the block in the image
            {                                           //bx, by is the index of each pixel within the block
                List<int?[]> pixelBlock = new List<int?[]>();
                
                for(int by = 0; by < 4; by ++)
                {
                    for(int bx = 0; bx < 4; bx ++)
                    {
                        //If the loop starts looking at indeces that are larger than the image dims since the dimension wasn't a perfect multiple of 4
                        //Then a pixel value of -1s is added to the block instead of a real pixel value
                        if(y + by >= img.Height || x + bx >= img.Width)
                        {
                            pixelBlock.Add(new int?[] {null, null, null, null});
                        }
                        else
                        {
                            Rgba32 pixel = img[x, y];
                            int?[] pixelRGBA = new int?[] {pixel.R, pixel.G, pixel.B, pixel.A};
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

    /*
    This will return a List of blocks but in a different format than they were inputted as created from the blockPixels() method
    This method will return blocks in the following format:
    List<List<List<int[]>>> - A list of rows of blocks
         List<List<int[]>> - A row of blocks
              List<int[]> - A block that contains a list of different channels indexed 0-3 for RGBA
                   int[] - A sequential list of binary integers followed by 2 stored 8-bit values for decoding
    */
    static List<List<List<int?[]>>> compressBlocks(List<List<List<int?[]>>> blocks)
    {
        List<List<List<int?[]>>> compressedBlocks = new List<List<List<int?[]>>>();
        for(int y = 0; y < blocks.Count; y ++)
        {
            List<List<int?[]>> compressedBlockRow = new List<List<int?[]>>();
            for(int x = 0; x < blocks[y].Count; x ++)
            {
                List<int?[]> compressedBlock = new List<int?[]>();
                for(int channel = 0; channel < 4; channel ++)
                {
                    compressedBlock.Add(processBlockChannel(blocks[y][x], channel));
                }
                compressedBlockRow.Add(compressedBlock);
            }
            compressedBlocks.Add(compressedBlockRow);
        }
        return compressedBlocks;
    }
    
    static int?[] processBlockChannel(List<int?[]> blockData, int channel)
    {        
        //Math ---------------------------------------------------------------------------------------------
        //Finding mean
        int? totalPixelValue = 0;
        for(int i = 0; i < blockData[channel].Length; i ++)
        {
            totalPixelValue += blockData[channel][i];
        }
        double blockMean = Convert.ToDouble(totalPixelValue / (blockData[channel].Length));
        //Finding standard dev ------------------------------------------------------
        double standardDevTotal = 0;
        for(int i = 0; i < blockData[channel].Length; i ++)
        {
            standardDevTotal += Convert.ToDouble((blockData[channel][i] - blockMean) * (blockData[channel][i] - blockMean));
        }
        double standardDev = Math.Sqrt(standardDevTotal / (blockData[channel].Length));
        //--------------------------------------------------------------------------------------------------
        
        int?[] compressedBlock = new int?[18];
        for(int i = 0; i < blockData[channel].Length; i ++)
        {
            if(blockData[channel][i] < blockMean)
                compressedBlock[i] = 0;
            else
                compressedBlock[i] = 1;
        }
        compressedBlock[16] = Convert.ToInt16(Math.Round(blockMean));
        compressedBlock[17] = Convert.ToInt16(Math.Round(standardDev));
        return compressedBlock;
    }
}