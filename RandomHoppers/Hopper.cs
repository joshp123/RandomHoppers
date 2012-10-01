using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomHoppers
{
    class Hopper
    {
        // Declaring instance of Random outside of the Hop() function, because:
        // otherwise when calling Hop to create a new Random in close succession, they'll have the same system time as seed and thus will not be random.
        
        static Random rand = new Random();

        protected static double GetNextDouble()
        {
            return rand.NextDouble();
        }



        static void Main(string[] args)
        {
            ToCSV(Hop,"Hop",1000);

            //double average_hops = hops.Average();

            // take average of array

            //Console.WriteLine(average_hops);
                                 
            return;
        }

        static void ToCSV(Func<double> method, string method_str, int iterations)
        {
            // Method to run another method a certain number of times, and spit out a CSV file based on whatever the method returns.
            // Use it to create statistics on, well, pretty much anything!

            // Takes arguments method (must return a covariant of double. i.e. floats and ints are ok), that method as a string (this is purely for naming files),
            // and the number of iterations to run it for.

            // CSV file is excel compatible; starts off as a single column file until it hits excel 2013's row limit (~1m)
            
            
            // Create a file to list our number of stat
            // Doing this as a .csv so statistics in excel are easy to manage
            
            string timestamp = DateTime.Now.ToString("yyyy-mm-d_hh-mm-ss");
            System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\Coding\\PHYS2320_Computing_2\\RandomHoppers\\" + method_str + "_" + iterations + "_" + timestamp + ".csv");
            
            int columns;

            if (iterations > 1000000)
            {
                columns = 1000000 / iterations;
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
                    line = line + stat[i].ToString() + ", ";
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

        static double Hop()
        {
            int length = 50; // length of line to hop
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

        static void PrintCurrentState(int time, int length, int pos)
        {

            if (time == 0)
                Console.Write("Start:\t");
            else
            {
                Console.Write(time + "\t");
            }
            for (int i = 1; i <= length; i++)
            {
                if (i == pos)
                    Console.Write("X");
                // print an X at the place the object is
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
