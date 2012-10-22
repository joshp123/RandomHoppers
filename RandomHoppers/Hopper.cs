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
            LoopOverPowersOfTen(2,2,1,0.9);

            // TODO: make this properly OO

            // TODO: make this a UI

            // TODO: document all functions properly. lmao this wont be fun
                                
            return;
        }

        private static void LoopOverPowersOfTen(int min, int max, double interval, double probability)
        {
            for (double i = min; i == max; i += interval)
            {
                Console.WriteLine("Testing a MultiHop of line length 50, probability 0.5, for 10^" + i + " iterations");

                if (i >= 9)
                // dont' do more than 100m iterations becaue that makes an object larger than 2gb and you get an OutOfMemory exception
                // creating an array of lenght 10^9 doesn't work in C either without ~workarounds~ so WONTFIX
                {
                    Console.WriteLine("Iteration size is too large and will overflow the memory. Stopping");
                    break;
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();

                Tuple<double, double> average_and_stdev = MultiHop(50, Convert.ToInt32(Math.Pow(10, i)), probability);
                double average = average_and_stdev.Item1;
                double stdev = average_and_stdev.Item2;

                Console.WriteLine("Average = " + average);
                Console.WriteLine("Standard deviation = " + stdev);
                Console.WriteLine("This took: " + sw.ElapsedMilliseconds + " milliseconds\n");

                sw.Stop();
            }
        }

        private static void LoopOverProbabilities(int min, int max, double interval, int iterations)
        {
            for (double i = min; i < max; i += interval)
            {
                Console.WriteLine("Testing a MultiHop of line length 50, probability " + i + ", for 10^" + iterations + " iterations");

                if (iterations >= 100000000)
                // dont' do more than 100m iterations becaue that makes an object larger than 2gb and you get an OutOfMemory exception
                // creating an array of lenght 10^9 doesn't work in C either without ~workarounds~ so WONTFIX
                {
                    Console.WriteLine("Iteration size is too large and will overflow the memory. Stopping");
                    break;
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();

                Tuple<double, double> average_and_stdev = MultiHop(50, iterations, i);
                double average = average_and_stdev.Item1;
                double stdev = average_and_stdev.Item2;

                Console.WriteLine("Average = " + average);
                Console.WriteLine("Standard deviation = " + stdev);
                Console.WriteLine("This took: " + sw.ElapsedMilliseconds + " milliseconds\n");

                sw.Stop();
            }
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

        private static void ToCSV(Func<int,int,int,Tuple<double,double>> method, int arg1, int arg2, int arg3, string method_str, string header1, string header2, int iterations)
        {
             /*
              *
              *  Same as the previous function but overloaded version for the multihopper method
              *  
              *  Method to run another method a certain number of times, and spit out a CSV file based on whatever
              *  the method returns.
              *  Use it to create statistics on, well, pretty much anything!
              *
              *  Takes arguments method with , that
              *  method as a string (this is purely for naming files), and the number of iterations to run it for.
              *  
              *  Don't call this for more than 1m iterations because then i'll have to implement columns of columns 
              *  and that would be a bit complicated. tia 
              * 
              *  Header1 and Header2 are names of the return values of the function "method". e.g. "Average" and "Standard Deviation"
              *  This is only used for writing to the CSV so you can really do whatever you like with it
              */


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

            if (iterations > 1000000)
            {
                // fix this with exceptions
                Console.WriteLine("Using > 1m iterations with multiple statistics is not supported. Please decrease the number of iterations!");
                return;
            }
            // don't create a multicolumn spreadsheet

            // Create an array for the statistic; using doubles incase we want to take an average of it

            Tuple<double,double>[] stat = new Tuple<double,double>[iterations];
            string line = "";
            file.WriteLine(header1 + ", " + header2);
            // write header row

            for (int i = 0; i < iterations; i++)
            {
                stat[i] = method(arg1, arg2, arg3); // sort columns in this overloaded method
                // call the method specified when calling this method (ToCSV)

                file.WriteLine(stat[i].Item1 + ", " + stat[i].Item2);
                // write each element of the tuple to a separate column

                // it would be pro if you could use foreach on tuples but apparently you can't so welp
                // not that it would save much time since you'd have to overload this function anyway
                // if you were feeding it methods that return N-item tuples
            }

            if (line != "")
                file.WriteLine(line);

            // if it ended on i = 999 and line is not blank or whatever write the residual bit

            file.Close();
            // dont forget to close the file!

            return;

        }

        static Tuple<double,double> MultiHop(int length, int maxtime, double probability)
        {
            // returns average number of hoppers per line and the standard deviation as a tuple of doubles

            int time = 0; // time counter
            int[] line = new int[length]; // initiate the line array

            PrintCurrentState(time, length, line); // testing line only

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

                PrintCurrentState(time, length, line); // (only used for visually testing)

            }

            // now that we have finished looping, take the minimum number from the line (counter always incremments), so we can disregard all values greater than 
            // line[line.Max()] from our average travel time calculations (as they haven't finished hopping)

            // this is wrong var valid_times = new ArraySegment<int>(travel_time, 0, line.Min() - 2);
            Console.WriteLine(line.Min());
            foreach(int i in travel_time.Take(line.Min(j => j != 0)))
                Console.WriteLine(i);
            double average_travel_time = travel_time.Take(line.Min() - 1).Average();

                        

            // get average hoppers per line between t=0 and t=maxtime, and standard deviation of this

            // interestingly the language implementations of .Average(); and this stdeviation calculation are fast as hell and
            // add almost nothing to the length of time to iterate over 10^8 time units (in the order of <1%)


            double average = hopper_count.Average();
            double sumOfSquaresOfDifferences = hopper_count.Select(val => (val - average) * (val - average)).Sum();
            double stdev = Math.Sqrt(sumOfSquaresOfDifferences / hopper_count.Length); 

            return new Tuple<double,double>(average,stdev);
            
        }

        static double SingleHop(int length, double probability){
            int pos = 1; // position, x 
            double time = 0; // time counter

            // PrintCurrentState(time, length, pos);

            while (pos <= length)
            {
                time++;
                // start of a "hop", increase time by 1 unit

                // flip coin to see if you hop
                if (GetNextDouble() > 0.5)
                    pos++;

                // PrintCurrentState(time, length, pos);
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
