using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Decompressor
{
    static class Program
    {
        public static int key = 6;
        public static string files;
        public static string val;
        public static string bits;
        public static string[] sequences;
        public static string content = "";
        public static string start;
        public static string input_file="";

        static void Main(string[] args)
        {

            if (args.Length >0)
                input_file = args[0];

            if (Directory.Exists(@"C:\decompressed"))
                Directory.Delete(@"C:\decompressed", true);
            Directory.CreateDirectory(@"c:\decompressed");

            fetchinfo();
            Decompression();

        }

        public static void fetchinfo()
        {

            //fetch bits, files and sequences
            sequences = new string[1000];
            int index = 0;
            int line = 1;
            string path = @"C:\compressed\infox.dat";
            using (StreamReader sr = new StreamReader(path))
            {
                while (sr.Peek() >= 0)
                {
                    if (line == 2)
                    {

                        bits = sr.ReadLine().Remove(0,12);
                        line++;
                    }

                    else if (line == 3)
                    {
                            files = sr.ReadLine().Remove(0,13);
                        line++;

                        for (int a = 0; a < Convert.ToInt16(files); a++)
                            sequences[a] = "";
                    }
                    else if (line == 1)
                    {
                        sr.ReadLine();
                        line++;
                    }
                    else
                    {
                        if(index>=0 && index <10)
                            sequences[index] += sr.ReadLine().Remove(0,7);
                        if (index >= 10 && index < 100)
                            sequences[index] += sr.ReadLine().Remove(0, 8);
                        if (index >= 100 && index < 1000)
                            sequences[index] += sr.ReadLine().Remove(0, 9);
                        index++;
                    }
                }
            }

        }

        public static void Decompression()
        {
            
            string replace = "";
            int limit;
            string new_address;
            string address;
            char value;
            int indexer = 0;
            //content pick up
            for (int ind = 1; ind <= Convert.ToInt16(files); ind++)
            {
                
                replace = "";

                content = "";

                address = @"C:\compressed\compressed_" + ind + ".txt";
                if (!File.Exists(address))
                    continue;
                using (StreamReader sr = new StreamReader(address))
                {
                    while (sr.Peek() >= 0)
                        content += DeShuffle(sr.ReadLine());
                }


                // find val and start
                #region non compressed
                if (sequences[indexer] == "-"||sequences[indexer]==" -")
                {
                    new_address = @"C:\decompressed\decompressed_" + ind + ".txt";
                    string path = @"C:\decompressed\summary.txt";
                    if (File.Exists(path))
                    {
                        using (StreamWriter swx = File.AppendText(path))
                        {
                            swx.WriteLine("File "+ind+": Not Decompressed");
                        }
                    }
                    else
                    {
                        using (StreamWriter swx = File.CreateText(path))
                        {
                            swx.WriteLine("File " + ind + ": Not Decompressed");
                        }

                    }
                    if (File.Exists(new_address))
                    {
                        using (StreamWriter swx = File.AppendText(new_address))
                        {
                            swx.WriteLine(content);
                        }
                    }
                    else
                    {
                        using (StreamWriter swx = File.CreateText(new_address))
                        {
                            swx.WriteLine(content);
                        }

                    }
                }
                #endregion
                else
                {

                  
                    //get starting val;
                    limit = sequences[indexer].Length;
                    limit = limit * 2;
                    val = Convert.ToString(content[limit]);


                    #region finding start
                    limit = limit / 2;
                    if (val == "1" && limit % 2 == 1)
                        start = "0";
                    else if (val == "1" && limit % 2 == 0)
                        start = "1";
                    else if (val == "0" && limit % 2 == 1)
                        start = "1";
                    else if (val == "0" && limit % 2 == 0)
                        start = "0";
                    else
                        continue;
                    #endregion

                    limit = limit * 2;
                    //comparison and substitution
                    for (int l = 0; l < limit; l = l + 2)
                    {
                        if (content[l] == '0' && content[l + 1] == '0')
                        {
                            if (start == "0")
                            {
                                replace += "00";
                                start = "1";
                            }
                            else
                            {
                                replace += "11";
                                start = "0";
                            }
                        }

                        else if (content[l] == '0' && content[l + 1] == '1')
                        {
                            if (start == "0")
                            {
                                replace += "000";
                                start = "1";
                            }
                            else
                            {
                                replace += "111";
                                start = "0";
                            }
                        }

                        else if (content[l] == '1' && content[l + 1] == '0')
                        {
                            if (start == "0")
                            {
                                replace += "0000";
                                start = "1";
                            }
                            else
                            {
                                replace += "1111";
                                start = "0";
                            }
                        }

                        else if (content[l] == '1' && content[l + 1] == '1')
                        {
                            if (start == "0")
                            {
                                replace += "00000";
                                start = "1";
                            }
                            else
                            {
                                replace += "11111";
                                start = "0";
                            }
                        }

                        else
                            continue;



                    }

                    #region writing into file
                    new_address = @"C:\decompressed\decompressed_" + ind + ".txt";
                    string path = @"C:\decompressed\summary.txt";
                    if (File.Exists(path))
                    {
                        using (StreamWriter swx = File.AppendText(path))
                        {
                            swx.WriteLine("File " + ind + ": Decompressed");
                        }
                    }
                    else
                    {
                        using (StreamWriter swx = File.CreateText(path))
                        {
                            swx.WriteLine("File " + ind + ": Decompressed");
                        }

                    }
                    if (File.Exists(new_address))
                    {
                        using (StreamWriter sw = File.AppendText(new_address))
                        {
                            sw.Write(replace);
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = File.CreateText(new_address))
                        {
                            sw.Write(replace);
                        }

                    }

                    using (StreamWriter swr = File.AppendText(new_address))
                    {
                        for (int a = limit; a < content.Length; a++)
                            swr.Write(content[a]);
                    }
                    #endregion
                
                    }

                indexer++;

            }
        }
        public static int[] GetShuffleExchanges(int size)
        {
            int[] exchanges = new int[size - 1];
            var rand = new Random(key);
            for (int i = size - 1; i > 0; i--)
            {
                int n = rand.Next(i + 1);
                exchanges[size - 1 - i] = n;
            }
            return exchanges;
        }
        public static string DeShuffle(this string shuffled)
        {
            int size = shuffled.Length;
            char[] chars = shuffled.ToArray();
            var exchanges = GetShuffleExchanges(size);
            for (int i = 1; i < size; i++)
            {
                int n = exchanges[size - i - 1];
                char tmp = chars[i];
                chars[i] = chars[n];
                chars[n] = tmp;
            }
            return new string(chars);
        }   
    }


}
