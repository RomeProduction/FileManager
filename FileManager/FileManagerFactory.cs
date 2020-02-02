using FileManager.Models.LocalProvider;
using FileManager.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileManager
{
	public enum FileManagerType
	{
		Local = 1
	}

	public class FileManagerFactory
	{
		public static IFileManager GetManager(FileManagerType managerType)
		{
			switch (managerType)
			{
				case FileManagerType.Local:
				{
					return new FileManager(new LocalProvider(new ConfigurationLocal()));
				}
				default:
				{
					throw new ArgumentNullException();
				}
			}
		}
	}

	public class FileManager : IFileManager
	{
		public FileManager(IFileProvider provider)
		{
		}
	}
}
