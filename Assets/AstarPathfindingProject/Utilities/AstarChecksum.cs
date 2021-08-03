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
using System;
namespace Pathfinding.Util {
	/// <summary>Calculates checksums of byte arrays</summary>
	public class Checksum {
		/// <summary>
		/// Calculate checksum for the byte array starting from a previous values.
		/// Useful if data is split up between several byte arrays
		/// </summary>
		public static uint GetChecksum (byte[] arr, uint hash) {
			// Sort of implements the Fowler–Noll–Vo hash function
			const int prime = 16777619;

			hash ^= 2166136261U;

			for (int i = 0; i < arr.Length; i++)
				hash = (hash ^ arr[i]) * prime;

			return hash;
		}
	}
}
