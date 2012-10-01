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
            // Create a file to list our number of hops
            System.IO.StreamWriter file = new System.IO.StreamWriter("d:\\hops.csv");
            // doing this as a csv so we can have like 10 columns to take statistics on it in excel without overflowing the ~1m row limit in excel (lol)

            // create an array of 10m hop values (it onlt takes 20s so fuck it, why not!)
            // using doubles since we're going to take an average of it

            int test_runs = 1000;

            double[] hops = new double[test_runs];
            string line = "";
            for (int i = 0; i < test_runs; i++)
            {
                hops[i] = Hop();
                if (i % 10 == 0)
                {
                    line = line + hops[i].ToString() + ", ";
                    file.WriteLine(line);
                    line = "";
                }
                else line = line + hops[i].ToString() + ", ";
                // Hope returns int but it should implicitly cast to double
            }

            file.Close();
            // close file

            double average_hops = hops.Average();

            // take average of array

            Console.WriteLine(average_hops);
                        
            return;
        }

        static int Hop()
        {
            int length = 50; // length of line to hop
            int pos = 1; // position, x 
            int time = 0; // time counter

            // PrintCurrentState(time, length, pos);

            while (pos <= length)
            {
                time++;
                // start of a "hop", increase time by 1 unit

                if (GetNextDouble() > 0.5)
                    pos++;

                // flip coin to see if you hop

                // PrintCurrentState(time, length, pos);
                // print what it looks like now
                // nb this is after first hop


                if (pos == length)
                {
                    // Console.WriteLine("End of line reached. It took " + time + " hops to reach the end of the line of length " + length);
                    return time;
                    // break;
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
                    Console.Write("o");
                }
                // print O in empty location
            }
            Console.WriteLine();
        }
    }
}
