using System;
using System.Collections;
using System.Collections.Generic;

namespace MB6.URP.Fade
{
    public class RendererElementList : IEnumerable<RendererListElement>
    {
        private List<RendererListElement> _listOfRendererElements;

        public int Count => _listOfRendererElements.Count;
        public IEnumerable<RendererListElement> redendererElements => _listOfRendererElements;

        public RendererElementList()
        {
            _listOfRendererElements = new List<RendererListElement>();
        }

        public RendererListElement this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("You are trying to access a value that is out of range.");
                }

                if (index >= _listOfRendererElements.Count)
                {
                    throw new ArgumentOutOfRangeException("You are trying to access a value that is out of range.");
                }

                return _listOfRendererElements[index];
            }
            set
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("You are trying to access a value that is out of range.");
                }

                if (index > _listOfRendererElements.Count)
                {
                    throw new ArgumentOutOfRangeException("You are trying to access a value that is out of range.");
                }

                if (index == _listOfRendererElements.Count)
                {
                    _listOfRendererElements.Add(value);
                    return;
                }

                _listOfRendererElements[index] = value;

            }
        }

        public void AddRendererElement(RendererListElement element)
        {
            _listOfRendererElements.Add(element);
        }

        public void RemoveRendererElement(RendererListElement element)
        {
            _listOfRendererElements.Remove(element);
        }
        public IEnumerator<RendererListElement> GetEnumerator()
        {
            return _listOfRendererElements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}