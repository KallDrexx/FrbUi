﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using FlatRedBall.Graphics;

namespace FrbUi
{
    public class UiControlManager
    {
        private static readonly object _padlock = new Object();
        private static UiControlManager _instance;

        private List<ILayoutable> _items;
        private List<SelectableControlGroup> _selectableControlGroups;
        private Layer _uiLayer;

        private UiControlManager() 
        {
            _items = new List<ILayoutable>();
            _selectableControlGroups = new List<SelectableControlGroup>();

            // Create the 2d layer
            _uiLayer = SpriteManager.AddLayer();
            _uiLayer.LayerCameraSettings = new LayerCameraSettings();
            _uiLayer.LayerCameraSettings.Orthogonal = true;
            _uiLayer.LayerCameraSettings.OrthogonalWidth = SpriteManager.Camera.OrthogonalWidth;
            _uiLayer.LayerCameraSettings.OrthogonalHeight = SpriteManager.Camera.OrthogonalHeight;
        }

        public static UiControlManager Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                        _instance = new UiControlManager();

                    return _instance;
                }
            }
        }

        public Layer Layer { get { return _uiLayer; } }

        public void AddControl(ILayoutable item)
        {
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

        public SelectableControlGroup CreateSelectableControlGroup()
        {
            var group = new SelectableControlGroup();
            _selectableControlGroups.Add(group);
            return group;
        }

        public void RemoveSelectableControlGroup(SelectableControlGroup group)
        {
            group.Destroy();
            if (!_selectableControlGroups.Contains(group))
                return;

            _selectableControlGroups.Remove(group);
        }

        public void RunActivities()
        {
            foreach (var item in _items)
                item.Activity();
        }

        public void UpdateDependencies()
        {
            foreach (var item in _items)
                item.UpdateDependencies(TimeManager.CurrentTime);
        }
    }
}
