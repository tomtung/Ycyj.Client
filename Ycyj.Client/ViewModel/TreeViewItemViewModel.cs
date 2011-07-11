using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;

namespace Ycyj.Client.ViewModel
{
    public class TreeViewItemViewModel : ViewModelBase
    {
        protected readonly ObservableCollection<TreeViewItemViewModel> ChildrenSource
            = new ObservableCollection<TreeViewItemViewModel>();

        private readonly ReadOnlyObservableCollection<TreeViewItemViewModel> _children;

        protected TreeViewItemViewModel(TreeViewItemViewModel parent)
        {
            Parent = parent;
            _children = new ReadOnlyObservableCollection<TreeViewItemViewModel>(ChildrenSource);
        }

        public IEnumerable<TreeViewItemViewModel> Children
        {
            get { return _children; }
        }

        public TreeViewItemViewModel Parent { get; protected set; }

        #region Bindable Properties

        #region IsExpanded Property

        public const string IsExpandedPropertyName = "IsExpanded";
        private bool _isExpanded;

        /// <summary>
        /// Gets the IsExpanded property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// Settting this property to true will cause the parent of the TreeViewItem to be expanded as well.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }

            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    RaisePropertyChanged(IsExpandedPropertyName);
                }

                if (_isExpanded && Parent != null)
                    Parent.IsExpanded = true;
            }
        }

        #endregion

        #region IsSelected Property

        public const string IsSelectedPropertyName = "IsSelected";
        private bool _isSelected;

        /// <summary>
        /// Gets the IsSelected property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                RaisePropertyChanged(IsSelectedPropertyName);
                if (_isSelected && Parent != null)
                    Parent.IsExpanded = true;
            }
        }

        #endregion

        #region IsCheckedProperty

        public const string IsCheckedPropertyName = "IsChecked";
        private bool? _isChecked = false;

        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetIsChecked(value, true, true); }
        }

        private void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
                return;
            _isChecked = value;

            if (updateChildren && _isChecked != null)
                foreach (TreeViewItemViewModel child in Children)
                    child.SetIsChecked(_isChecked, true, false);

            if (updateParent && Parent != null)
                Parent.VerifyCheckState();
            RaisePropertyChanged(IsCheckedPropertyName);
        }

        private void VerifyCheckState()
        {
            if (_children.Count == 0) return;

            bool? state = Children.All(child => child.IsChecked == Children.First().IsChecked)
                              ? Children.First().IsChecked
                              : null;
            SetIsChecked(state, false, true);
        }

        #endregion

        #endregion

        public void DetachFromParent()
        {
            BeforeDetachFromParent();
            if (Parent == null) return;
            Parent.ChildrenSource.Remove(this);
            Parent = null;
        }

        protected virtual void BeforeDetachFromParent()
        {
        }
    }
}