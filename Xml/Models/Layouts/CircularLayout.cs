using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Animation;

namespace FrbUi.Xml.Models.Layouts
{
    public class CircularLayout : AssetBase
    {
        public enum ArrangementMode { Clockwise, EvenlySpaced, Manual, CounterClockwise }

        #region Values

        private float? _minDegreeOffset;
        private float? _startingDegrees;
        private float? _radius;
        private float? _margin;
        private float? _alpha;
        private string _animationChainFile;
        private string _initialAnimationChainName;
        private ArrangementMode? _arrangementMode;
        private List<CircularLayoutChild> _children;

        #endregion

        #region Xml

        [XmlIgnore]
        public bool MinDegreeOffsetValueSpecified { get { return _minDegreeOffset.HasValue; } }

        [XmlAttribute("MinDegreeOffset")]
        public float MinDegreeOffsetValue
        {
            get { return _minDegreeOffset ?? default(float); }
            set { _minDegreeOffset = value; }
        }

        [XmlIgnore]
        public bool StartingDegreesValueSpecified { get { return _startingDegrees.HasValue; } }

        [XmlAttribute("StartingDegrees")]
        public float StartingDegreesValue
        {
            get { return _startingDegrees ?? default(float); }
            set { _startingDegrees = value; }
        }

        [XmlIgnore]
        public bool RadiusValueSpecified { get { return _radius.HasValue; } }

        [XmlAttribute("Radius")]
        public float RadiusValue
        {
            get { return _radius ?? default(float); }
            set { _radius = value; }
        }

        [XmlIgnore]
        public bool MarginValueSpecified { get { return _margin.HasValue; } }

        [XmlAttribute("Margin")]
        public float MarginValue
        {
            get { return _margin ?? default(float); }
            set { _margin = value; }
        }

        [XmlIgnore]
        public bool AlphaValueSpecified { get { return _alpha.HasValue; } }

        [XmlAttribute("Alpha")]
        public float AlphaValue
        {
            get { return _alpha ?? default(float); }
            set { _alpha = value; }
        }

        [XmlIgnore]
        public bool ArrangementValueSpecified { get { return _arrangementMode.HasValue; } }

        [XmlAttribute("ArrangementMode")]
        public ArrangementMode ArrangementValue
        {
            get { return _arrangementMode ?? default(ArrangementMode); }
            set { _arrangementMode = value; }
        }

        [XmlAttribute]
        public string AchxFile
        {
            get { return _animationChainFile; }
            set { _animationChainFile = value; }
        }

        [XmlAttribute]
        public string InitialAnimationChainName
        {
            get { return _initialAnimationChainName; }
            set { _initialAnimationChainName = value; }
        }

        [XmlArray]
        public List<CircularLayoutChild> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        #endregion

        public override ILayoutable GenerateILayoutable(string contentManagerName, Dictionary<string, ILayoutable> namedControls, Dictionary<string, BitmapFont> namedFonts)
        {
            var layout = UiControlManager.Instance.CreateControl<FrbUi.Layouts.CircularLayout>();
            SetBaseILayoutableProperties(layout, namedControls);

            layout.MinDegreeOffset = _minDegreeOffset ?? 0f;
            layout.StartingDegrees = _startingDegrees ?? 0f;
            layout.Radius = _radius ?? 0f;
            layout.Margin = _margin ?? 0f;
            layout.Alpha = _alpha ?? 1f;
            
            if (!string.IsNullOrWhiteSpace(_animationChainFile))
                layout.BackgroundAnimationChains = FlatRedBallServices.Load<AnimationChainList>(_animationChainFile, contentManagerName);

            if (!string.IsNullOrWhiteSpace(_initialAnimationChainName))
                layout.CurrentAnimationChainName = _initialAnimationChainName;

            switch (_arrangementMode)
            {
                case ArrangementMode.CounterClockwise:
                    layout.CurrentArrangementMode = FrbUi.Layouts.CircularLayout.ArrangementMode.CounterClockwise;
                    break;

                case ArrangementMode.EvenlySpaced:
                    layout.CurrentArrangementMode = FrbUi.Layouts.CircularLayout.ArrangementMode.EvenlySpaced;
                    break;

                case ArrangementMode.Manual:
                    layout.CurrentArrangementMode = FrbUi.Layouts.CircularLayout.ArrangementMode.Manual;
                    break;

                default:
                    layout.CurrentArrangementMode = FrbUi.Layouts.CircularLayout.ArrangementMode.Clockwise;
                    break;
            }

            foreach (var child in Children.Where(x => x.Item != null))
            {
                var item = child.Item.GenerateILayoutable(contentManagerName, namedControls, namedFonts);
                layout.AddItem(item, child.RadiusOffsetValue, child.DegreeOffsetValue);
            }

            return layout;
        }
    }
}
