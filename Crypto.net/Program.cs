// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text;


var testRandom = RandomNumberGenerator.GetBytes(32);

var testBase64 = Convert.ToBase64String(testRandom);

var testConversion = Convert.FromBase64String(testBase64);




var key = Encoding.UTF8.GetBytes("aqt1wtQVqTKXWWzdhHyxb8TuXmMmGymo");
Console.WriteLine(key.Length);

var sha256 = new HMACSHA256(key);

byte[] data = Encoding.ASCII.GetBytes("LexMcclellan");

var result = sha256.ComputeHash(data);

Console.WriteLine(BitConverter.ToString(result).Replace("-", "").ToLower());

var test2 = SHA256.Create().ComputeHash(data);
Console.WriteLine(BitConverter.ToString(test2).Replace("-", "").ToLower());


var ikm = RandomNumberGenerator.GetBytes(16);

var test = HKDF.DeriveKey(HashAlgorithmName.SHA256, data, 32);
Console.WriteLine(test.Length);

var sym = new AesGcm(key);

//var test3  = sym.Encrypt()




FileInfo file = new FileInfo(@"C:\Users\mitch\OneDrive\Desktop\result.txt");

Aes aes = Aes.Create();

//string outFile = Path.Combine(@"C:\Users\mitch\OneDrive\Desktop", Path.ChangeExtension(file.Name, ".enc"));

//string outFile = Path.ChangeExtension(file.Name, ".enc");

string outFile = "test.txt";

using (var outFileStream = new FileStream(outFile, FileMode.Create, FileAccess.Write, FileShare.Write))
{
    using (var outStreamEncrypted = new CryptoStream(outFileStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
    {
        // a time, you can save memory
        // and accommodate large files.
        int count = 0;
        int offset = 0;

        // blockSizeBytes can be any arbitrary size.
        int blockSizeBytes = aes.BlockSize / 8;
        byte[] fileData = new byte[blockSizeBytes];
        int bytesRead = 0;

        using (var inFs = new FileStream(file.FullName, FileMode.Open))
        {
            do
            {
                count = inFs.Read(fileData, 0, blockSizeBytes);
                offset += count;
                outStreamEncrypted.Write(fileData, 0, count);
                bytesRead += blockSizeBytes;
            } while (count > 0);
        }
        outStreamEncrypted.FlushFinalBlock();
    }
}