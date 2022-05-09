﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Layoutize.Elements;

internal abstract partial class ViewGroupElement : ViewElement
{
    private static UpdateComparer Comparer { get; } = new();

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

    internal override void MountTo(Element? parent)
    {
        Debug.Assert(!IsDisposed);
        base.MountTo(parent);
        foreach (Element child in Children)
        {
            child.MountTo(this);
        }
        Debug.Assert(Children.All(child => child.IsMounted));
        Debug.Assert(Children.All(child => child.Parent == this));
    }

    internal override void Unmount()
    {
        Debug.Assert(!IsDisposed);
        foreach (Element child in Children)
        {
            if (child.IsMounted) child.Unmount();
        }
        Debug.Assert(Children.All(child => !child.IsMounted));
        Debug.Assert(Children.All(child => child.Parent == null));
        base.Unmount();
    }

    private protected ViewGroupElement(ViewGroupLayout layout)
        : base(layout)
    {
        _children = new(() => GetChildrenAttribute());
    }

    private protected override void OnLayoutUpdated(EventArgs e)
    {
        Debug.Assert(!IsDisposed);
        Debug.Assert(IsMounted);
        ImmutableSortedSet<Element>.Builder newChildrenBuilder = ImmutableSortedSet.CreateBuilder<Element>();
        foreach (Element newChild in GetChildrenAttribute())
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

    private ImmutableSortedSet<Element> GetChildrenAttribute()
    {
        Debug.Assert(!IsDisposed);
        if (Layout.Attributes.TryGetValue("Children", out object? childrenObject))
        {
            switch (childrenObject)
            {
                case IEnumerable<object> children:
                    return children.Cast<Layout>().Select(childLayout => childLayout.CreateElement()).ToImmutableSortedSet();

                default:
                    return ImmutableSortedSet.Create(((Layout)childrenObject).CreateElement());
            }
        }
        return ImmutableSortedSet<Element>.Empty;
    }
}

internal abstract partial class ViewGroupElement
{
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
                enteringChild.MountTo(this);
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

    private Lazy<ImmutableSortedSet<Element>> _children;
}
