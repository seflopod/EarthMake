// 
// HeightMapSerializer.cs
//  
// Author:
//       Peter Bartosch <bartoschp@gmail.com>
// 
// Copyright (c) 2013 Peter Bartosch
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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
								WorleyOptions wo, Texture2D tex)
	{
		//if file already exists, will overwrite without warning
		using(BinaryWriter w = new BinaryWriter(File.Open(fileName+".emb",
												FileMode.Create)))
		{
			w.Write(no.size);
			w.Write(no.seed);
			w.Write(no.multiplier);
			w.Write(no.cloudInf);
			w.Write(no.worleyInf);
			w.Write(no.showSeams);
			
			w.Write(co.upperLeftStart);
			w.Write(co.lowerLeftStart);
			w.Write(co.lowerRightStart);
			w.Write(co.upperRightStart);
			
			w.Write((int)wo.metric);
			w.Write((int)wo.combiner);
			w.Write(wo.zoomLevel);
			
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
								WorleyOptions wo)
	{
		Write(fileName, no, co, wo, null);
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
							ref CloudOptions co, ref WorleyOptions wo,
							ref Texture2D tex)
	{
		using(BinaryReader r = new BinaryReader(File.OpenRead(fileName)))
		{
			no.size = r.ReadInt32();
			no.seed = r.ReadInt32();
			no.multiplier = r.ReadSingle();
			no.cloudInf = r.ReadSingle();
			no.worleyInf = r.ReadSingle();
			no.showSeams = r.ReadBoolean();
			
			co.upperLeftStart = r.ReadSingle();
			co.lowerLeftStart = r.ReadSingle();
			co.lowerRightStart = r.ReadSingle();
			co.upperRightStart = r.ReadSingle();
			
			wo.metric = (WorleyNoise.DistanceMetric)r.ReadInt32();
			wo.combiner = (WorleyNoise.CombineType)r.ReadInt32();
			wo.zoomLevel = r.ReadSingle();
	
			tex.Resize(no.size, no.size);
			int bLeft = (int)(r.BaseStream.Length - r.BaseStream.Position);
			if(bLeft > 0)
				tex.LoadImage(r.ReadBytes(bLeft));
		}
	}
}