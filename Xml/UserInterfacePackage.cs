using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using FlatRedBall.Graphics;
using FlatRedBall.IO;
using FrbUi.Xml.Models;

namespace FrbUi.Xml
{
    public class UserInterfacePackage
    {
        private readonly Dictionary<string, ILayoutable> _namedControls;
        private readonly Dictionary<string, ISelectableControlGroup> _namedSelectableGroups;
        private readonly Dictionary<string, BitmapFont> _namedFonts; 

        public UserInterfacePackage(string xmlFile, string contentManagerName)
        {
            _namedControls = new Dictionary<string, ILayoutable>();
            _namedSelectableGroups = new Dictionary<string, ISelectableControlGroup>();
            _namedFonts = new Dictionary<string, BitmapFont>();
            LoadUserInterfacePackage(xmlFile, contentManagerName);
        }

        public T GetNamedControl<T>(string name) where T : class, ILayoutable
        {
            ILayoutable control;
            if (!_namedControls.TryGetValue(name, out control))
                return null;

            return control as T;
        }

        public IEnumerable<KeyValuePair<string, Type>> GetNamedControls()
        {
            return _namedControls.Select(x => new KeyValuePair<string, Type>(x.Key, x.Value.GetType()));
        }

        public T GetNamedSelectableGroup<T>(string name) where T : class, ISelectableControlGroup
        {
            ISelectableControlGroup control;
            if (!_namedSelectableGroups.TryGetValue(name, out control))
                return null;

            return control as T;
        }

        public IEnumerable<KeyValuePair<string, Type>> GetNamedSelectableGroups()
        {
            return _namedSelectableGroups.Select(x => new KeyValuePair<string, Type>(x.Key, x.Value.GetType()));
        }

        public BitmapFont GetNamedFont(string name)
        {
            BitmapFont font;
            if (!_namedFonts.TryGetValue(name, out font))
                return null;

            return font;
        }

        private void LoadUserInterfacePackage(string xmlFile, string contentManagerName)
        {
            // Set the relative path to the same path as the xml file
            // but store the old path so we can reset it after
            var oldRelativePath = FileManager.RelativeDirectory;

            try
            {
                FileManager.RelativeDirectory = FileManager.GetDirectory(xmlFile);

                var serializer = new XmlSerializer(typeof (AssetCollection));
                using (var reader = new StreamReader(xmlFile))
                {
                    var assetCollection = (AssetCollection) serializer.Deserialize(reader);

                    // Fonts must be retrieved first, so they are available when LayoutableText objects
                    // are instantiated
                    foreach (var fontXml in assetCollection.BitmapFonts)
                    {
                        var font = fontXml.GenerateFont(contentManagerName);
                        _namedFonts[fontXml.Name] = font;
                    }

                    // Instantiate the layoutable controls, and save the named controls
                    foreach (var asset in assetCollection.Controls)
                        asset.GenerateILayoutable(contentManagerName, _namedControls, _namedFonts);

                    foreach (var selectableGroup in assetCollection.SelectableGroups)
                        selectableGroup.GenerateSelectableControlGroup(_namedControls, _namedSelectableGroups);
                }
            }
            finally
            {
                // Even if an error occurs, we need to reset the relative path
                FileManager.RelativeDirectory = oldRelativePath;
            }
        }
    }
}
