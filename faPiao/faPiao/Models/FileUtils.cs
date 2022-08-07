using System;
using System.IO;

namespace iocr_api_demo
{
	public static class FileUtils
	{

		public static String getFileBase64(String fileName) { 
			FileStream filestream = new FileStream(fileName, FileMode.Open);
			byte[] arr = new byte[filestream.Length];
			filestream.Read(arr, 0, (int)filestream.Length);
			string baser64 = Convert.ToBase64String(arr);
			filestream.Close();
			return baser64;
		}
	}
}
