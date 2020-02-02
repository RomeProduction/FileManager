using System;
using System.Collections.Generic;
using System.Text;

namespace FileManager.Models.LocalProvider
{
	public class FileLocal
	{
		public string Path { get; set; }
		public string Name { get; set; }
		public string Extension { get; set; }
		public byte[] Content { get; set; }
	}
}
