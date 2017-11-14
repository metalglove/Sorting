using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Sorting
{
    public static class SortingAlgorithms
    {
        public static async void InsertionSortAsync(CoreDispatcher dispatcher, ObservableCollectionNumbers numbers)
        {
            Debug.WriteLine("----------");
            Debug.WriteLine("InsertionSort:");
            Debug.WriteLine("Starting state:");
            numbers.Print();
            for (int i = 1; i < numbers.Count; i++)
            {
                for (int j = i; j > 0 && numbers[j - 1].Value > numbers[j].Value; j--)
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () => 
                    {
                        numbers.Swap(j, j - 1);
                        numbers.Print();
                    });
                }
            }
            Debug.WriteLine("----------");
        }
    }
}
