#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Loader
{
    public static class ComponentReferenceHolder
    {
        public static Dictionary<string, Transform>? PositionSavers;
        public static string? EntryPointName;
        public static bool CreditFromTitle;

        public static void Reset()
        {
            PositionSavers = null;
            EntryPointName = null;
        }
    }
}
