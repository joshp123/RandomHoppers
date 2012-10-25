/*Programme to generate ranom numbers between 0 and 1*/

#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <math.h>

float Average(int array[], int length)
{
	float sum = 0;

	for (int i = 0; i < length; ++i)
	{
		sum+= array[i];
	}
	float average = sum / length;
	return average;
}

void VisualizeHopper(int line[], int time, int length)
{
	printf("%d :",time );
	for (int i = 0; i < length; ++i)
	{
		if (line[i] == 1)
			printf("X");
		else
			printf("_");
	}
	printf("\n");
}

float StDev(int array[], int length, float average)
{
	float sumOfSquaresOfDifferences = 0.0;
	for (int i = 0; i < length; ++i)
	{
		sumOfSquaresOfDifferences += pow((array[i] - average),2);
	}
	float stdev = sqrt(sumOfSquaresOfDifferences / length );
	return stdev;
}

void MultiHopper(int length, float probability, int maxtime)
{
	int line[length];
	for (int i = 0; i < length; ++i)
	{
		line[i] = 0;
	}

	// initialize the line as an array of 0

	line[0] = 1; 
	int time = 0;
	int hoppers_average[maxtime];
	for (int i = 0; i < maxtime; ++i)
	{
		hoppers_average[i] = 0;
	}
	// initialize the array full of zeros 

	// start the hopper off at position 1
	while(time < maxtime + 1)
	{
		time++;
		int current_hoppers = 0;
		for (int i = 0; i < length; ++i)
		{
			if (line[i] == 1)// if there's a hopper present
			{
				current_hoppers++;
				if ((rand()/(float)(RAND_MAX)) <= probability)
				// if the random number is greater than probability, hop
				{
					if((i + 1) == length)
					{
						// it's reached the end of the line, done
						line[i] = 0;
					}
						
					else if (line[i+1] == 1)
						// if there's a hopper on the next position, don't do anything
					{
						continue;
					}
					else
						// hop
					{
						line[i+1] = 1;
						line[i] = 0;
					}
				}
			}
			if (line[0] == 0)
			{
				// if first postiion is blank, add a new hopper
				current_hoppers++;
				line[0] = 1;
			}
		}
		hoppers_average[time-1] = current_hoppers;
		VisualizeHopper(line,time,length);

	}

	float average = Average(hoppers_average,maxtime);
	float stdev = StDev(hoppers_average, maxtime, average);
	printf("Average of %d second = %f \n",maxtime,average);
	printf("Standard Deviaton of %d seconds = %f \n",maxtime,stdev);

}



int main ()
{

	/* using srand() to generate random numbers*/

	srand(time(NULL));
	int length = 50;
	float a[length];
	int time_to_run_until = 10000;

	MultiHopper(50,0.5,time_to_run_until);

	// float average = Average(hoppers_over_time,time_to_run_until);
	// float stdev = StDev(hoppers_over_time, length, average);
	// printf("Average of %d second = %f \n",time_to_run_until,average);
	// printf("Standard Deviaton of %d seconds = %f \n",time_to_run_until,stdev);

	/* Now generate 50 random numbers */

	/*int i;
	for (i=0; i<length; i++)
	{
		a[i] =(rand()/(float)(RAND_MAX));
		printf ("a[%d]= %f \n", i, a[i]);
	}
	printf("average = %f \n",Average(a, length));
	return 0;*/

}

