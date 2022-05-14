﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Layoutize.Elements;

internal abstract partial class ViewGroupElement : ViewElement
{
    private protected ViewGroupElement(ViewGroupLayout layout)
        : base(layout)
    {
        _children = new(() => Attributes.Children.Of(this)?.Select(layout => layout.CreateElement()).ToImmutableSortedSet() ?? ImmutableSortedSet<Element>.Empty);
    }

    internal override bool IsMounted
    {
        get
        {
            if (base.IsMounted)
            {
                Debug.Assert(Children.All(child => child.IsMounted));
                Debug.Assert(Children.All(child => child.Parent == this));
                return true;
            }
            return false;
        }
    }

    internal ViewGroupLayout ViewGroupLayout => (ViewGroupLayout)Layout;

    private protected override void OnLayoutUpdated(EventArgs e)
    {
        Debug.Assert(!IsDisposed);
        Debug.Assert(IsMounted);
        ImmutableSortedSet<Element>.Builder newChildrenBuilder = ImmutableSortedSet.CreateBuilder<Element>();
        foreach (Element newChild in Attributes.Children.Of(this)?.Select(layout => layout.CreateElement()).ToImmutableSortedSet() ?? ImmutableSortedSet<Element>.Empty)
        {
            if (Children.TryGetValue(newChild, out Element? currentChild) && Comparer.Equals(currentChild, newChild))
            {
                currentChild.Layout = newChild.Layout;
                newChildrenBuilder.Add(currentChild);
            }
            else
            {
                newChildrenBuilder.Add(newChild);
            }
        }
        if (newChildrenBuilder.Any())
        {
            Children = newChildrenBuilder.ToImmutable();
        }
        base.OnLayoutUpdated(e);
    }

    private protected override void OnMounting(EventArgs e)
    {
        base.OnMounting(e);
        Debug.Assert(!IsDisposed);
        foreach (Element child in Children)
        {
            child.Mount(this);
        }
        Debug.Assert(Children.All(child => child.IsMounted));
        Debug.Assert(Children.All(child => child.Parent == this));
    }

    private protected override void OnUnmounting(EventArgs e)
    {
        base.OnUnmounting(e);
        Debug.Assert(!IsDisposed);
        foreach (Element child in Children)
        {
            if (child.IsMounted)
            {
                child.Unmount();
            }
        }
        Debug.Assert(Children.All(child => !child.IsMounted));
        Debug.Assert(Children.All(child => child.Parent == null));
    }
}

internal abstract partial class ViewGroupElement
{
    private Lazy<ImmutableSortedSet<Element>> _children;

    internal event EventHandler? ChildrenUpdated;

    internal event EventHandler? ChildrenUpdating;

    internal ImmutableSortedSet<Element> Children
    {
        get => _children.Value;
        private protected set
        {
            Debug.Assert(!IsDisposed);
            Debug.Assert(IsMounted);
            OnChildrenUpdating(EventArgs.Empty);
            IEnumerable<Element> enteringChildren = value.Except(Children);
            IEnumerable<Element> exitingChildren = Children.Except(value);
            foreach (Element exitingChild in exitingChildren)
            {
                exitingChild.Unmount();
            }
            Debug.Assert(exitingChildren.All(child => !child.IsMounted));
            Debug.Assert(exitingChildren.All(child => child.Parent == null));
            _children = new(() => value);
            foreach (Element enteringChild in enteringChildren)
            {
                enteringChild.Mount(this);
            }
            Debug.Assert(Children.All(child => child.IsMounted));
            Debug.Assert(Children.All(child => child.Parent == this));
            OnChildrenUpdated(EventArgs.Empty);
        }
    }

    private protected virtual void OnChildrenUpdated(EventArgs e)
    {
        Debug.Assert(!IsDisposed);
        Debug.Assert(IsMounted);
        ChildrenUpdated?.Invoke(this, e);
    }

    private protected virtual void OnChildrenUpdating(EventArgs e)
    {
        Debug.Assert(!IsDisposed);
        Debug.Assert(IsMounted);
        ChildrenUpdating?.Invoke(this, e);
    }
}
