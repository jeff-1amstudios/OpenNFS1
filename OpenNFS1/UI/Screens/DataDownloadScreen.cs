using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Ionic.Zip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NfsEngine;

namespace OpenNFS1.UI.Screens
{
	class DataDownloadScreen : BaseUIScreen, IGameScreen
	{
		Thread _downloadThread;
		long _dataContentLength, _dataDownloaded;
		bool _downloadError;
		bool _unpacking;
		bool _completed;

		public DataDownloadScreen() : base()
		{
			_downloadThread = new Thread(DownloadDataThreadProc);
			_downloadThread.Priority = ThreadPriority.AboveNormal;
			_downloadThread.Start();
		}

		public void Update(Microsoft.Xna.Framework.GameTime gameTime)
		{
			if (Engine.Instance.Input.WasPressed(Keys.Enter))
			{
				if (_completed)
					Engine.Instance.Screen = new HomeScreen();
				else if (_downloadError)
					Engine.Instance.Game.Exit();
			}
		}

		public override void Draw()
		{
			base.Draw();
			
			WriteTitleLine("Download CD data");

			if (_dataDownloaded > 0)
			{
				int ratio = (int)(((double)_dataDownloaded / (double)_dataContentLength) * 50);
				string progress = new string('=', ratio);
				WriteLine(progress, Color.White, 150, 30, 0.5f);

				long downloadedMb = _dataDownloaded / 1024 / 1024;
				long contentLengthMb = _dataContentLength / 1024 / 1024;
				WriteLine(String.Format("Downloaded {0}mb / {1}mb", downloadedMb, contentLengthMb));
			}

			if (_unpacking)
			{
				WriteLine("");
				WriteLine("Unpacking into CD_Data folder...");
			}
			if (_completed)
			{
				WriteLine("Unpacking complete! Hit enter to continue.");
			}

			if (_downloadError)
			{
				WriteLine("An error occured while downloading.");
				WriteLine("Please check exception.txt for details.");
				WriteLine("");
				WriteLine("Press Enter to exit.");
			}

			WriteLine("The Need for Speed 1 CD data package contains files produced", Color.White, 350, 30, 0.5f);
			WriteLine("by Pioneer Productions / EA Seattle in 1995.");
			WriteLine("* 1amStudios and OpenNFS1 are not connected in any way", Color.White, 420, 30, 0.5f);
			WriteLine("with Pioneer Productions or EA Seattle.");

			Engine.Instance.SpriteBatch.End();
		}

		private void DownloadDataThreadProc()
		{
			try
			{
				string url = "http://www.1amstudios.com/download/nfs1_cd_data.zip";
				url = "http://127.0.0.1/nfs1_cd_data.zip";

				WebRequest request = WebRequest.Create(url);
				var response = request.GetResponse();
				_dataContentLength = long.Parse(response.Headers["Content-Length"]);

				byte[] buffer = new byte[4096];
				string tempFileName = Path.GetTempFileName();
				Stream fileStream = File.Open(tempFileName, FileMode.Create);
				using (Stream s = response.GetResponseStream())
				{
					while (true)
					{
						int read = s.Read(buffer, 0, 4096);
						_dataDownloaded += read;
						if (read == 0)
							break;
						fileStream.Write(buffer, 0, read);
					}
				}
				fileStream.Close();
				_unpacking = true;

				var zipFile = ZipFile.Read(tempFileName);
				Directory.CreateDirectory("CD_Data");
				zipFile.ExtractAll("CD_Data");
				zipFile.Dispose();
				File.Delete(tempFileName);
				_completed = true;
			}
			catch (Exception ex)
			{
				File.WriteAllText("exception.txt", ex.ToString());
				_downloadError = true;
			}
		}
	}
}
