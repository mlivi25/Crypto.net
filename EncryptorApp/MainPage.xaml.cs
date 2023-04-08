using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        private void resetPasswords()
        {
            PasswordInput.Password = "";
            PasswordInputConfirm.Password = "";
        }

        private void SubmitHash_Click(object sender, RoutedEventArgs e)
        {
            var payload = Encoding.UTF8.GetBytes(HashInput.Text);

            var hash = SHA256.Create().ComputeHash(payload);

            Base64HashResult.Text = Convert.ToBase64String(hash);

            ToStringHashResult.Text = BitConverter.ToString(hash);


        }

        private async Task<bool> doPasswordsMatchAsync()
        {
            bool result = PasswordInput.Password == PasswordInputConfirm.Password;

            if (PasswordInput.Password == "" || PasswordInputConfirm.Password == "")
            {
                var messageDialog = new MessageDialog("Empty password field.");
                await messageDialog.ShowAsync();
                return false;
            }
            else if (result)
            {
                return true;
            }
            else
            {
                var messageDialog = new MessageDialog("Passwords do not match.");
                await messageDialog.ShowAsync();
                return false;
            }
        }

        private async void AESEncyrpt_Click(object sender, RoutedEventArgs e)
        {
            if (!await doPasswordsMatchAsync())
                return; //Throw error;

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".txt");


            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file is null)
                return;

            Aes aes = Aes.Create();

            var pbSalt = new byte[16];
            RandomNumberGenerator.Create().GetBytes(pbSalt);

            string password = PasswordInput.Password;
            var pbkd2 = new Rfc2898DeriveBytes(password, pbSalt).GetBytes(32);

            aes.Key = pbkd2;
            aes.IV = pbSalt;
            var outFileName = Path.ChangeExtension(file.Name, ".enc");

            //https://learn.microsoft.com/en-us/windows/uwp/files/file-access-permissions
            var DownloadsFileEntry = await DownloadsFolder.CreateFileAsync(outFileName, CreationCollisionOption.GenerateUniqueName);

            using (var outFileStream = await DownloadsFileEntry.OpenStreamForWriteAsync())
            {
                outFileStream.Write(pbSalt, 0, pbSalt.Length);
                byte[] fileType = Encoding.ASCII.GetBytes(file.FileType);

                byte[] fileBytes = new byte[5];
                int i = 0;
                foreach (char c in fileType)
                {
                    fileBytes[i] = (byte)c;
                    i++;
                }

                outFileStream.Write(fileBytes, 0, fileBytes.Length);

                

                using (var outStreamEncrypted = new CryptoStream(outFileStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    // a time, you can save memory
                    // and accommodate large files.
                    int count = 0;
                    int offset = 0;

                    // blockSizeBytes can be any arbitrary size.
                    int blockSizeBytes = aes.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];
                    int bytesRead = 0;

                    using (var inFs = await file.OpenStreamForReadAsync())
                    {
                        do
                        {
                            count = inFs.Read(data, 0, blockSizeBytes);
                            offset += count;
                            outStreamEncrypted.Write(data, 0, count);
                            bytesRead += blockSizeBytes;
                        } while (count > 0);
                    }
                    outStreamEncrypted.FlushFinalBlock();
                }
            }
            resetPasswords();
        }

        private async void AesDecryptFile_Click(object sender, RoutedEventArgs e)
        {
            if (!await doPasswordsMatchAsync())
                return; //Throw error;

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            openPicker.FileTypeFilter.Add(".enc");


            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file is null)
                return;
            Aes aes = Aes.Create();


            using (var inFs = await file.OpenStreamForReadAsync())
            {
                var pbSalt = new byte[16];
                inFs.Seek(0, SeekOrigin.Begin);
                inFs.Read(pbSalt, 0, 16);

                var fileTypeB = new byte[5];
                inFs.Read(fileTypeB, 0, fileTypeB.Length);

                string fileType = Encoding.ASCII.GetString(fileTypeB, 0, fileTypeB.Length).Replace("\0", "");


                string password = PasswordInput.Password;
                var pbkd2 = new Rfc2898DeriveBytes(password, pbSalt).GetBytes(32);

                aes.Key = pbkd2;
                aes.IV = pbSalt;

                // Decrypt the cipher text from
                // from the FileSteam of the encrypted
                // file (inFs) into the FileStream
                // for the decrypted file (outFs).
                using (var outStreamDecrypted = new CryptoStream(inFs, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    //var outFileName = Path.ChangeExtension(file.Name, ".txt"); 
                    var outFileName = Path.ChangeExtension(file.Name, fileType);

                    //https://learn.microsoft.com/en-us/windows/uwp/files/file-access-permissions
                    var DownloadsFileEntry = await DownloadsFolder.CreateFileAsync(outFileName, CreationCollisionOption.GenerateUniqueName);

                    using (var outFileStream = await DownloadsFileEntry.OpenStreamForWriteAsync())
                    {
                        int count = 0;
                        int offset = 0;
                        int blockSizeBytes = aes.BlockSize / 8;
                        byte[] data = new byte[blockSizeBytes];
                        do
                        {
                            count = outStreamDecrypted.Read(data, 0, blockSizeBytes);
                            offset += count;
                            outFileStream.Write(data, 0, count);
                        } while (count > 0);
                        
                    }
                }
                resetPasswords();
            }

            }

    }
}
