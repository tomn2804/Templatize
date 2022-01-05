﻿using System;
using System.Collections.Generic;

namespace Templata;

public sealed partial class Blueprint
{
    public IReadOnlyDictionary<object, object> Details { get; private init; } = null!;

    public Type ModelType { get; private init; } = null!;

    public string Path { get; private init; } = null!;

    internal Builder ToBuilder()
    {
        return new(this);
    }

    private Blueprint()
    {
    }

    private IReadOnlyList<Template> Templates { get; init; } = null!;
}