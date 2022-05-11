﻿using Layoutize.Elements;
using System.Diagnostics;
using System.Management.Automation;

namespace Layoutize.Attributes;

internal static class OnDeleting
{
    internal static ScriptBlock? Of(IBuildContext context)
    {
        Element element = context.Element;
        Debug.Assert(!element.IsDisposed);
        return Of(element.Layout);
    }

    internal static ScriptBlock? Of(Layout layout)
    {
        return layout.GetValue<ScriptBlock?>(nameof(OnDeleting));
    }

    internal static ScriptBlock RequireOf(IBuildContext context)
    {
        Element element = context.Element;
        Debug.Assert(!element.IsDisposed);
        return RequireOf(element.Layout);
    }

    internal static ScriptBlock RequireOf(Layout layout)
    {
        return layout.RequireValue<ScriptBlock>(nameof(OnDeleting));
    }
}
