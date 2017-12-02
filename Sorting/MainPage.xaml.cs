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
using Windows.Media;
using System.Runtime.InteropServices;
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
            await dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                Swap(IndexA, IndexB);
                //Print();
                SortingItem item = this[IndexB];
                Beeper.Beep(item.Value * 150, 1);
            });
            await Task.Delay(1);
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
    public static class Beeper
    {
        public static void Beep(double freq, uint tenthseconds)
        {
            string header_GroupID = "RIFF";  // RIFF
            uint header_FileLength = 0;      // total file length minus 8, which is taken up by RIFF
            string header_RiffType = "WAVE"; // always WAVE

            string fmt_ChunkID = "fmt "; // Four bytes: "fmt "
            uint fmt_ChunkSize = 16;     // Length of header in bytes
            ushort fmt_FormatTag = 1;        // 1 for PCM
            ushort fmt_Channels = 1;         // Number of channels, 2=stereo
            uint fmt_SamplesPerSec = 14000;  // sample rate, e.g. CD=44100
            ushort fmt_BitsPerSample = 8;   // bits per sample
            ushort fmt_BlockAlign =
                (ushort)(fmt_Channels * (fmt_BitsPerSample / 8)); // sample frame size, in bytes
            uint fmt_AvgBytesPerSec =
                fmt_SamplesPerSec * fmt_BlockAlign; // for estimating RAM allocation

            string data_ChunkID = "data";  // "data"
            uint data_ChunkSize;           // Length of header in bytes
            byte[] data_ByteArray;

            // Fill the data array with sample data

            // Number of samples = sample rate * channels * bytes per sample * duration in seconds
            uint numSamples = fmt_SamplesPerSec * fmt_Channels * tenthseconds / 10;
            data_ByteArray = new byte[numSamples];

            //int amplitude = 32760, offset=0; // for 16-bit audio
            int amplitude = 127, offset = 128; // for 8-audio
            double period = (2.0 * Math.PI * freq) / (fmt_SamplesPerSec * fmt_Channels);
            double amp;
            for (uint i = 0; i < numSamples - 1; i += fmt_Channels)
            {
                amp = amplitude * (double)(numSamples - i) / numSamples; // amplitude decay
                                                                         // Fill with a waveform on each channel with amplitude decay
                for (int channel = 0; channel < fmt_Channels; channel++)
                {
                    data_ByteArray[i + channel] = Convert.ToByte(amp * Math.Sin(i * period) + offset);
                }
            }

            // Calculate file and data chunk size in bytes
            data_ChunkSize = (uint)(data_ByteArray.Length * (fmt_BitsPerSample / 8));
            header_FileLength = 4 + (8 + fmt_ChunkSize) + (8 + data_ChunkSize);

            // write data to a MemoryStream with BinaryWriter
            MemoryStream audioStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(audioStream);

            // Write the header
            writer.Write(header_GroupID.ToCharArray());
            writer.Write(header_FileLength);
            writer.Write(header_RiffType.ToCharArray());

            // Write the format chunk
            writer.Write(fmt_ChunkID.ToCharArray());
            writer.Write(fmt_ChunkSize);
            writer.Write(fmt_FormatTag);
            writer.Write(fmt_Channels);
            writer.Write(fmt_SamplesPerSec);
            writer.Write(fmt_AvgBytesPerSec);
            writer.Write(fmt_BlockAlign);
            writer.Write(fmt_BitsPerSample);

            // Write the data chunk
            writer.Write(data_ChunkID.ToCharArray());
            writer.Write(data_ChunkSize);
            foreach (byte dataPoint in data_ByteArray)
            {
                writer.Write(dataPoint);
            }
            MediaElement player = new MediaElement();
            audioStream.Seek(0, SeekOrigin.Begin);
            player.SetSource(audioStream.AsRandomAccessStream(), "audio/wav");
            player.Play();
            //player = null;
            //audioStream.Dispose();
        }
    }
}
