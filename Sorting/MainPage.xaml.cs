using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Sorting
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollectionNumbers oNumbers;
        public MainPage()
        {
            this.InitializeComponent();
            oNumbers = new ObservableCollectionNumbers(Dispatcher);
            lvNumbers.ItemsSource = oNumbers;
            DataContext = this;
        }
        private async void btInsertionSort_Click(object sender, RoutedEventArgs e)
        {
            await ThreadPool.RunAsync((WorkItem) => oNumbers.InsertionSort());
        }
        private async void btSelectionSort_Click(object sender, RoutedEventArgs e)
        {
            await ThreadPool.RunAsync((WorkItem) => oNumbers.SelectionSort());
        }
        private async void btBubbleSort_Click(object sender, RoutedEventArgs e)
        {
            await ThreadPool.RunAsync((WorkItem) => oNumbers.BubbleSort());
        }
        private async void btQuickSort_Click(object sender, RoutedEventArgs e)
        {
            await ThreadPool.RunAsync((WorkItem) => oNumbers.QuickSort());
        }
        private void btShuffle_Click(object sender, RoutedEventArgs e)
        {
            oNumbers.Shuffle();
        }
    }
    public class ObservableCollectionNumbers : ObservableCollection<SortingItem>
    {
        private CoreDispatcher dispatcher;
        public ObservableCollectionNumbers(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            GenerateNumberCollection();
        }

        #region Sorts
        public async void InsertionSort()
        {
            for (int i = 1; i < Count; i++)
            {
                for (int j = i; j > 0 && this[j - 1].Value > this[j].Value; j--)
                {
                    await SwapAndPrint(j, j - 1);
                }
            }
        }
        public async void SelectionSort()
        {
            for (int i = 0; i < Count; i++)
            {
                int LowestValue = i;
                for (int j = i + 1; j < Count; j++)
                {
                    if (this[j].Value < this[LowestValue].Value)
                    {
                        LowestValue = j;
                    }
                }
                await SwapAndPrint(i, LowestValue);
            }
        }
        public async void BubbleSort()
        {
            for (int i = 0; i < Count - 1; i++)
            {
                bool Swapped = false;
                for (int j = 0; j < Count - 1; j++)
                {
                    if (this[j].Value > this[j + 1].Value)
                    {
                        await SwapAndPrint(j, j + 1);
                        Swapped = true;
                    }
                }
                if (Swapped == false)
                {
                    break;
                }
            }
        }
        public async void QuickSort()
        {
            await Quick(0, Count - 1);

            async Task Quick(int left, int right)
            {
                if (right - left <= 0)
                {
                    return;
                }
                else
                {
                    int Pivot = this[right].Value;
                    int PartitionPoint = await Partition(left, right, Pivot);
                    await Quick(left, PartitionPoint - 1);
                    await Quick(PartitionPoint + 1, right);
                }
            }

            async Task<int> Partition(int left, int right, int pivot)
            {
                int LeftPointer = left - 1;
                int RightPointer = right;

                while (true)
                {
                    while (this[++LeftPointer].Value < pivot)
                    {
                        //Increase leftpointer until value of the leftpointer is lower then the pivot
                    }
                    while (RightPointer > 0 && this[--RightPointer].Value > pivot)
                    {
                        //Decrease rightpointer until value of the rightpointer is higher then the pivot and is higher then 0
                    }
                    if (LeftPointer >= RightPointer)
                    {
                        break;
                    }
                    else
                    {
                        await SwapAndPrint(LeftPointer, RightPointer);
                    }
                }
                await SwapAndPrint(LeftPointer, right);
                return LeftPointer;
            }
        }
        #endregion

        #region Utils
        private async Task SwapAndPrint(int IndexA, int IndexB)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                Swap(IndexA, IndexB);
                //Print();
            });
        }
        public void Print()
        {
            Debug.WriteLine("");
            foreach (SortingItem item in this)
            {
                Debug.Write(item.Value + " ");
            }
            Debug.WriteLine("");
        }
        public void Swap(int IndexA, int IndexB)
        {
            SortingItem temp = this[IndexA];
            this[IndexA] = this[IndexB];
            this[IndexB] = temp;
        }
        public void Shuffle()
        {
            Random rand = new Random();
            for (int i = 0; i < Count - 1; ++i)
            {
                int randomNumber = rand.Next(i, Count);
                Swap(i, randomNumber);
            }
        }
        public void GenerateNumberCollection()
        {
            Random rand = new Random();
            for (int i = 0; i < 500; i++)
            {
                Add(new SortingItem() { Value = i });
            }
        }
        #endregion
    }
    public class SortingItem
    {
        private int _Value;
        public int Value { get { return _Value; } set { _Value = value; } }
        public int Height
        {
            get
            {
                if (_Value != 0)
                {
                    return (500 / 500) * Value;
                }
                else
                {
                    return 0;
                }
            }
        }
    }


}
