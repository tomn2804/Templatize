﻿
using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;

namespace Schemata
{
	public class StrictDirectoryModel : DirectoryModel
	{
		public StrictDirectoryModel(ImmutableDefinition Outline)
			: base(outline)
		{
			PipeAssembly.Register(PipeOption.Format).OnProcessing += (_, _) =>
			{
				foreach (string path in Directory.EnumerateFiles(FullName))
				{
					if (!Children.ContainsName(Path.GetFileName(path)))
					{
						try
						{
							if (Directory.Exists(path))
							{
								FileSystem.DeleteDirectory(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
							}
							else
							{
								FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
							}
						}
						catch (Exception e)
						{
							Console.WriteLine($"Error: {e}");
						}
					}
				}
			};
		}
	}
}