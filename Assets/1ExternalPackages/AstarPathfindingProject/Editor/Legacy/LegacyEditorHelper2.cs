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
using System.Linq;

namespace Pathfinding.Legacy {
	public static class LegacyEditorHelper {
		public static void UpgradeDialog (Object[] targets, System.Type upgradeType) {
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			var gui = EditorGUIUtility.IconContent("console.warnicon");
			gui.text = "You are using the compatibility version of this component. It is recommended that you upgrade to the newer version. This may change the component's behavior.";
			EditorGUILayout.LabelField(GUIContent.none, gui, EditorStyles.wordWrappedMiniLabel);
			if (GUILayout.Button("Upgrade")) {
				Undo.RecordObjects(targets.Select(s => (s as Component).gameObject).ToArray(), "Upgrade from Legacy Component");
				foreach (var tg in targets) {
					var comp = tg as Component;
					var components = comp.gameObject.GetComponents<Component>();
					int index = System.Array.IndexOf(components, comp);
					var newRVO = Undo.AddComponent(comp.gameObject, upgradeType);
					foreach (var field in newRVO.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)) {
						field.SetValue(newRVO, field.GetValue(comp));
					}
					Undo.DestroyObjectImmediate(comp);
					for (int i = components.Length - 1; i > index; i--) UnityEditorInternal.ComponentUtility.MoveComponentUp(newRVO);
				}
			}
			EditorGUILayout.EndVertical();
		}
	}
}
