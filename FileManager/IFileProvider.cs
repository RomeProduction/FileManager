using System.Collections.Generic;
using System.IO;

namespace FileManager
{
	public interface IFileProvider
	{

	}
	public interface IFileProvider<TFile, TConfiguration> : IFileProvider
		where TFile :class
	{
		TConfiguration Configuration { get; }

		IEnumerable<TFile> List(IEnumerable<TFile> files);
		byte[] Get(TFile file);
		Stream GetStream(TFile file);
		TFile Save(TFile file);
		TFile Remove(TFile file);
		IEnumerable<TFile> RemoveRange(IEnumerable<TFile> files);
		TFile Rename(TFile file, string newName);
		string CreatePathToNewFile(TFile file);
	}
}
