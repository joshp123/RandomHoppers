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

            //for (double probability = 0; probability <= 1; probability+=0.1)
            //{
            //    Hop(10, 50, probability);
            //}

            Tuple<double,double> average_and_stdev = Hop(10, 100, 0.5);
            double average = average_and_stdev.Item1;
            double stdev = average_and_stdev.Item2;

            Console.WriteLine("Average = " + average);
            Console.WriteLine("Standard deviation = " + stdev);
            // TODO: make this properly OO

            // TODO: make this a UI

            // TODO: setup args so we can multi-test or single test or whatever i guess
                                 
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

        }

        static void ToCSV(Func<int,int,int,Tuple<double,double>> method, int arg1, int arg2, int arg3, string method_str, int iterations)
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
                stat[i] = method(arg1, arg2, arg3); // sort columns in this overloaded method
                // call the method specified when calling this method (ToCSV)
                if ((i + 1) % columns == 0)
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
            // dont forget to close the file!

            return;

        }

        static Tuple<double,double> Hop(int length, int maxtime, double probability)
        {
            // returns average number of hoppers per line and the standard deviation as a tuple of doubles

            int time = 0; // time counter
            int[] line = new int[length]; // initiate the line array

            // PrintCurrentState(time, length, line); // testing line only

            // Console.WriteLine("Starting hopper with parameters: \tlength = " + length + "\t maxtime = " + maxtime); // testing line only

            int[] hopper_count = new int[maxtime]; // initiate array for getting average of hoppers

            // start hopper 1 off
            line[0] = 1;

            while (time < maxtime)
            {
                time++;
                // start of a run, increase time by 1 unit
                int current_hoppers = 0;               
                for (int i = length-1; i >= 0; i--) // work backwards along the line
                {
                    if (line[i] == 1)
                    {
                        current_hoppers++;
                        // run hop routine
                        if (probability > GetNextDouble()) // if coinflip works, hop
                        {
                            if (i == length-1)
                            {
                                // take it off the end of the line if we're there
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

                    if (line[0] == 0)
                    {
                        line[0] = 1;
                        current_hoppers++;
                        // add a new hopper if the first position is free
                    }

                }
                hopper_count[time - 1] = current_hoppers;

                // PrintCurrentState(time, length, line); // (only used for visually testing)

            }
            
            double average = hopper_count.Average();
            double sumOfSquaresOfDifferences = hopper_count.Select(val => (val - average) * (val - average)).Sum();
            double stdev = Math.Sqrt(sumOfSquaresOfDifferences / hopper_count.Length); 

            return new Tuple<double,double>(average,stdev);
            
        }

        static void PrintCurrentState(double time, int length, int[] line)
        {
            // Method to visually represent the line. It will break if the length is over the display width of the console but that's an edge case that's not worth fixing
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
