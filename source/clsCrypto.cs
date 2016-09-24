using System;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace TEICOCF.WebServices
{
	
	public class Crypto
	{

		private const string sEncryptionKey = "--7key3--";


		/// <summary>
		///    Decrypts  a particular string with a specific Key
		/// </summary>
		public static string Decrypt(string stringToDecrypt) 
		{
			byte[] key = {}; 
			byte[] IV = {10, 20, 30, 40, 50, 60, 70, 80};
			byte[] inputByteArray = new byte[stringToDecrypt.Length]; 
			try 
			{ 
				key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0,8)); 
				DESCryptoServiceProvider des = new DESCryptoServiceProvider(); 
				inputByteArray = Convert.FromBase64String(stringToDecrypt); 
				MemoryStream ms = new MemoryStream(); 
				CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write); 
				cs.Write(inputByteArray, 0, inputByteArray.Length); 
				cs.FlushFinalBlock(); 
				Encoding encoding = Encoding.UTF8 ; 
				return encoding.GetString(ms.ToArray()); 
			} 
			catch (System.Exception Ex) 
			{ 
				System.Diagnostics.Debug.WriteLine(Ex.ToString());
				return (Ex.ToString());
			} 
		} 

		/// <summary>
		///   Encrypts  a particular string with a specific Key
		/// </summary>
		/// <param name="stringToEncrypt"></param>
		/// <param name="sEncryptionKey"></param>
		/// <returns></returns>
		public static string Encrypt( string stringToEncrypt) 
		{
			byte[] key = {}; 
			byte[] IV = {10, 20, 30, 40, 50, 60, 70, 80}; 
			byte[] inputByteArray; //Convert.ToByte(stringToEncrypt.Length) 

			try 
			{ 
				key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0,8)); 
				DESCryptoServiceProvider des = new DESCryptoServiceProvider(); 
				inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt); 
				MemoryStream ms = new MemoryStream(); 
				CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write); 
				cs.Write(inputByteArray, 0, inputByteArray.Length); 
				cs.FlushFinalBlock(); 
				return Convert.ToBase64String(ms.ToArray()); 
			} 
			catch (System.Exception) 
			{ 
				return (string.Empty);
			} 
		} 
	} 
}