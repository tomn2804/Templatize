﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchemataPreview
{
	public class Pipeline
	{
		public Pipeline(Model model)
		{
			Model = model;
		}

		public static void TraverseReversePostOrder(object key, IEnumerable<Model> children)
		{
			Stack<PipeSegment> segments = new();
			foreach (Model child in children)
			{
				PipeSegment segment = new();
				segments.Push(segment);
				if (child.PipeAssembly[key] is Pipe pipe)
				{
					segment.Extend(pipe);
				}
			}
			foreach (Model child in children)
			{
				if (child.Children != null)
				{
					TraverseReversePostOrder(key, child.Children);
				}
				segments.Pop().Flush();
			}
		}

		public static void TraverseReversePreOrder(object key, IEnumerable<Model> children)
		{
			foreach (Model child in children)
			{
				PipeSegment segment = new();
				if (child.PipeAssembly[key] is Pipe pipe)
				{
					segment.Extend(pipe);
				}
				if (child.Children != null)
				{
					TraverseReversePreOrder(key, child.Children);
				}
				segment.Flush();
			}
		}

		public static void TraverseReversePreOrderParallel(object key, IEnumerable<Model> children)
		{
			List<Task> tasks = new();
			foreach (Model child in children)
			{
				tasks.Add(Task.Run(() =>
				{
					PipeSegment segment = new();
					if (child.PipeAssembly[key] is Pipe pipe)
					{
						segment.Extend(pipe);
					}
					if (child.Children != null)
					{
						TraverseReversePreOrderParallel(key, child.Children);
					}
					segment.Flush();
				}));
			}
			Task.WaitAll(tasks.ToArray());
		}

		public void Invoke(object key)
		{
			PipeSegment segment = new();
			if (Model.PipeAssembly[key] is Pipe pipe)
			{
				segment.Extend(pipe);
			}
			if (Model.Children != null)
			{
				switch (Model.TraversalOption)
				{
					case PipelineTraversalOption.PostOrder:
						TraverseReversePostOrder(key, Model.Children);
						break;

					case PipelineTraversalOption.PreOrder:
					default:
						TraverseReversePreOrder(key, Model.Children);
						break;
				}
			}
			segment.Flush();
		}

		public async Task InvokeAsync(object key)
		{
			await Task.Run(() => Invoke(key));
		}

		public async Task InvokeParallel(object key)
		{
			await Task.Run(() =>
			{
				PipeSegment segment = new();
				if (Model.PipeAssembly[key] is Pipe pipe)
				{
					segment.Extend(pipe);
				}
				if (Model.Children != null)
				{
					TraverseReversePreOrderParallel(key, Model.Children);
				}
				segment.Flush();
			});
		}

		protected Model Model { get; init; }
	}
}
