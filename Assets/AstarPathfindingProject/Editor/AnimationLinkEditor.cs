//supressed warning by unity global warning supressor
#pragma warning disable 8600
#pragma warning disable 8601
#pragma warning disable 8602
#pragma warning disable 8603
#pragma warning disable 8604
#pragma warning disable 8618
#pragma warning disable 8625
#pragma warning disable 0169
//supressed warning by unity global warning supressor
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Pathfinding {
	[CustomEditor(typeof(AnimationLink))]
	public class AnimationLinkEditor : Editor {
		public override void OnInspectorGUI () {
			DrawDefaultInspector();

			var script = target as AnimationLink;

			EditorGUI.BeginDisabledGroup(script.EndTransform == null);
			if (GUILayout.Button("Autoposition Endpoint")) {
				List<Vector3> buffer = Pathfinding.Util.ListPool<Vector3>.Claim ();
				Vector3 endpos;
				script.CalculateOffsets(buffer, out endpos);
				script.EndTransform.position = endpos;
				Pathfinding.Util.ListPool<Vector3>.Release (buffer);
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}
