# ZipHash

Use the ZipHash tool when you need to generate checksum hashes for the contents and entries of zip file(s).

Run it as a standalone executable. The tool supports 3 hash formats: SFV, MD5 and SHA1.

## Download

To get a copy of ZipHash you have the following options:

* Download the source and run the `build.cmd` (which runs an MSBuild against `build.proj`)
* ***NuGet package coming soon***

### Prerequisites

To use ZipHash you will need to have the [.NET Framework 4.0](http://www.microsoft.com/en-GB/download/details.aspx?id=17851) installed.

## Command line arguments

	ziphash.exe foo.zip [--options]

Valid options are:

	--pattern                     File pattern to match when searching a directory. Default value is "*.zip".
	--algorithms                  Types of checksum algorithm to use, the options are: "sfv", "md5" and "sha1".
	--verbose                     Print detail messages.

## References
This version can be found at https://github.com/leekelleher/ZipHash.NET

## License
Copyright &copy; 2013 Lee Kelleher<br/>

This project is licensed under [MIT](http://opensource.org/licenses/MIT).

Please see [LICENSE](LICENSE) for further details.
