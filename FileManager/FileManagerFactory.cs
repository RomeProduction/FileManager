using FileManager.Models;
using FileManager.Models.LocalProvider;
using FileManager.Providers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileManager
{
	public class FileManagerFactory<TConfiguration>
	{
		public static IFileManager<TFile> GetManager<TFile>(TConfiguration configuration, ILogger logger = null)
			where TFile : FileBase, new()
		{
			if (typeof(TFile) == typeof(FileLocal))
			{
				var conf = configuration as ConfigurationLocal;
				if (conf == null)
				{
					throw new Exception($"Not correct configuration for {nameof(TFile)}, expected {nameof(ConfigurationLocal)}");
				}
				var fileManager = new FileManager<TFile, ConfigurationLocal>(new LocalProvider<TFile>(), conf, logger);
				return fileManager;
			}
			else
			{
				throw new ArgumentException();
			}
		}
	}

	public class FileManager<TFile, TConfiguration> : IFileManager<TFile>
		where TFile : FileBase, new()
	{
		IFileProvider<TFile, TConfiguration> _fileProvider;
		ILogger _logger;
		public FileManager(IFileProvider<TFile, TConfiguration> provider, TConfiguration configuration)
		{
			_fileProvider = provider;
			provider.Init(configuration);
		}
		public FileManager(IFileProvider<TFile, TConfiguration> provider, TConfiguration configuration, ILogger logger)
			: this(provider, configuration)
		{
			_logger = logger;
		}

		public byte[] GetBytes(string filePath)
		{
			try
			{
				var fileInf = _fileProvider.GetFile(filePath, false);
				return _fileProvider.GetBytes(fileInf);
			}
			catch (Exception ex)
			{
				_logger?.LogError("[FileManager] " + $" {nameof(GetBytes)}=>", ex);
				throw ex;
			}
		}

		public TFile GetFile(string filePath, bool withFile)
		{
			return List(new[] { filePath }, withFile).FirstOrDefault();
		}

		public Stream GetStream(string filePath)
		{
			try
			{
				var fileInf = _fileProvider.GetFile(filePath, false);
				return _fileProvider.GetStream(fileInf);
			}
			catch (Exception ex)
			{
				_logger?.LogError("[FileManager] " + $" {nameof(GetStream)}=>", ex);
				throw ex;
			}
		}

		public IEnumerable<TFile> List(IEnumerable<string> filePathes, bool withFiles)
		{
			try
			{
				List<TFile> files = new List<TFile>();
				foreach (var filePath in filePathes)
				{
					var file = _fileProvider.GetFile(filePath, withFiles);
					files.Add(file);
				}
				return files;
			}
			catch (Exception ex)
			{
				_logger?.LogError("[FileManager] " + $" {nameof(List)}=>", ex);
				throw ex;
			}
		}

		public TFile Remove(string filePath)
		{
			return RemoveRange(new List<string>() { filePath }).FirstOrDefault();
		}

		public IEnumerable<TFile> RemoveRange(IEnumerable<string> filePathes)
		{
			try
			{
				List<TFile> removedFiles = new List<TFile>();
				foreach (var filePath in filePathes)
				{
					var file = _fileProvider.GetFile(filePath, false);
					removedFiles.Add(file);
				}
				return _fileProvider.RemoveRange(removedFiles);
			}
			catch (Exception ex)
			{
				_logger?.LogError("[FileManager] " + $" {nameof(RemoveRange)}=>", ex);
				throw ex;
			}
		}

		public bool Rename(string filePath, string newName)
		{
			try
			{
				var file = _fileProvider.GetFile(filePath, false);
				file = _fileProvider.Rename(file, newName);
				return true;
			}
			catch (Exception ex)
			{
				_logger?.LogError("[FileManager] " + $" {nameof(Rename)}=>", ex);
				throw ex;
			}
		}

		public TFile Save(TFile file)
		{
			try
			{
				return _fileProvider.Save(file);
			}
			catch (Exception ex)
			{
				_logger?.LogError("[FileManager] " + $" {nameof(Save)}=>", ex);
				throw ex;
			}
		}
	}
}
