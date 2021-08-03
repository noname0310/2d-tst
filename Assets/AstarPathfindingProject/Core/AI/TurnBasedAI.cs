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
using System.Collections.Generic;

namespace Pathfinding.Examples {
	/// <summary>Helper script in the example scene 'Turn Based'</summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_examples_1_1_turn_based_a_i.php")]
	public class TurnBasedAI : VersionedMonoBehaviour {
		public int movementPoints = 2;
		public BlockManager blockManager;
		public SingleNodeBlocker blocker;
		public GraphNode targetNode;
		public BlockManager.TraversalProvider traversalProvider;

		void Start () {
			blocker.BlockAtCurrentPosition();
		}

		protected override void Awake () {
			base.Awake();
			// Set the traversal provider to block all nodes that are blocked by a SingleNodeBlocker
			// except the SingleNodeBlocker owned by this AI (we don't want to be blocked by ourself)
			traversalProvider = new BlockManager.TraversalProvider(blockManager, BlockManager.BlockMode.AllExceptSelector, new List<SingleNodeBlocker>() { blocker });
		}
	}
}
