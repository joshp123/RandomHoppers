﻿using System;
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
            JaggedArrayToConsole(SingleHopLoop(1, 7, 50, 0.5, 0.5));
            JaggedArrayToConsole(LoopOverProbabilities(0.05, 1.00, 0.05, 10000));
            
            // TODO: document all functions properyl. lmao this wont be fun
                                
            return;
        }

        private static void JaggedArrayToConsole(string[][] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array[i].Length; j++)
                {
                    Console.Write(array[i][j] + " \t");
                }
                Console.WriteLine();
            }
        }

        private static string[][] LoopOverPowersOfTen(int min, int max, double interval, double probability)
        {
            int loops = Convert.ToInt32((max - min) / interval);
            string[][] retval  = new string[loops + 2][];
            int iteration = 1;
            retval[0] = new string[] { "Running a loop over iterations between 10^" + min + " + 10^" + max + " with probability " + probability };
            retval[1] = new string[] { "Iterations" , "Average hoppers on line", "Standard deviation" , "Average travel time" , "Time taken to Calculate (ms)" };

            // this code handles creating the array to return
            // basically it returns a big ass table

            for (double i = min; i < (max + interval); i += interval)
            {
                iteration++;
                if (!(0 <= probability && probability <= 1))
                {
                    Console.WriteLine("Probability is not valid (between 0 and 1). Stopping");
                    throw new ArgumentOutOfRangeException("Probability out of range");
                }

                // should probably re-do this with exceptions but that would probably just complicate the codebase tbh
                
                Console.WriteLine("Testing a MultiHop of line length 50, probability " + probability + ", for 10^" + i + " iterations");

                if (i >= 9)
                // dont' do more than 100m iterations becaue that makes an object larger than 2gb and you get an OutOfMemory exception
                // creating an array of lenght 10^9 doesn't work in C either without ~workarounds~ so WONTFIX
                {
                    Console.WriteLine("Iteration size is too large and will overflow the memory. Stopping");
                    break;
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();

                Tuple<double, double, double> stats = MultiHop(50, Convert.ToInt32(Math.Pow(10, i)), probability);
                double average = stats.Item1;
                double stdev = stats.Item2;
                double average_travel_time = stats.Item3;

                sw.Stop();

                retval[iteration] = new string[] { Math.Pow(10, i).ToString(), average.ToString(), stdev.ToString(), average_travel_time.ToString(), sw.ElapsedMilliseconds.ToString() };
            }
            return retval;
        }

        private static string[][] LoopOverProbabilities(double min, double max, double interval, int iterations)
        {
            if (min < 0  || max > 1)
            {
                Console.WriteLine("Probability out of range. Terminating");
                throw new ArgumentOutOfRangeException("Probability out of range");
            }
            
            int loops = Convert.ToInt32((max - min) / interval) + 1;
            string[][] retval = new string[loops + 2][];
            int iteration = 1;
            retval[0] = new string[] { "Running a loop over probabilities between " + min + " + " + max + " over " + iterations + " iterations" };
            retval[1] = new string[] { "Probability", "Average hoppers on line", "Standard deviation", "Average travel time", "Time taken to Calculate (ms)" };

            for (double i = min; i < (max + interval); i += interval)
            {
                // Console.WriteLine("Testing a MultiHop of line length 50, probability " + i + ", for " + iterations + " iterations"); // line for testing purposes only
                iteration++; 
                // this is the array counter variable counting which loop we are on
                // not to be confused with the length of time we are calling the multihop for. which is technically maxtime. i could change this but effort

                if (iterations >= 100000000)
                // dont' do more than 100m iterations becaue that makes an object larger than 2gb and you get an OutOfMemory exception
                // creating an array of lenght 10^9 doesn't work in C either without ~workarounds~ so WONTFIX
                {
                    Console.WriteLine("Iteration size is too large and will overflow the memory. Stopping");
                    break;
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();

                Tuple<double, double, double> stats = MultiHop(50, iterations, i);
                double average = stats.Item1;
                double stdev = stats.Item2;
                double average_travel_time = stats.Item3;

                sw.Stop();

                retval[iteration] = new string[] { i.ToString(), average.ToString(), stdev.ToString(), average_travel_time.ToString(), sw.ElapsedMilliseconds.ToString() };
            }
            return retval;
        }

        private static string[][] SingleHopLoop(int min, int max, int length, double interval, double probability)
        {
            // this should loop over powers of 10 to illustrate how the averages converge
            int loops = Convert.ToInt32((max - min) / interval) + 1;
            string[][] retval = new string[loops + 2][];
            retval[0] = new string[] { "Running a loop on a single hopper to illustrate how repeats converge as iterations increase. Line length = " + "length" + " ; Probability = " + probability };
            retval[1] = new string[] { "Iterations" , "Average travel time", "Standard Deviation" , "Time Taken (ms)" };
            // construct the array to return

            int iteration = 0;

            for (double i = min; i < (max + interval); i+= interval)
			{
			    iteration++;
                // use iteration variable to track instead of i because it's a little bit more lightweight (as i is a power of ten)

                int repeats = Convert.ToInt32(Math.Pow(10,i));
                int[] times = new int[repeats];
                Stopwatch sw = new Stopwatch();
                sw.Start();

                for (int j = 0; j < repeats; j++)
                {
                    times[j] = SingleHop(length, probability);
                }
                Tuple<double, double> stats = AverageAndStdevOfArray(times);
                sw.Stop();
                retval[iteration + 1] = new string[] { repeats.ToString(), stats.Item1.ToString(), stats.Item2.ToString(), sw.ElapsedMilliseconds.ToString() };
                // add to the array to return
            }
            
            return retval;
        }

        private static void ToCSV(Func<int,int,double> method, int arg1, int arg2, string method_str, int iterations)
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
                stat[i] = method(arg1, arg2);
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

        //private static void JaggedArrayToCSV(string[][] array)
        //{

        //    // Create a file to list our number of stat
        //    // Doing this as a .csv so statistics in excel are easy to manage

        //    string timestamp = DateTime.Now.ToString("yyyy-mm-d_hh-mm-ss");

        //    // See if M:\ exists and save there (i.e. if we're at uni), otherwise just save on the D:\

        //    string savepath = "D:\\Coding\\PHYS2320_Computing_2\\RandomHoppers\\";

        //    try
        //    {
        //        if (Directory.Exists("M:\\PHYS2320_Computing_2\\RandomHoppers"))
        //        {
        //            savepath = "M:\\PHYS2320_Computing_2\\RandomHoppers\\";
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        // fail silently
        //    }
            
        //    // TODO: parse array[0] as a save path
        //    System.IO.StreamWriter file = new System.IO.StreamWriter(savepath + method_str + "_" + iterations + "_" + timestamp + ".csv");

        //    if (array.Length > 1000000)
        //    {
        //        // fix this with exceptions
        //        Console.WriteLine("Your table is too damn big! (and will overflow excel. Use less than 1m loops");
        //        return;
        //    }

        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        string line = "";
        //        for (int j = 0; j < array[i].Length; j++)
        //        {
        //            if (j == (array[i].Length - 1))
        //            {
        //                line = line + array[i][j];
        //                // don't add an extra column on the final thing per line
        //            }
        //            else
        //                line = line + array[i][j] + ",";
        //        }
        //        file.WriteLine(line);
        //    }

        //    file.Close();
        //    // dont forget to close the file!

        //    return;

        //}

        static Tuple<double,double,double> MultiHop(int length, int maxtime, double probability)
        {
            // returns average number of hoppers per line and the standard deviation as a tuple of doubles

            int time = 0; // time counter
            int[] line = new int[length]; // initiate the line array

            // PrintCurrentState(time, length, line); // testing line only

            // Console.WriteLine("Starting hopper with parameters: \tlength = " + length + "\t maxtime = " + maxtime); // testing line only

            int[] hopper_count = new int[maxtime];
            // initiate array for getting average of hoppers
            
            int[] travel_time = new int[maxtime];
            // initiate array with size maxtime, as even if one hopper is added for every unit of time, the amount of hoppers in existence ever will never be more than maxtime

            // start hopper 1 off
            line[0] = 1;
            int unique_hoppers = 1;

            while (time < maxtime)
            {
                time++;
                // start of a run, increase time by 1 unit
                int current_hoppers = 0;               
                for (int i = length-1; i >= 0; i--) // work backwards along the line
                {
                    if (line[i] != 0) // using != 0 so we can distinguish between hoppers
                    {
                        current_hoppers++;
                        travel_time[line[i]-1] ++;
                        // using line[i] as an array index, i.e. first hopper is represented as "1", second "2" in the array. When it hits a number (e.g. "2"), increment travel time by one
                        
                        // run hop routine
                        if (probability > GetNextDouble()) // if coinflip works, hop!
                        {
                            if (i == length-1)
                            {
                                // take it off the end of the line if we're there
                                line[i] = 0;
                                current_hoppers--;

                            }
                            else // normal hop
                            {
                                if (line[i + 1] == 0) // if next space is free, hop!
                                {
                                    line[i + 1] = line[i];
                                    // change the variable first before we blank it otherwise we lose track of what it is!
                                    line[i] = 0;
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
                    // add a new hopper if the first position is free
                    {
                        // figure out what number to stick on the line
                        unique_hoppers++;
                        line[0] = unique_hoppers;
                        // incrementing a counter here is probably more efficent than searching the travel time array to figure out how many non-zero elements there are

                        // count non-zero array elements
                        current_hoppers++;
                        
                    }

                }
                hopper_count[time - 1] = current_hoppers;

                // PrintCurrentState(time, length, line); // (only used for visually testing)

            }

            // now that we have finished looping, take the minimum number from the line (counter always incremments), so we can disregard all values greater than 
            // line[line.min()] from our average travel time calculations (as they haven't finished hopping)

            // loops backward over the line. the first nonzero value it encounters is the smallest value on the line
            // since int isn't nullable, blank positions are represented by 0 not null, so we can't take the minimum of the array and we have to do this

            int linemin = MinNonZeroOfArray(line);
            double average_travel_time = -1;
            
            try
            {
                average_travel_time = travel_time.Take(linemin - 1).Average();
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("No hoppers have completed their run. No average travel time statistics available");
            }
            
            // get average hoppers per line between t=0 and t=maxtime, and standard deviation of this

            // interestingly the language implementations of .Average(); and this stdeviation calculation are fast as hell and
            // add almost nothing to the length of time to iterate over 10^8 time units (in the order of <1%)

            Tuple<double, double> array_stats = AverageAndStdevOfArray(hopper_count);

            return new Tuple<double, double, double>(array_stats.Item1, array_stats.Item2, average_travel_time);
            
        }

        private static Tuple<double, double> AverageAndStdevOfArray(int[] hopper_count)
        {
            double average = hopper_count.Average();
            double sumOfSquaresOfDifferences = hopper_count.Select(val => (val - average) * (val - average)).Sum();
            double stdev = Math.Sqrt(sumOfSquaresOfDifferences / hopper_count.Length);
            return new Tuple<double, double>(average, stdev);
        }

        private static int MinNonZeroOfArray(int[] line)
        {
            // function to return the nonzero minimum value of an array (assuming the array is sorted in descending order e.g. {9, 8, 7})
            
            for (int i = line.Length -1; i >= 0; i--)
            {
                if (line[i] == 0)
                {
                    continue;
                }
                else
                {
                    return line[i];
                }
            }
            return -1;
        }

        static int SingleHop(int length, double probability){
            int pos = 1; // position, x 
            int time = 0; // time counter

            // PrintCurrentState(time, length, pos);

            while (pos <= length)
            {
                time++;
                // start of a "hop", increase time by 1 unit

                // flip coin to see if you hop
                if (GetNextDouble() > 0.5)
                    pos++;

                // PrintCurrentState(time, length, pos); // this is broken since i changed it to use arrays for MultiHop and there's really no point changing
                // print what it looks like now
                // nb this is after first hop


                if (pos == length)
                {
                    // Console.WriteLine("End of line reached. It took " + time + " hops to reach the end of the line of length " + length);
                    return time; // return number of hops taken to reach end of line
                    // break out here, when the position reaches length
                }
            }
            // shouldn't reach here, so return -1
            return -1;
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
                if (line[i] != 0)
                    Console.Write(line[i]);
                // print hopper value if a hopper exists at this location (its gonna be numbered for multihoppers, 1 for singlehopper. i could distinguish but effort)
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
