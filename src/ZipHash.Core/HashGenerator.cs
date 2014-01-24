using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace ZipHash.Core
{
	public class HashGenerator
	{
		private FileInfo _file;
		private Dictionary<string, HashAlgorithm> _algorithms;

		public HashGenerator(string file, IEnumerable<HashAlgorithm> algorithms)
			: this(new FileInfo(file), algorithms)
		{
		}

		public HashGenerator(FileInfo file, IEnumerable<HashAlgorithm> algorithms)
		{
			_file = file;
			_algorithms = algorithms.ToDictionary(x => x.GetType().Name.Replace("CryptoServiceProvider", string.Empty).ToLower(), x => x);
		}

		public void GenerateHashes()
		{
			GenerateHashesForZipFile();
			GenerateHashesForZipEntries();
		}

		internal void GenerateHashesForZipFile()
		{
			var path = _file.FullName;
			var ext = _file.Extension;

			GetComputedHash()
				.Where(x => !File.Exists(string.Concat(path, ".", x.Key)))
				.All(x =>
				{
					File.WriteAllText(string.Concat(path, ".", x.Key), x.Value);
					return true;
				});
		}

		internal void GenerateHashesForZipEntries()
		{
			var path = _file.FullName;
			var ext = _file.Extension;

			GetComputedHashes()
				.Where(x => !File.Exists(path.Replace(ext, "." + x.Key)))
				.All(x =>
				{
					File.WriteAllLines(path.Replace(ext, "." + x.Key), x.Value);
					return true;
				});
		}

		internal Dictionary<string, string> GetComputedHash(byte[] bytes = null)
		{
			if (bytes == null)
				bytes = File.ReadAllBytes(_file.FullName);

			return _algorithms
				.ToDictionary(x => x.Key, x => ConvertHash(x.Value.ComputeHash(bytes)));
		}

		internal Dictionary<string, string[]> GetComputedHashes(ZipFile zipFile = null)
		{
			if (zipFile == null)
				zipFile = new ZipFile(_file.FullName);

			try
			{
				return _algorithms
				.ToDictionary(
					x => x.Key,
					x => zipFile
						.Cast<ZipEntry>()
						.Select(e => string.Format("{1}  {0}", e.Name, ConvertHash(x.Value, zipFile.GetInputStream(e))))
						.ToArray());
			}
			finally
			{
				zipFile.Close();
			}
		}

		internal string ConvertHash(HashAlgorithm algorithm, ZipFile zipFile, ZipEntry entry)
		{
			return ConvertHash(algorithm, zipFile.GetInputStream(entry));
		}

		internal string ConvertHash(HashAlgorithm algorithm, Stream stream)
		{
			return ConvertHash(algorithm.ComputeHash(stream));
		}

		internal string ConvertHash(byte[] hash)
		{
			return BitConverter
				.ToString(hash)
				.Replace("-", string.Empty)
				.ToLower();
		}
	}
}
