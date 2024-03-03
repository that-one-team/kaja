using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TOT.Common
{
    public static class Extensions
    {
        public static void DestroyChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }

        public static T SelectRandom<T>(this IEnumerable<T> list)
        {
            var rnd = new System.Random();
            return list.ElementAt(rnd.Next(list.Count()));
        }

        public static IList<T> Shuffle<T>(this IList<T> list) => list.OrderBy(x => Guid.NewGuid()).ToList();
    }
}