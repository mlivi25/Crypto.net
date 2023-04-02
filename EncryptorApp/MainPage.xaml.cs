using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EncryptorApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void HashInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            Console.WriteLine(HashInput.Text);
        }

        private async void SubmitHash_Click(object sender, RoutedEventArgs e)
        {
            var payload = Encoding.UTF8.GetBytes(HashInput.Text);

            var hash = SHA256.Create().ComputeHash(payload);

            Base64HashResult.Text = Convert.ToBase64String(hash);

            ToStringHashResult.Text = BitConverter.ToString(hash);


            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".txt");


            StorageFile file = await openPicker.PickSingleFileAsync();
            var test = file.

            var hashFile = SHA256.Create().ComputeHash("");

            if (file != null)
            {
                // Application now has read/write access to the picked file
                //OutputTextBlock.Text = "Picked photo: " + file.Name;
            }
            else
            {
                //OutputTextBlock.Text = "Operation cancelled.";
            }


            //var x = new FileOpenPicker();
            //x.PickSingleFileAsync();
        }
    }
}
