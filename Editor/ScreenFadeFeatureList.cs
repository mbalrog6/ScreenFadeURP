using System;
using System.Collections;
using System.Collections.Generic;

namespace MB6.URP.Fade
{
    public class ScreenFadeFeatureList : IEnumerable<FeatureListElement>
    {
        public FeatureListElement DefaultFeature { get; private set; }
        public int Count => featureElements.Count;
        private List<FeatureListElement> featureElements;

        public IEnumerable<FeatureListElement> Features => featureElements;

        public ScreenFadeFeatureList()
        {
            featureElements = new List<FeatureListElement>();
        }

        public void AddFeature(FeatureListElement element)
        {
            featureElements.Add(element);
            if (element.IsOnDefaultRenderer)
            {
                DefaultFeature = element;
            }
        }
        
        public FeatureListElement this[int index]
        {
            get
            {
                if (index >= featureElements.Count)
                    throw new ArgumentOutOfRangeException("You are trying to access a value that is out of range.");
                
                if(index < 0)
                    throw new ArgumentOutOfRangeException("You are trying to access a value that is out of range.");

                return featureElements[index];
            }
            set
            {
                if (index > featureElements.Count)
                    throw new ArgumentOutOfRangeException("You are trying to access a value that is out of range.");
                
                if(index < 0)
                    throw new ArgumentOutOfRangeException("You are trying to access a value that is out of range.");

                if (index == featureElements.Count)
                {
                    featureElements.Add(value);
                    return;
                }

                featureElements[index] = value;
            }
        }

        public void RemoveFeature(FeatureListElement element)
        {
            featureElements.Remove(element);
            if (element.IsOnDefaultRenderer)
            {
                DefaultFeature = null;
            }
        }

        public IEnumerator<FeatureListElement> GetEnumerator()
        {
            return featureElements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}