using BabylonExport.Entities;

namespace RTree
{
	public interface ISpatialData
	{
		Envelope Envelope { get; }
	}

	public class TileNode : ISpatialData
	{
		public Envelope Envelope { get; set; }

		public BabylonScene Scene { get; set; }

		private void ComputeBound()
		{

		}
	}
}