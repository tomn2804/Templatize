﻿using Microsoft.VisualBasic.FileIO;
using System.IO;

#nullable enable

namespace SchemataPreview
{
	public class FileModel : Model
	{
		public override bool Exists => File.Exists(AbsolutePath);
		public override ModelList? Children => null;

		public override void Create()
		{
			base.Create();
			File.Create(AbsolutePath).Dispose();
		}

		public override void Delete()
		{
			base.Delete();
			FileSystem.DeleteFile(AbsolutePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
		}
	}
}

#nullable disable
