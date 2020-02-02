using FileManager.Exceptions;
using FileManager.Models.LocalProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileManager.Providers
{
	public class LocalProvider : IFileProvider<FileLocal, ConfigurationLocal>
	{
		public ConfigurationLocal Configuration { get; }

		public LocalProvider(ConfigurationLocal configuration)
		{
			Configuration = configuration;
		}

		public void CreateDirectory(string path)
		{
			var isExist = Directory.Exists(path);
			if (!isExist)
			{
				Directory.CreateDirectory(Configuration.DefaultPath);
			}
		}

		public void CheckFileExist(string path, bool isCreateIfNotExist = false)
		{
			bool isFileExist = File.Exists(path);
			if (isFileExist)
			{
				return;
			}
			if (isCreateIfNotExist)
			{
				File.Create(path);
			}
			else
			{
				throw new FileNotFoundException($"File {path} not found");
			}
		}

		public string CreatePathToNewFile(FileLocal ent)
		{
			return Path.Combine(Configuration.DefaultPath, ent.Name + "." + ent.Extension);
		}

		public byte[] Get(FileLocal ent)
		{
			CheckFileExist(ent.Path);
			using (var stream = GetStream(ent))
			{
				var bytes = new byte[stream.Length];
				stream.Read(bytes, 0, (int)stream.Length);

				return bytes;
			}
		}

		public Stream GetStream(FileLocal ent)
		{
			CheckFileExist(ent.Path);
			return new FileStream(ent.Path, FileMode.Open);
		}

		public IEnumerable<FileLocal> List(IEnumerable<FileLocal> ents)
		{
			foreach (var file in ents)
			{
				file.Content = Get(file);
			}

			return ents;
		}

		public FileLocal Remove(FileLocal ent)
		{
			return RemoveRange(new FileLocal[] { ent }).FirstOrDefault();
		}

		public IEnumerable<FileLocal> RemoveRange(IEnumerable<FileLocal> ents)
		{
			foreach (var file in ents)
			{
				try
				{
					CheckFileExist(file.Path);
					File.Delete(file.Path);
				}
				catch (FileNotFoundException ex)
				{
					continue;
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			return ents;
		}

		public FileLocal Rename(FileLocal ent, string newName)
		{
			CheckFileExist(ent.Path);
			ent.Name = Path.GetFileNameWithoutExtension(newName);
			File.Move(ent.Path, CreatePathToNewFile(ent));
			return ent;
		}

		public FileLocal Save(FileLocal ent)
		{
			if (ent.Content == null)
			{
				throw new ArgumentNullException($"File content is null");
			}
			CreateDirectory(Configuration.DefaultPath);
			var path = CreatePathToNewFile(ent);
			if (File.Exists(path))
			{
				throw new FileDuplicateException($"File with name {ent.Name}.{ent.Extension} already exist");
			}
			CheckFileExist(path, true);

			File.WriteAllBytes(path, ent.Content);
			ent.Path = path;
			return ent;
		}
	}
}
