namespace FileManager.Models
{
	public class FileBase
	{
		public string Path { get; set; }
		public string Name { get; set; }
		public string Extension { get; set; }
		public byte[] Content { get; set; }
	}
}
