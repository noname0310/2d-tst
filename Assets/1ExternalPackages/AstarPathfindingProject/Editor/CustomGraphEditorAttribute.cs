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
namespace Pathfinding {
	/// <summary>Added to editors of custom graph types</summary>
	[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public class CustomGraphEditorAttribute : System.Attribute {
		/// <summary>Graph type which this is an editor for</summary>
		public System.Type graphType;

		/// <summary>Name displayed in the inpector</summary>
		public string displayName;

		/// <summary>Type of the editor for the graph</summary>
		public System.Type editorType;

		public CustomGraphEditorAttribute (System.Type t, string displayName) {
			graphType = t;
			this.displayName = displayName;
		}
	}
}
