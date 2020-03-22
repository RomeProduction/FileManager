using FileManager.Models;
using System.Collections.Generic;
using System.IO;

namespace FileManager
{
	public interface IFileProvider
	{
	}
	public interface IFileProvider<TFile, TConfiguration> : IFileProvider
		where TFile : FileBase, new()
	{
		TConfiguration Configuration { get; }

		bool Init(TConfiguration config);
		IEnumerable<TFile> List(IEnumerable<TFile> files);
		byte[] GetBytes(TFile file);
		Stream GetStream(TFile file);
		TFile GetFile(string path, bool withFile = true);
		TFile Save(TFile file);
		TFile Remove(TFile file);
		IEnumerable<TFile> RemoveRange(IEnumerable<TFile> files);
		TFile Rename(TFile file, string newName);
		string CreatePathToNewFile(TFile file);
	}
}
