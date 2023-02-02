using System;
using System.Collections;
using System.Collections.Generic;

namespace MB6.URP.Fade
{
    public class RendererDetailsList : IEnumerable<RendererDetails>
    {
        public bool HasFeatureOnDefaultPipeline { get; private set; }
        public bool FeatureSetDefaultRendererForDefaultPipeline { get; private set; }
        
        private List<RendererDetails> _listOfDetails;
        private bool isDirty = true;
        private RendererDetails _defaultRendererDetails;

        public RendererDetailsList()
        {
            _listOfDetails = new List<RendererDetails>();
        }

        public int Count => _listOfDetails.Count;

        public IEnumerator<RendererDetails> GetEnumerator()
        {
            return _listOfDetails.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public RendererDetails this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("Is out of range");
                }

                if (index >= _listOfDetails.Count)
                {
                    throw new ArgumentOutOfRangeException("Is out of range");
                }

                return _listOfDetails[index];
            }

            set
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("Is out of range");
                }

                if (index > _listOfDetails.Count)
                {
                    throw new ArgumentOutOfRangeException("Is out of range");
                }

                if (index == _listOfDetails.Count)
                {
                    if (value != null)
                    {
                        Add(value);
                    }

                    return;
                }

                if (value == null)
                {
                    Remove(value);
                }

                Add(value);
            }
        }

        public void Add(RendererDetails details)
        {
            if (details.IsOnDefaultPipeline)
            {
                HasFeatureOnDefaultPipeline = true;
                if (details.IsDefaultRendererInPipeline)
                {
                    FeatureSetDefaultRendererForDefaultPipeline = true;
                }
            }
            
            _listOfDetails.Add(details);
        }

        public void Remove(RendererDetails details)
        {
            _listOfDetails.Remove(details);

            isDirty = true;

            if (details.IsDefaultRendererInPipeline)
            {
                FeatureSetDefaultRendererForDefaultPipeline = false;
            }
            
            if (CheckForFeaturesOnDefaultPipeline() == false)
            {
                HasFeatureOnDefaultPipeline = false;
            }
        }

        public void SetRendererDetailsList(List<RendererDetails> list)
        {
            _listOfDetails = list;
        }

        public bool CheckForFeaturesOnDefaultPipeline()
        {
            foreach (var rendererDetail in _listOfDetails)
            {
                if (rendererDetail.IsOnDefaultPipeline)
                {
                    return true;
                }
            }

            return false;
        }

        public RendererDetails GetDefault()
        {
            if (!isDirty) return _defaultRendererDetails;

            foreach (var detail in _listOfDetails)
            {
                if (detail.IsOnDefaultPipeline && detail.IsDefaultRendererInPipeline)
                {
                    _defaultRendererDetails = detail;
                    isDirty = false;
                    break;
                }
            }

            return _defaultRendererDetails;
        }
    }
}
