using FileManager.Exceptions;
using FileManager.Models;
using FileManager.Models.LocalProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileManager.Providers
{
	public class LocalProvider<TFile> : IFileProvider<TFile, ConfigurationLocal>
		where TFile : FileBase, new()
	{
		public ConfigurationLocal Configuration { get; private set; }

		public LocalProvider() { }

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
			if(string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentNullException($"File not found, path is empty");
			}
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

		public string CreatePathToNewFile(TFile ent)
		{
			return Path.Combine(Configuration.DefaultPath, ent.Name + "." + ent.Extension);
		}

		public byte[] GetBytes(TFile ent)
		{
			CheckFileExist(ent.Path);
			using (var stream = GetStream(ent))
			{
				var bytes = new byte[stream.Length];
				stream.Read(bytes, 0, (int)stream.Length);

				return bytes;
			}
		}

		public Stream GetStream(TFile ent)
		{
			CheckFileExist(ent.Path);
			return new FileStream(ent.Path, FileMode.Open);
		}

		public IEnumerable<TFile> List(IEnumerable<TFile> ents)
		{
			foreach (var file in ents)
			{
				file.Content = GetBytes(file);
			}

			return ents;
		}

		public TFile Remove(TFile ent)
		{
			return RemoveRange(new TFile[] { ent }).FirstOrDefault();
		}

		public IEnumerable<TFile> RemoveRange(IEnumerable<TFile> ents)
		{
			foreach (var file in ents)
			{
				try
				{
					CheckFileExist(file.Path);
					File.Delete(file.Path);
				}
				catch (FileNotFoundException)
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

		public TFile Rename(TFile ent, string newName)
		{
			CheckFileExist(ent.Path);
			ent.Name = Path.GetFileNameWithoutExtension(newName);
			File.Move(ent.Path, CreatePathToNewFile(ent));
			return ent;
		}

		public TFile Save(TFile ent)
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

		public TFile GetFile(string path, bool withFile = true)
		{
			CheckFileExist(path);
			var file = new TFile()
			{
				Path = path,
				Name = Path.GetFileNameWithoutExtension(path),
				Extension = Path.GetExtension(path),
			};
			if (withFile)
			{
				file.Content = GetBytes(file);
			}
			return file;
		}

		public bool Init(ConfigurationLocal config)
		{
			Configuration = config;
			return true;
		}	
	}
}
