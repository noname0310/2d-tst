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

namespace Pathfinding {
	[ExecuteInEditMode]
	/// <summary>
	/// Helper class to keep track of references to GameObjects.
	/// Does nothing more than to hold a GUID value.
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_unity_reference_helper.php")]
	public class UnityReferenceHelper : MonoBehaviour {
		[HideInInspector]
		[SerializeField]
		private string guid;

		public string GetGUID () {
			return guid;
		}

		public void Awake () {
			Reset();
		}

		public void Reset () {
			if (string.IsNullOrEmpty(guid)) {
				guid = Pathfinding.Util.Guid.NewGuid().ToString();
				Debug.Log("Created new GUID - "+guid);
			} else {
				foreach (UnityReferenceHelper urh in FindObjectsOfType(typeof(UnityReferenceHelper)) as UnityReferenceHelper[]) {
					if (urh != this && guid == urh.guid) {
						guid = Pathfinding.Util.Guid.NewGuid().ToString();
						Debug.Log("Created new GUID - "+guid);
						return;
					}
				}
			}
		}
	}
}
