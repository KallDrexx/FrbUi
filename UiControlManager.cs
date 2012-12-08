using System;
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
        private Layer _uiLayer;

        private UiControlManager() 
        {
            _items = new List<ILayoutable>();

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

        public void DestroyControl(ILayoutable item)
        {
            if (item == null)
                return;

            _items.Remove(item);
            item.Destroy();
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
