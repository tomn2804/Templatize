﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;

namespace SchemataPreview
{
	public class StrictDirectoryModel : DirectoryModel
	{
		public override void Format()
		{
			base.Format();
			foreach (string path in Directory.EnumerateFiles(AbsolutePath))
			{
				if (!Children.Contains(Path.GetFileName(path)))
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
		}
	}
}
