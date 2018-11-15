//Version: 2018-11-15-00
﻿
using System.Collections.Generic;
using UnityEngine;

// Usage:
//    yield return new WaitForEndOfFrame();     =>      yield return Yielders.EndOfFrame;
//    yield return new WaitForFixedUpdate();    =>      yield return Yielders.FixedUpdate;
//    yield return new WaitForSeconds(1.0f);    =>      yield return Yielders.GetWaitForSeconds(1.0f);
// http://forum.unity3d.com/threads/c-coroutine-waitforseconds-garbage-collection-tip.224878/

//摘自：https://github.com/PerfAssist/PA_Common/blob/master/Scripts/Yielders.cs

namespace Jerry
{
    /// <summary>
    /// Yielders
    /// </summary>
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

        /// <summary>
        /// 等价于：new WaitForEndOfFrame()
        /// </summary>
        public static WaitForEndOfFrame EndOfFrame = new WaitForEndOfFrame();
        /// <summary>
        /// 等价于：new WaitForFixedUpdate()
        /// </summary>
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

        /// <summary>
        /// 切场景的时候可以清理一次
        /// </summary>
        public static void ClearWaitForSeconds()
        {
            _waitForSecondsYielders.Clear();
        }

        private static Dictionary<float, WaitForSeconds> _waitForSecondsYielders = new Dictionary<float, WaitForSeconds>(100, new FloatComparer());
    }
}