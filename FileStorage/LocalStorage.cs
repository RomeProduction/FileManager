using System;
using System.IO;

namespace FileStorage
{
	public class LocalStorage : IFileStorage<string>
	{
		private readonly string _path;
		public LocalStorage(string pathToStorage)
		{
			_path = pathToStorage;
		}

		public byte[] GetBytes(string id)
		{
			var pathToFile = GetPathToFile(id);
			byte[] content;
			using (var fs = GetFileExist(pathToFile, false))
			{
				content = new byte[fs.Length];
				fs.Read(content, 0, (int)fs.Length);
			}
			return content;
		}

		public Stream GetStream(string id)
		{
			var pathToFile = GetPathToFile(id);
			return GetFileExist(pathToFile, false);
		}

		public bool Remove(string id)
		{
			try
			{
				var pathToFile = GetPathToFile(id);
				if (CheckFileExist(pathToFile))
				{
					File.Delete(pathToFile);
				}
				return true;
			}
			catch (FileNotFoundException)
			{
				return true;
			}
		}

		public string Save(string extension, Stream stream)
		{
			string fileName;
			if (stream == null || stream.Length == 0)
			{
				throw new ArgumentNullException("Stream is empty");
			}
			CheckDirectory();
			var pathToFile = CreatePathToNewFile(extension, out fileName);
			using (var fileStream = GetFileExist(pathToFile, true))
			{
				stream.Seek(0, SeekOrigin.Begin);
				stream.CopyTo(fileStream);
			}

			return fileName;
		}

		public string Save(string extension, byte[] file)
		{
			string fileName;
			if (file == null || file.Length == 0)
			{
				throw new ArgumentNullException("Stream is empty");
			}
			CheckDirectory();
			var pathToFile = CreatePathToNewFile(extension, out fileName);
			using (var fileStream = GetFileExist(pathToFile, true))
			{
				fileStream.Write(file, 0, file.Length);
			}

			return fileName;
		}

		#region Helper methods
		private void CheckDirectory()
		{
			var isExist = Directory.Exists(_path);
			if (!isExist)
			{
				Directory.CreateDirectory(_path);
			}
		}

		private bool CheckFileExist(string path)
		{
			if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentNullException($"File not found, path is empty");
			}
			return File.Exists(path);
		}

		private FileStream GetFileExist(string path, bool isCreateIfNotExist = false)
		{
			bool isFileExist = CheckFileExist(path);
			if (isFileExist)
			{
				return File.OpenRead(path);
			}
			if (isCreateIfNotExist)
			{
				return File.Create(path);
			}
			else
			{
				throw new FileNotFoundException($"File {path} not found");
			}
		}

		private string GetPathToFile(string fileName)
		{
			return Path.Combine(_path, fileName);
		}

		private string CreatePathToNewFile(string extension, out string fileName)
		{
			fileName = GetNewFilename(extension);
			return Path.Combine(_path, fileName);
		}

		private string GetNewFilename(string extension)
		{
			var id = Guid.NewGuid().ToString();
			return $"{id}.{extension}";
		}
		#endregion
	}
}
