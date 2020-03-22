using FileManager.Models;
using System.Collections.Generic;
using System.IO;

namespace FileManager
{
	public interface IFileManager <TFile>
		where TFile : FileBase
	{
		IEnumerable<TFile> List(IEnumerable<string> filePathes, bool withFiles);
		TFile GetFile(string filePath, bool withFile);
		byte[] GetBytes(string filePath);
		Stream GetStream(string filePath);
		TFile Save(TFile file);
		TFile Remove(string filePath);
		IEnumerable<TFile> RemoveRange(IEnumerable<string> filePathes);
		bool Rename(string filePath, string newName);
	}
}
