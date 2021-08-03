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
using UnityEditor;

namespace Pathfinding.Legacy {
	[CustomEditor(typeof(LegacyAIPath))]
	[CanEditMultipleObjects]
	public class LegacyAIPathEditor : BaseAIEditor {
		protected override void Inspector () {
			base.Inspector();
			var gravity = FindProperty("gravity");
			if (!gravity.hasMultipleDifferentValues && !float.IsNaN(gravity.vector3Value.x)) {
				gravity.vector3Value = new UnityEngine.Vector3(float.NaN, float.NaN, float.NaN);
				serializedObject.ApplyModifiedPropertiesWithoutUndo();
			}
			LegacyEditorHelper.UpgradeDialog(targets, typeof(AIPath));
		}
	}
}
