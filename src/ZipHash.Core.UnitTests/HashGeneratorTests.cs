using System;
using System.Linq;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;

namespace ZipHash.Core.UnitTests
{
	[TestClass]
	public class HashGeneratorTests
	{
		[TestClass]
		public class GenerateHashes
		{
			const string ZipOneEntryA1 = "zip-one-entry-a1.zip";

			[TestInitialize]
			public void Init()
			{
				this.CreateZipOneEntry(ZipOneEntryA1);
			}

			[TestCleanup]
			public void Cleanup()
			{
				var zips = new[] { ZipOneEntryA1 };
				foreach (var zip in zips.Where(File.Exists))
					File.Delete(zip);
			}

			[TestMethod]
			public void HashGenerator_GenerateHashesForZipFile()
			{
				var file = new FileInfo(ZipOneEntryA1);
				var algorithms = new HashAlgorithm[] { MD5.Create() };
				var generator = new HashGenerator(file, algorithms);

				var first = generator.GetComputedHash();

				var bytes = File.ReadAllBytes(ZipOneEntryA1);
				var second = generator.GetComputedHash(bytes);

				foreach (var hash in first)
				{
					Assert.AreEqual(hash.Value, second[hash.Key]);
				}
			}

			[TestMethod]
			public void HashGenerator_GenerateHashesForZipEntries()
			{
				var file = new FileInfo(ZipOneEntryA1);
				var algorithms = new HashAlgorithm[] { MD5.Create() };

				var generator = new HashGenerator(file, algorithms);

				var hashes = generator.GetComputedHashes();

				foreach (var hash in hashes)
				{
					Assert.AreEqual(hash.Value.FirstOrDefault(), "c9a34cfc85d982698c6ac89f76071abd A");
				}
			}

			private void CreateZipOneEntry(string filename, char c = 'a')
			{
				using (var writer = new StreamWriter(filename))
				{
					var testZipOS = new ZipOutputStream(writer.BaseStream);

					var entry1 = new ZipEntry(char.ToUpper(c).ToString());
					testZipOS.PutNextEntry(entry1);

					var data = new string(c, 1024);
					var bytes = UTF8Encoding.UTF8.GetBytes(data);
					foreach (var bit in bytes)
						testZipOS.WriteByte(bit);

					testZipOS.Finish();
					testZipOS.Flush();
				}
			}
		}
	}
}