using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Zip;

namespace ZipHash
{
	class Program
	{
		const int EXITCODE_ERROR = 1;
		const string EXTENSION_MD5 = ".md5";
		const string EXTENSION_SFV = ".sfv";
		const string EXTENSION_SHA1 = ".sha1";
		const string EXTENSION_ZIP = ".zip";

		// TODO: [LK] Explore the other options
		// TODO: [LK] Separate out into a ZipHash.Core project - then can be used programatically

		static Options options { get; set; }

		static void Main(string[] args)
		{
			options = new Options();
			if (!CommandLine.Parser.Default.ParseArguments(args, options))
			{
				Console.WriteLine("No zip file(s) have been specified.");
				Environment.Exit(EXITCODE_ERROR);
			}

			var files = GetFiles(options.Path, options.FilePattern);

			if (files != null)
			{
				foreach (var file in files)
				{
					CheckFile(file);
					GenerateHashes(file);
				}
			}
		}

		static FileInfo[] GetFiles(string path, string pattern = "*.zip", SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			var attr = File.GetAttributes(path);

			if (attr.HasFlag(FileAttributes.Directory))
			{
				var dir = new DirectoryInfo(path);
				if (dir.Exists)
					return dir.GetFiles(pattern, searchOption);
			}

			if (File.Exists(path))
				return new[] { new FileInfo(path) };

			return null;
		}

		static void CheckFile(FileInfo file)
		{
			if (!file.Exists)
			{
				Console.WriteLine("'{0}' does not exist.", file.Name);
				Environment.Exit(EXITCODE_ERROR);
			}

			if (file.Attributes.HasFlag(FileAttributes.Directory))
			{
				Console.WriteLine("'{0}' is a directory.", file.Name);
				Environment.Exit(EXITCODE_ERROR);
			}

			Console.WriteLine("File exists: '{0}'", file.Name);
		}

		static void GenerateHashes(FileInfo file)
		{
			var generator = new ZipHash.Core.HashGenerator(file, new HashAlgorithm[] { MD5.Create(), SHA1.Create() });
			generator.GenerateHashes();


			//var path = file.FullName;
			//var ext = file.Extension;

			//var bytes = File.ReadAllBytes(path);
			//var zipfile = new ZipFile(path);

			//using (var md5Crypt = MD5.Create())
			//using (var sha1Crypt = SHA1.Create())
			//{
			//	var outputMd5 = options.HashAlgorithms.Contains("md5");
			//	var extMd5 = string.Concat(ext, EXTENSION_MD5);
			//	if (outputMd5 && !File.Exists(path.Replace(ext, extMd5)))
			//	{
			//		File.WriteAllText(path.Replace(ext, extMd5), ConvertHash(md5Crypt.ComputeHash(bytes)));
			//		if (options.Verbose)
			//			Console.WriteLine("Generated MD5 checksum for '{0}'.", file.Name);
			//	}

			//	var outputSha1 = options.HashAlgorithms.Contains("sha1");
			//	var extSha1 = string.Concat(ext, EXTENSION_SHA1);
			//	if (outputSha1 && !File.Exists(path.Replace(ext, extSha1)))
			//	{
			//		File.WriteAllText(path.Replace(ext, extSha1), ConvertHash(sha1Crypt.ComputeHash(bytes)));
			//		if (options.Verbose)
			//			Console.WriteLine("Generated SHA1 checksum for '{0}'.", file.Name);
			//	}

			//	Console.WriteLine();

			//	var outputSfv = options.HashAlgorithms.Contains("sfv");
			//	var outputMd5sum = options.HashAlgorithms.Contains("md5");
			//	var outputSha1sum = options.HashAlgorithms.Contains("sha1");

			//	var sfvHashes = new List<string>();
			//	var md5Hashes = new List<string>();
			//	var sha1Hashes = new List<string>();

			//	for (int i = 0; i < zipfile.Count; i++)
			//	{
			//		var entry = zipfile[i];

			//		if (options.Verbose)
			//			Console.WriteLine("Processing zip entry: '{0}'.", entry.Name);

			//		if (outputSfv && !File.Exists(path.Replace(ext, EXTENSION_SFV)))
			//		{
			//			sfvHashes.Add(string.Format("{0} {1:X8}", entry.Name, entry.Crc));
			//			if (options.Verbose)
			//				Console.WriteLine("Extracting CRC32 from zip entry: '{0}'.", entry.Name);
			//		}

			//		if (outputMd5sum && !File.Exists(path.Replace(ext, EXTENSION_MD5)))
			//		{
			//			md5Hashes.Add(string.Format("{1} {0}", entry.Name, ConvertHash(md5Crypt, zipfile, entry)));
			//			if (options.Verbose)
			//				Console.WriteLine("Generating MD5 for zip entry: '{0}'.", entry.Name);
			//		}

			//		if (outputSha1sum && !File.Exists(path.Replace(ext, EXTENSION_SHA1)))
			//		{
			//			sha1Hashes.Add(string.Format("{1} {0}", entry.Name, ConvertHash(sha1Crypt, zipfile, entry)));
			//			if (options.Verbose)
			//				Console.WriteLine("Generating SHA1 for zip entry: '{0}'.", entry.Name);
			//		}
			//	}

			//	Console.WriteLine();

			//	if (outputSfv && !File.Exists(path.Replace(ext, EXTENSION_SFV)))
			//	{
			//		File.WriteAllLines(path.Replace(ext, EXTENSION_SFV), sfvHashes);
			//		if (options.Verbose)
			//			Console.WriteLine("Writing SFV checksums for '{0}'.", file.Name);
			//	}

			//	if (outputMd5sum && !File.Exists(path.Replace(ext, EXTENSION_MD5)))
			//	{
			//		File.WriteAllLines(path.Replace(ext, EXTENSION_MD5), md5Hashes);
			//		if (options.Verbose)
			//			Console.WriteLine("Writing MD5 checksums for '{0}'.", file.Name);
			//	}

			//	if (outputSha1sum && !File.Exists(path.Replace(ext, EXTENSION_SHA1)))
			//	{
			//		File.WriteAllLines(path.Replace(ext, EXTENSION_SHA1), sha1Hashes);
			//		if (options.Verbose)
			//			Console.WriteLine("Writing SHA1 checksums for '{0}'.", file.Name);
			//	}
			//}

			Console.WriteLine("Checksum generation for '{0}' is complete.", file.Name);
		}

		static string ConvertHash(HashAlgorithm algorithm, ZipFile zipFile, ZipEntry entry)
		{
			return ConvertHash(algorithm, zipFile.GetInputStream(entry));
		}

		static string ConvertHash(HashAlgorithm algorithm, Stream stream)
		{
			return ConvertHash(algorithm.ComputeHash(stream));
		}

		static string ConvertHash(byte[] hash)
		{
			return BitConverter
				.ToString(hash)
				.Replace("-", string.Empty)
				.ToLower();
		}
	}
}