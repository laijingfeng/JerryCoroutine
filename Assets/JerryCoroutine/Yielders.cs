﻿using System.Collections.Generic;
using UnityEngine;

// Usage:
//    yield return new WaitForEndOfFrame();     =>      yield return Yielders.EndOfFrame;
//    yield return new WaitForFixedUpdate();    =>      yield return Yielders.FixedUpdate;
//    yield return new WaitForSeconds(1.0f);    =>      yield return Yielders.GetWaitForSeconds(1.0f);
// http://forum.unity3d.com/threads/c-coroutine-waitforseconds-garbage-collection-tip.224878/

//摘自：https://github.com/PerfAssist/PA_Common/blob/master/Scripts/Yielders.cs

namespace Jerry
{
    public static class Yielders
    {
        // dictionary with a key of ValueType will box the value to perform comparison / hash code calculation while scanning the hashtable.
        // here we implement IEqualityComparer<float> and pass it to your dictionary to avoid that GC
        class FloatComparer : IEqualityComparer<float>
        {
            bool IEqualityComparer<float>.Equals(float x, float y)
            {
                return x == y;
            }
            int IEqualityComparer<float>.GetHashCode(float obj)
            {
                return obj.GetHashCode();
            }
        }

        public static WaitForEndOfFrame EndOfFrame = new WaitForEndOfFrame();
        public static WaitForFixedUpdate FixedUpdate = new WaitForFixedUpdate();

        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            WaitForSeconds wfs;
            // it is always better to use TryGetValue() method instead of dict.Contains(key) + dict[key] 
            // since it performs what you want with a single 'pass' through hashtable.
            if (!_waitForSecondsYielders.TryGetValue(seconds, out wfs))
            {
                _waitForSecondsYielders.Add(seconds, wfs = new WaitForSeconds(seconds));
            }

            return wfs;
        }

        public static void ClearWaitForSeconds()
        {
            _waitForSecondsYielders.Clear();
        }

        private static Dictionary<float, WaitForSeconds> _waitForSecondsYielders = new Dictionary<float, WaitForSeconds>(100, new FloatComparer());
    }
}