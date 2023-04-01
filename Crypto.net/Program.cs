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

//var sym = new AesGcm(key);

//var test3  = sym.Encrypt()



