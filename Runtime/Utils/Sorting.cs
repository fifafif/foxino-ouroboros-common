using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public static class Sorting
    {
        public static void BubbleSort<T>(this IList<T> arr, System.Comparison<T> comparison)
        {
            //int[] arr = { 78, 55, 45, 98, 13 };
            T temp;
            for (int j = 0; j <= arr.Count - 2; j++)
            {
                for (int i = 0; i <= arr.Count - 2; i++)
                {
                    if (comparison((T)arr[i], (T)arr[i + 1]) == 1)
                    //if (arr[i] > arr[i + 1])
                    {
                        temp = (T)arr[i + 1];
                        arr[i + 1] = arr[i];
                        arr[i] = temp;
                    }
                }
            }
        }
    }
}