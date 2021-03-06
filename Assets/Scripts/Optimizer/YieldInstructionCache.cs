using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Optimizer
{
    internal static class YieldInstructionCache
    {
        public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

        private static readonly Dictionary<float, WaitForSeconds> TimeInterval = new Dictionary<float, WaitForSeconds>(new FloatComparer());
        private static readonly Dictionary<float, WaitForSecondsRealtime> TimeIntervalRealtime = new Dictionary<float, WaitForSecondsRealtime>(new FloatComparer());

        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            if (!TimeInterval.TryGetValue(seconds, out var waitForSeconds))
                TimeInterval.Add(seconds, waitForSeconds = new WaitForSeconds(seconds));
            return waitForSeconds;
        }

        public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
        {
            if (!TimeIntervalRealtime.TryGetValue(seconds, out var waitForSecondsRealtime))
                TimeIntervalRealtime.Add(seconds, waitForSecondsRealtime = new WaitForSecondsRealtime(seconds));
            return waitForSecondsRealtime;
        }

        private class FloatComparer : IEqualityComparer<float>
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            bool IEqualityComparer<float>.Equals(float x, float y) => x == y;

            int IEqualityComparer<float>.GetHashCode(float obj) => obj.GetHashCode();
        }
    }
}
