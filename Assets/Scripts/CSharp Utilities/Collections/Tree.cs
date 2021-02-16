using System;
using System.Collections.Generic;

namespace CSharpUtilities.Collections
{
    /// <summary>
    /// A collection of elements that is navigated via hierarchy.
    /// </summary>
    public sealed class Tree<T>
    {
        #region Branch Data Class
        private sealed class Branch
        {
            public Branch parent;
            public T value;
            public List<Branch> children;
        }
        #endregion
        #region State Fields
        private readonly Branch root;
        private Branch location;
        private Branch savedLocation;
        private bool savedLocationWasRemoved;
        #endregion
        #region Constructor
        /// <summary>
        /// Creates a new tree with the given immutable root element.
        /// </summary>
        /// <param name="root">The root element of the tree.</param>
        public Tree(T root)
        {
            // Create the root and set it as the start
            // point for tree navigation.
            this.root = new Branch
            {
                parent = null,
                value = root,
                children = new List<Branch>()
            };
            location = this.root;
            IsAtRoot = true;
            // Set the exposed easy access properties.
            Current = this.root.value;
            Children = ChildrenToValues(this.root);
            Endpoints = 1;
            // Initialize logical state for traversal.
            savedLocation = null;
            savedLocationWasRemoved = false;
        }
        #endregion
        #region State Accessor Properties
        // These states are set every time a method
        // is invoked that changes them.
        /// <summary>
        /// True when the current location is the tree root.
        /// </summary>
        public bool IsAtRoot { get; private set; }
        /// <summary>
        /// The total number of child endpoints on the tree.
        /// </summary>
        public int Endpoints { get; private set; }
        /// <summary>
        /// The object at the current tree location.
        /// </summary>
        public T Current { get; private set; }
        /// <summary>
        /// The child objects at the current tree location.
        /// </summary>
        public List<T> Children { get; private set; }
        #endregion
        #region Utility Functions
        // Used to grab the values located in the
        // children of a branch.
        private List<T> ChildrenToValues(Branch branch)
        {
            List<T> values = new List<T>();
            foreach (Branch child in branch.children)
                values.Add(child.value);
            return values;
        }
        // Used to locate a child branch based on the
        // provided value that may or may not exist
        // on a child branch.
        private Branch FindChildByValue(T value)
        {
            bool foundChild = false;
            int i;
            for (i = 0; i < Children.Count; i++)
            {
                if (Children[i].Equals(value))
                {
                    foundChild = true;
                    break;
                }
            }
            if (!foundChild)
                throw new ArgumentOutOfRangeException("child",
                    $"Requested child was not found at the current tree location.");
            else
                return location.children[i];
        }
        // Used to set the accessor properties when moving
        // to another location in the tree.
        private void StepIn(Branch branch)
        {
            location = branch;
            IsAtRoot = false;
            Current = location.value;
            Children = ChildrenToValues(location);
        }
        #endregion
        #region Child Management Methods
        /// <summary>
        /// Adds a collection of children at the current location in the tree.
        /// </summary>
        /// <param name="childrenToAdd">The children objects to add.</param>
        public void AddChildren(params T[] childrenToAdd)
        {
            // Update the endpoints count.
            Endpoints += childrenToAdd.Length;
            if (Children.Count == 0 && childrenToAdd.Length != 0)
                Endpoints--;
            // Add the collection of children.
            foreach (T child in childrenToAdd)
                location.children.Add(new Branch
                {
                    parent = location,
                    value = child,
                    children = new List<Branch>()
                });
            // Update the children property.
            Children = ChildrenToValues(location);
        }
        /// <summary>
        /// Removes a child of the current location along with
        /// all of its children down the entire tree.
        /// </summary>
        /// <param name="child">The child to remove.</param>
        public void RemoveChildBranch(T child)
        {
            Branch branch = FindChildByValue(child);
            // Remove branches recursively.
            Endpoints -= RemoveBranchRecursive(branch);
            // Finish by removing this branch.
            location.children.Remove(branch);
            Children = ChildrenToValues(location);
            // Recursive function definition.
            int RemoveBranchRecursive(Branch baseBranch)
            {
                // Continue recursion keeping track of
                // the value returned representing the
                // endpoints that have been removed.
                int endpointsRemoved = 0;
                foreach (Branch childBranch in baseBranch.children)
                    endpointsRemoved += RemoveBranchRecursive(childBranch);
                // Keep track of whether the saved location
                // is removed to throw better contextual errors.
                if (baseBranch == savedLocation)
                {
                    savedLocation = null;
                    savedLocationWasRemoved = true;
                }
                // If THIS recursion is an endpoint that the
                // higher recursions know.
                if (baseBranch.children.Count == 0)
                    endpointsRemoved++;
                // Dereference everything to assist garbage collector.
                baseBranch.parent = null;
                baseBranch.children = null;
                return endpointsRemoved;
            }
        }
        /// <summary>
        /// Gets all of the child nodes below the given location,
        /// including children at all depth levels.
        /// </summary>
        /// <returns>A full collection of all children below the location.</returns>
        public List<T> GetAllChildrenBelowLocation()
        {
            // Use recursion to retrieve all children
            // below the current position.
            List<T> children = new List<T>();
            foreach (Branch branch in location.children)
                GetDeepChildrenRecursive(branch);
            return children;
            // Recursive function definition.
            void GetDeepChildrenRecursive(Branch branch)
            {
                foreach (Branch childBranch in branch.children)
                    GetDeepChildrenRecursive(childBranch);
                children.Add(branch.value);
            }
        }
        #endregion
        #region Save/Load Traversal Methods
        /// <summary>
        /// Saves a location in the tree that can be returned to by calling LoadLocation.
        /// </summary>
        public void SaveLocation()
        {
            savedLocation = location;
            savedLocationWasRemoved = false;
        }
        /// <summary>
        /// Loads the previously saved location in the tree.
        /// </summary>
        public void LoadLocation()
        {
            // Ensure that this load is valid.
            if (savedLocationWasRemoved)
                throw new InvalidOperationException("Attempted to return to tree location that was removed.");
            else if (savedLocation == null)
                throw new InvalidOperationException("A location must be saved before being loaded.");
            else
            {
                // Go to the saved location and set
                // all accessor properties.
                location = savedLocation;
                IsAtRoot = (location == root);
                Current = location.value;
                Children = ChildrenToValues(location);
            }
        }
        #endregion
        #region Depth Traversal Methods
        /// <summary>
        /// Moves up to the current location's parent.
        /// </summary>
        public void StepOut()
        {
            // Throw an error if we are already at the root.
            if (IsAtRoot)
                throw new InvalidOperationException(
                    "Attempted to step out when at root of the tree. Check `IsAtRoot` property.");
            else
            {
                // Move up and set accessor properties.
                location = location.parent;
                IsAtRoot = (location == root);
                Current = location.value;
                Children = ChildrenToValues(location);
            }
        }
        /// <summary>
        /// Moves down to the specified child index of the current location.
        /// </summary>
        /// <param name="childIndex">The index of the child to move to.</param>
        public void StepIn(int childIndex)
        {
            if (childIndex > location.children.Count - 1)
                throw new ArgumentOutOfRangeException("childIndex",
                    $"Requested child index `{childIndex}` is out of range. Should be between 0 and {location.children.Count - 1}.");
            else
                StepIn(location.children[childIndex]);
        }
        /// <summary>
        /// Moves down to the specified child of the current location.
        /// </summary>
        /// <param name="child">The child to move to.</param>
        public void StepIn(T child)
        {
            StepIn(FindChildByValue(child));
        }
        #endregion
    }
}
