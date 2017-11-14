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
        private ObservableCollectionNumbers oNumbers = new ObservableCollectionNumbers()
        {
            //0,  6,  5,  7,  2,  3,  8,  1,  9,  4,
            //10, 12, 11, 13, 15, 16, 14, 19, 18, 17,
            //20, 29, 28, 27, 26, 25, 24, 23, 22, 21,
            //30
        };
        public MainPage()
        {
            this.InitializeComponent();
            GenerateNumberCollection();
            lvNumbers.ItemsSource = oNumbers;
            DataContext = this;

        }

        private void GenerateNumberCollection()
        {
            Random rand = new Random();
            for (int i = 0; i < 250; i++)
            {
                oNumbers.Add(new SortingItem() { Value = i});
            }
            //oNumbers.Shuffle();
        }

        private async void btInsertionSort_Click(object sender, RoutedEventArgs e)
        {
            //await ThreadPool.RunAsync( (WorkItem) => SortingAlgorithms.InsertionSortAsync(Dispatcher, oNumbers));
            await ThreadPool.RunAsync((WorkItem) => oNumbers.InsertionSort(Dispatcher));
        }
        private void btShuffle_Click(object sender, RoutedEventArgs e)
        {
            oNumbers.Shuffle();
        }
    }
    public class ObservableCollectionNumbers : ObservableCollection<SortingItem>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Sorts
        public async void InsertionSort(CoreDispatcher dispatcher)
        {
            Debug.WriteLine("----------");
            Debug.WriteLine("InsertionSort:");
            Debug.WriteLine("Starting state:");
            Print();
            for (int i = 1; i < Count; i++)
            {
                for (int j = i; j > 0 && this[j - 1].Value > this[j].Value; j--)
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                    {
                        Swap(j, j - 1);
                        //Print();
                    });
                }
            }
            Debug.WriteLine("----------");
        }
        #endregion

        #region Utils
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
        #endregion
    }
    public class SortingItem
    {
        private int _Value;
        public int Value { get { return _Value; } set { _Value = value; } }
        public int Height {
            get
            {
                if (_Value != 0)
                {
                    return (400 / 250) * Value;
                }
                else
                {
                    return 1;
                }
            }
        }
    }


}
