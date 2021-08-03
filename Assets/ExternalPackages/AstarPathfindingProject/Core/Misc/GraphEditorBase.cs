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
using Pathfinding.Serialization;

namespace Pathfinding {
	[JsonOptIn]
	/// <summary>Defined here only so non-editor classes can use the <see cref="target"/> field</summary>
	public class GraphEditorBase {
		/// <summary>NavGraph this editor is exposing</summary>
		public NavGraph target;
	}
}
