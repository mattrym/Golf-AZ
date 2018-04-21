using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GolfApp.Structures
{
    public class Matching : IEnumerable<Hit>
    {
        public static readonly Matching Empty = new Matching();

        private IList<Hit> hits = new List<Hit>();

        private Matching()
        {
        }

        public Matching(Hit hit)
        {
            hits.Add(hit);
        }

        public bool IsPlanar()
        {
            for (var i = 0; i < hits.Count; ++i)
            for (var j = i + 1; j < hits.Count; ++j)
                if (hits[i].Intersects(hits[j]))
                    return false;

            return true;
        }

        public static Matching operator +(Matching matchingLeft, Matching matchingRight)
        {
            return new Matching()
                .Add(matchingLeft)
                .Add(matchingRight);
        }

        public static Matching operator +(Matching matching, Hit hit)
        {
            return new Matching()
                .Add(hit)
                .Add(matching);
        }

        public static Matching operator +(Hit hit, Matching matching)
        {
            return matching + hit;
        }

        private Matching Add(Hit hit)
        {
            hits.Add(hit);
            return this;
        }

        private Matching Add(Matching matching)
        {
            hits = hits.Concat(matching.hits).ToList();
            return this;
        }

        public IEnumerator<Hit> GetEnumerator()
        {
            return hits.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) hits).GetEnumerator();
        }
    }
}