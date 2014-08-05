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

			WriteLine("Downloading CD data package", Color.Red, 0, 30, TextSize);

			if (_dataDownloaded > 0)
			{
				int ratio = (int)(((double)_dataDownloaded / (double)_dataContentLength) * 50);
				string progress = new string('=', ratio);
				WriteLine(progress, TextColor, 80, 30, TextSize);

				long downloadedMb = _dataDownloaded / 1024 / 1024;
				long contentLengthMb = _dataContentLength / 1024 / 1024;
				WriteLine(String.Format("Downloaded {0}mb / {1}mb", downloadedMb, contentLengthMb), TextColor, 20, 30, TextSize);
			}

			if (_unpacking)
			{
				WriteLine("Unpacking into CD_Data folder...", TextColor, 30, 30, TextSize);
			}
			if (_completed)
			{
				WriteLine("Unpacking complete! Hit enter to continue.", TextColor, 30, 30, TextSize);
			}

			if (_downloadError)
			{
				WriteLine("An error occured while downloading - please check\r\nexception.txt for details.\r\n\r\nHit enter to quit.", TextColor, 60, 30, TextSize);
			}

			Engine.Instance.SpriteBatch.End();
		}

		private void DownloadDataThreadProc()
		{
			try
			{
				string url = "http://www.1amstudios.com/download/NFSSE_cd_data.zip";

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
