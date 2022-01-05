﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Templata;

public sealed partial class Blueprint
{
    internal sealed class Builder
    {
        internal Builder()
        {
            Path = Directory.GetCurrentDirectory();
            Templates = new();
        }

        internal Builder(Blueprint blueprint)
        {
            Path = blueprint.Path;
            Templates = blueprint.Templates.ToList();
            Debug.Assert(blueprint.Details == Details);
            Debug.Assert(blueprint.ModelType == ModelType);
        }

        internal IReadOnlyDictionary<object, object> Details => Templates.FirstOrDefault()?.Details ?? ImmutableDictionary.Create<object, object>();

        internal Type ModelType => Templates.LastOrDefault()?.ModelType ?? typeof(Model);

        internal string Path { get; set; }

        internal List<Template> Templates { get; }

        internal Blueprint ToBlueprint()
        {
            return new() { Details = Details, ModelType = ModelType, Path = Path, Templates = Templates.ToImmutableList() };
        }
    }
}