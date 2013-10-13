using System;
using System.Xml.Serialization;
using FlatRedBall.Graphics;

namespace FrbUi.Xml.Models.Fonts
{
    public class BitmapFontXml
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string FontFile { get; set; }

        [XmlAttribute]
        public string TextureFile { get; set; }

        public BitmapFont GenerateFont(string contentManagerName)
        {
            if (string.IsNullOrWhiteSpace(FontFile))
                throw new InvalidOperationException("No font file specified for the BitmapFont");

            return !string.IsNullOrWhiteSpace(TextureFile)
                       ? new BitmapFont(TextureFile, FontFile, contentManagerName)
                       : new BitmapFont(FontFile, contentManagerName);
        }
    }
}
