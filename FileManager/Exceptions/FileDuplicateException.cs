using System;
using System.Runtime.Serialization;

namespace FileManager.Exceptions
{
	public class FileDuplicateException : Exception
	{
		public FileDuplicateException()
		{
		}

		public FileDuplicateException(string message) : base(message)
		{
		}

		public FileDuplicateException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected FileDuplicateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
