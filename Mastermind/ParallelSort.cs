using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mastermind
{
	/// <summary>
	/// Parallel quicksort algorithm.
	/// </summary>
	public class ParallelSort
	{
		#region Public Static Methods

		/// <summary>
		/// Sequential quicksort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="arr"></param>
		public static void QuicksortSequential<T>(T[] arr) where T : IComparable<T>
		{
			QuicksortSequential(arr, 0, arr.Length - 1);
		}

		/// <summary>
		/// Parallel quicksort
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="arr"></param>
		public static void QuicksortParallel<T>(T[] arr) where T : IComparable<T>
		{
		const int MERGE_THRESHOLD = 50000;

			//It seems to be faster to sort sections of the array in independent threads,
			//then doing a quick merge sort on the already sorted sections.
			//This can't be done in place however, increasing memory usage.
			if (arr.Length > MERGE_THRESHOLD)
			{
			Thread[] QuickSortThreads = new Thread[Environment.ProcessorCount];

				for (int i = 0; i < QuickSortThreads.Length; i++)
				{
				int Left = (arr.Length / QuickSortThreads.Length) * i;
				int Right = Left + (arr.Length / QuickSortThreads.Length) - 1;

					//Use a 16MB stack (default 4) to decrease the odds of a recursion stack overflow in 700,000+ element arrays
					QuickSortThreads[i] = new Thread(new ThreadStart(delegate { QuicksortSequential(arr, Left, Right); }), 16*1024*1024);
					QuickSortThreads[i].Priority = ThreadPriority.BelowNormal;
					QuickSortThreads[i].Start();
				}

				//Wait for threads to finish
				for (int i = 0; i < QuickSortThreads.Length; i++)
				{
					QuickSortThreads[i].Join();
				}

				//Merge sort sub arrays
			T[] NewArr = new T[arr.Length];
			int[] CurrentIndex = new int[Environment.ProcessorCount];
			int[] MaxIndex = new int[Environment.ProcessorCount];

				for (int i = 0; i < MaxIndex.Length; i++)
				{
					int Left = (arr.Length / QuickSortThreads.Length) * i;
					int Right = Left + (arr.Length / QuickSortThreads.Length);

					CurrentIndex[i] = Left;
					MaxIndex[i] = Right;
				}

				MaxIndex[MaxIndex.Length - 1] = NewArr.Length;

				for (int i = 0; i < NewArr.Length; i++)
				{
				int UsedIndex = -1;
				T Lowest = default(T);

					for (int j = 0; j < CurrentIndex.Length; j++)
					{
						if(CurrentIndex[j] < MaxIndex[j])
						{
							if(UsedIndex == -1)
							{
								Lowest = arr[CurrentIndex[j]];
								UsedIndex = j;
							}
							else if (arr[CurrentIndex[j]].CompareTo(Lowest) < 0)
							{
								Lowest = arr[CurrentIndex[j]];
								UsedIndex = j;
							}
						}
					}

					CurrentIndex[UsedIndex]++;
					NewArr[i] = Lowest;
				}

				NewArr.CopyTo(arr, 0);
				NewArr = null;
				GC.Collect();
			}
			else
			{
				QuicksortParallel(arr, 0, arr.Length - 1);
			}
		}

		#endregion

		#region Private Static Methods

		private static void QuicksortSequential<T>(T[] arr, int left, int right) where T : IComparable<T>
		{
			if (right > left)
			{
				int pivot = Partition(arr, left, right);
				
				QuicksortSequential(arr, left, pivot - 1);
				QuicksortSequential(arr, pivot + 1, right);
			}
		}

		private static void QuicksortParallel<T>(T[] arr, int left, int right) where T : IComparable<T>
		{
		const int SEQUENTIAL_THRESHOLD = 2000;
			
			if (right > left)
			{
				if (right - left < SEQUENTIAL_THRESHOLD)
				{
					QuicksortSequential(arr, left, right);
				}
				else
				{
					int pivot = Partition(arr, left, right);
					
					Parallel.Invoke(new Action[] 
						{ 
							delegate {QuicksortParallel(arr, left, pivot - 1);},
							delegate {QuicksortParallel(arr, pivot + 1, right);}
						});
				}
			}
		}

		private static void Swap<T>(T[] arr, int i, int j)
		{
			T tmp = arr[i];
			arr[i] = arr[j];
			arr[j] = tmp;
		}

		private static int Partition<T>(T[] arr, int low, int high)
			where T : IComparable<T>
		{
			// Simple partitioning implementation
			int pivotPos = (high + low) / 2;
			T pivot = arr[pivotPos];
			Swap(arr, low, pivotPos);

			int left = low;
			for (int i = low + 1; i <= high; i++)
			{
				if (arr[i].CompareTo(pivot) < 0)
				{
					left++;
					Swap(arr, i, left);
				}
			}

			Swap(arr, low, left);
			return left;
		}

		#endregion
	}
}
