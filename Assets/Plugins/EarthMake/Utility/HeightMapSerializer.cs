// 
// HeightMapSerializer.cs
//  
// Author:
//       Peter Bartosch <bartoschp@gmail.com>

using UnityEngine;
using System.IO;
using System.Text;

/// <summary>
/// Static class for reading and writing a binary file containing heightmap data
/// and parameters.
/// </summary>
/// <description>
/// TODO: check into compression for these files.  This may reduce file sizes
/// for large heightmaps.
/// </description>
public static class HeightMapFileIO
{
	/// <summary>
	/// Write the specified the heightmap and all paramaters to the provided
	/// file name.
	/// </summary>
	/// <param name='fileName'>
	/// The file name.  This can be relative or fully-qualified.
	/// </param>
	/// <param name='no'>
	/// The <see cref="NormalOptions" /> to write.
	/// </param>
	/// <param name='co'>
	/// The <see cref="CloudOptions" /> to write.
	/// </param>
	/// <param name='wo'>
	/// The <see cref="WorleyOptions" /> to write.
	/// </param>
	/// <param name='tex'>
	/// The <code>Texture2D</code> containing the heightmap data.
	/// </param>
	/// <description>
	/// This uses a <code>BinaryWriter</code> to convert all data into binary
	/// data for writing.
	/// </description>
	public static void Write(string fileName, NormalOptions no, CloudOptions co,
								VoronoiOptions vo, Texture2D tex)
	{
		//if file already exists, will overwrite without warning
		using(BinaryWriter w = new BinaryWriter(File.Open(fileName+".emb",
												FileMode.Create)))
		{
			w.Write(no.size);
			w.Write(no.seed);
			w.Write(no.multiplier);
			w.Write(no.cloudInf);
			w.Write(no.voronoiInf);
			w.Write(no.showSeams);
			
			w.Write(co.upperLeftStart);
			w.Write(co.lowerLeftStart);
			w.Write(co.lowerRightStart);
			w.Write(co.upperRightStart);
			
			w.Write((int)vo.metric);
			w.Write((int)vo.combiner);
			w.Write(vo.numberOfFeaturePoints);
			w.Write(vo.numberOfSubregions);
			
			if(tex != null)
				w.Write(tex.EncodeToPNG());
		}
	}
	
	/// <summary>
	/// Overload for <see cref="Write" /> that does not have texture data.
	/// </summary>
	/// <param name='fileName'>
	/// The file name.  This can be relative or fully-qualified.
	/// </param>
	/// <param name='no'>
	/// The <see cref="NormalOptions" /> to write.
	/// </param>
	/// <param name='co'>
	/// The <see cref="CloudOptions" /> to write.
	/// </param>
	/// <param name='wo'>
	/// The <see cref="WorleyOptions" /> to write.
	/// </param>
	/// <param name='tex'>
	/// The <code>Texture2D</code> containing the heightmap data.
	/// </param>
	public static void Write(string fileName, NormalOptions no, CloudOptions co,
	                         VoronoiOptions vo)
	{
		Write(fileName, no, co, vo, null);
	}
	
	/// <summary>
	/// Reads from the provided file name all parameters and data for a
	/// heightmap.  If the data for the heightmap does not exist, then
	/// no data is written to the provided texture.
	/// </summary>
	/// <param name='fileName'>
	/// The file name.  This can be relative or fully-qualified.
	/// </param>
	/// <param name='no'>
	/// The <see cref="NormalOptions" /> that will store read-in parameters.
	/// </param>
	/// <param name='co'>
	/// The <see cref="CloudOptions" /> that will store read-in parameters for
	/// <see cref="CloudFractal" />.
	/// </param>
	/// <param name='wo'>
	/// The <see cref="WorleyOptions" />  that will store read-in parameters for
	/// <see cref="WorleyNoise" />.
	/// </param>
	/// <param name='tex'>
	/// The <code>Texture2D</code> containing the heightmap data.
	/// </param>
	public static void Read(string fileName, ref NormalOptions no,
	                        ref CloudOptions co, ref VoronoiOptions vo,
							ref Texture2D tex)
	{
		using(BinaryReader r = new BinaryReader(File.OpenRead(fileName)))
		{
			no.size = r.ReadInt32();
			no.seed = r.ReadInt32();
			no.multiplier = r.ReadSingle();
			no.cloudInf = r.ReadSingle();
			no.voronoiInf = r.ReadSingle();
			no.showSeams = r.ReadBoolean();
			
			co.upperLeftStart = r.ReadSingle();
			co.lowerLeftStart = r.ReadSingle();
			co.lowerRightStart = r.ReadSingle();
			co.upperRightStart = r.ReadSingle();
			
			vo.metric = (DistanceFuncs.DistanceMetric)r.ReadInt32();
			vo.combiner = (CombinerFunctions.CombineFunction)r.ReadInt32();
			vo.numberOfFeaturePoints = r.ReadInt32();
			vo.numberOfSubregions = r.ReadInt32();
	
			tex.Resize(no.size, no.size);
			int bLeft = (int)(r.BaseStream.Length - r.BaseStream.Position);
			if(bLeft > 0)
				tex.LoadImage(r.ReadBytes(bLeft));
		}
	}
}