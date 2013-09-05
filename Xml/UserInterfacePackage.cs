using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FlatRedBall;
using FlatRedBall.IO;
using FrbUi.Xml.Models;

namespace FrbUi.Xml
{
    public class UserInterfacePackage
    {
        private readonly Dictionary<string, ILayoutable> _namedControls; 

        public UserInterfacePackage(string xmlFile, string contentManagerName)
        {
            _namedControls = new Dictionary<string, ILayoutable>();
            LoadUserInterfacePackage(xmlFile, contentManagerName);
        }

        public T GetNamedControl<T>(string name) where T : class
        {
            ILayoutable control;
            if (!_namedControls.TryGetValue(name, out control))
                return null;

            return control as T;
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

                    // Instantiate the layoutable controls, and save the named controls
                    foreach (var asset in assetCollection.Controls)
                        asset.GenerateILayoutable(contentManagerName, _namedControls);
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
