using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace RandomHoppers
{
    class Hopper
    {
        // Declaring instance of Random outside of the Hop() function, because:
        // otherwise when calling Hop to create a new Random in close succession, they'll have the same system time as seed
        // and thus will not be random.
        
        static Random rand = new Random();

        protected static double GetNextDouble()
        {
            return rand.NextDouble();
        }

        
        static void Main(string[] args)
        {
            //Stopwatch sw = new Stopwatch();

            //sw.Start();

            //ToCSV(Hop,"Hop",10000000);

            //sw.Stop();

            //Console.WriteLine("10000000 iterations took: " + sw.ElapsedMilliseconds / 1000 + " seconds");

            Hop(50, 10);

            // TODO: neaten timing

            // TODO: make this properly OO

            // TODO: make this a UI

            // TODO: setup args so we can multi-test or single test or whatever i guess

            //double average_hops = hops.Average();

            // take average of array

            //Console.WriteLine(average_hops);
                                 
            return;
        }

        static void ToCSV(Func<double> method, string method_str, int iterations)
        {
            // Method to run another method a certain number of times, and spit out a CSV file based on whatever
            // the method returns.
            // Use it to create statistics on, well, pretty much anything!

            // Takes arguments method (must return a covariant of double. i.e. floats and ints are ok), that
            // method as a string (this is purely for naming files), and the number of iterations to run it for.

            // CSV file is excel compatible; starts off as a single column file until it hits excel 2013's row limit (~1m)
            
            
            // Create a file to list our number of stat
            // Doing this as a .csv so statistics in excel are easy to manage
            
            string timestamp = DateTime.Now.ToString("yyyy-mm-d_hh-mm-ss");
            
            // See if M:\ exists and save there (i.e. if we're at uni), otherwise just save on the D:\

            string savepath = "D:\\Coding\\PHYS2320_Computing_2\\RandomHoppers\\";

            try
            {
               if (Directory.Exists("M:\\PHYS2320_Computing_2\\RandomHoppers"))
               {
                   savepath = "M:\\PHYS2320_Computing_2\\RandomHoppers\\"; 
               }
            }
            catch (Exception)
            {
                // fail silently
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter(savepath + method_str + "_" + iterations + "_" + timestamp + ".csv");
            
            int columns;

            if (iterations > 1000000)
            {
                columns = iterations / 1000000;
            }
            else columns = 1;
            // make it fit excel's 1m row limit

            // Create an array for the statistic; using doubles incase we want to take an average of it

            double[] stat = new double[iterations];
            string line = "";
            for (int i = 0; i < iterations; i++)
            {
                stat[i] = method();
                // call the method specified when calling this method (ToCSV)
                if ((i+1) % columns == 0)
                {
                    line = line + stat[i].ToString();  // since writing line, dont need final ","
                    file.WriteLine(line);
                    line = "";
                }
                else line = line + stat[i].ToString() + ", ";
                // if method returns int should implicitly cast to double
            }

            if (line != "")
                file.WriteLine(line);

            // if it ended on i = 999 and line is not blank or whatever write the residual bit
            file.Close();
            // close file

            // TODO: add averages here
        }

        static double Hop(int length, int maxtime) // todo add args for L, T, maxT
        {
            double time = 0; // time counter

            int[] line = new int[length];

            PrintCurrentState(time, length, line);

            Console.WriteLine("Starting hopper with parameters: \tlength = " + length + "\t maxtime = " + maxtime);

            // start hopper 1 off

            line[0] = 1;

            while (time <= maxtime)
            {
                time++;
                // start of a run, increase time by 1 unit

                for (int i = length-1 ; i == 0; i--) // work backwards along the line
                {
                    Console.WriteLine(i + "  " + line[i]);
                    if (line[i] == 1)
                    {
                        Console.WriteLine("fart");
                        // run hop routine
                        if (GetNextDouble() > 0.5) // if coinflip works, hop
                        {
                            if (i == length-1)
                            {
                                //take it off the end of the line
                                line[i] = 0;
                            }
                            else // normal hop
                            {
                                if (line[i + 1] == 0) // if next space is free, hop!
                                {
                                    line[i] = 0;
                                    line[i + 1] = 1;
                                    // hop!
                                }
                            }
                        }
                    }
                    else
                    {
                        // do nothing and move onto next hopper.
                    }
                }

                PrintCurrentState(time, length, line);

            }
            // shouldn't reach here, so return -1
            return -1;
        }

        static void PrintCurrentState(double time, int length, int[] line)
        {

            if (time == 0)
                Console.Write("Start:\t");
            else
            {
                Console.Write(time + "\t");
            }
            for (int i = 0; i < length; i++)
            {
                if (line[i] == 1)
                    Console.Write("X");
                // print an X if a hopper exists at this location
                else
                {
                    Console.Write("_");
                }
                // print _ in empty location
            }
            Console.WriteLine();
        }
    }
}
