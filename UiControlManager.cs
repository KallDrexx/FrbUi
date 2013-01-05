using System;
using System.Collections.Generic;
using System.Linq;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Screens;
using FrbUi.SelectableGroupings;

namespace FrbUi
{
    public class UiControlManager
    {
        private static readonly object Padlock = new Object();
        private static UiControlManager _instance;

        private readonly List<ILayoutable> _items;
        private readonly List<ISelectableControlGroup> _selectableControlGroups;
        private Layer _uiLayer;
        private Screen _lastScreen;

        private UiControlManager() 
        {
            _items = new List<ILayoutable>();
            _selectableControlGroups = new List<ISelectableControlGroup>();
        }

        public static UiControlManager Instance
        {
            get
            {
                lock (Padlock)
                {
                    return _instance ?? (_instance = new UiControlManager());
                }
            }
        }

        public Layer Layer { get { return _uiLayer; } }

        public void AddControl(ILayoutable item)
        {
            CheckCurrentScreen();

            if (item == null || _items.Contains(item))
                return;

            _items.Add(item);
            item.AddToManagers(_uiLayer);
        }

        public TResult CreateControl<TResult>() where TResult : ILayoutable
        {
            if (typeof(TResult).IsInterface)
                throw new InvalidOperationException("Cannot create a control from an interface");

            var control = Activator.CreateInstance<TResult>();
            AddControl(control);
            return control;
        }

        public void DestroyControl(ILayoutable item)
        {
            if (item == null)
                return;

            _items.Remove(item);
            item.Destroy();

            // If the control is selectable and is in a control group, remove it
            var selectable = item as ISelectable;
            if (selectable != null)
            {
                foreach (var group in _selectableControlGroups)
                    if (group.Contains(selectable))
                        group.Remove(selectable);
            }
        }

        public TResult CreateSelectableControlGroup<TResult>() where TResult : ISelectableControlGroup
        {
            if (typeof(TResult).IsInterface)
                throw new InvalidOperationException("Cannot create a selectable control group from an interface");

            var group = Activator.CreateInstance<TResult>();
            _selectableControlGroups.Add(group);
            return group;
        }

        public void RemoveSelectableControlGroup(ISelectableControlGroup group)
        {
            group.Destroy();
            if (_selectableControlGroups.Contains(group))
                _selectableControlGroups.Remove(group);
        }

        public void RunActivities()
        {
            CheckCurrentScreen();

            foreach (var item in _items)
                item.Activity();
        }

        public void UpdateDependencies()
        {
            CheckCurrentScreen();

            // Update all ui controls that do not have a parent
            //  Layout managers *should* cascade the updates downward
            foreach (var item in _items.Where(x => x.ParentLayout == null))
                item.UpdateDependencies(TimeManager.CurrentTime);
        }

        private void CreateUiLayer()
        {
            // Create the 2d layer
            _uiLayer = SpriteManager.AddLayer();
            _uiLayer.LayerCameraSettings = new LayerCameraSettings();
            _uiLayer.LayerCameraSettings.Orthogonal = true;
            _uiLayer.LayerCameraSettings.OrthogonalWidth = SpriteManager.Camera.OrthogonalWidth;
            _uiLayer.LayerCameraSettings.OrthogonalHeight = SpriteManager.Camera.OrthogonalHeight;
        }

        private void CheckCurrentScreen()
        {
            if (_lastScreen == ScreenManager.CurrentScreen)
                return;

            // Since the layer has changed, recreate the UI layer
            CreateUiLayer();

            // Screen has changed, set it up so all UI controls get destroyed
            //   when this screen is destroyed
            if (_lastScreen != null)
                _lastScreen.ScreenDestroy -= PrepareForScreenChange;

            ScreenManager.CurrentScreen.ScreenDestroy += PrepareForScreenChange;
            _lastScreen = ScreenManager.CurrentScreen;
        }

        private void PrepareForScreenChange()
        {
            for (int x = _items.Count - 1; x >= 0; x--)
            {
                _items[x].Destroy();
                _items.RemoveAt(x);
            }

            // Destroy the layer
            SpriteManager.RemoveLayer(_uiLayer);
            _uiLayer = null;
        }
    }
}
