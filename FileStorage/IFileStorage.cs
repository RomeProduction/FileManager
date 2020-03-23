using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileStorage
{
	public interface IFileStorage<TKey>
	{
		TKey Save(string extension, Stream stream);
		TKey Save(string extension, byte[] file);
		Stream GetStream(TKey id);
		byte[] GetBytes(TKey id);
		bool Remove(TKey id);
	}
}
