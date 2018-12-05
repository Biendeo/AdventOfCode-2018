import os

for day in range(6, 26):
	for part in range(1, 3):
		os.mkdir("Day{}-{}".format(day, part))
		with open("Day{}-{}/Day{}-{}.csproj".format(day, part, day, part), "w") as proj:
			proj.write("""<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.1</TargetFramework>
    <RootNamespace>Day{}_{}</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\Common\Common.csproj" />
  </ItemGroup>

</Project>
			""".format(day, part))
		with open("Day{}-{}/Program.cs".format(day, part), "w") as cs:
			cs.write("""using Common.DayARGH;
using System;

namespace DayARGH_BLEG {
	class Program {
		static void Main(string[] args) {
			// Uncomment when task done.
			// var aoc = new Something();
			// Console.WriteLine(aoc.PartBLEG());
		}
	}
}
			""".replace("ARGH", str(day)).replace("BLEG", str(part)))