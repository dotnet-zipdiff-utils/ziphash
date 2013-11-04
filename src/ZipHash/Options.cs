using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace ZipHash
{
	public class Options
	{
		[ValueOption(0)]
		public string Path { get; set; }

		[Option('p', "pattern", Required = false, DefaultValue = "*.zip", HelpText = "File pattern")]
		public string FilePattern { get; set; }

		[OptionList('a', "algorithms", ';', DefaultValue = new[] { "sfv", "md5", "sha1" })]
		public IList<string> HashAlgorithms { get; set; }

		// TODO: [LK] Option to omit empty values (e.g. directories)

		[Option('v', "verbose", Required = false, HelpText = "Verbose mode")]
		public bool Verbose { get; set; }
	}
}