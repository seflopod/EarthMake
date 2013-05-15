using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Text;

public static class HeightMapFileIO
{
	public static void Write(string fileName, NormalOptions no, CloudOptions co, WorleyOptions wo, Texture2D tex)
	{
		//if file already exists, will overwrite without warning
		FileStream f = File.Open(fileName+".emb", FileMode.Create);
		
		using(BinaryWriter w = new BinaryWriter(f))
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
		}
		
		if(tex != null)
		{
			byte[] png = tex.EncodeToPNG();
			for(int i=0;i<png.Length;++i)
				f.WriteByte(png[i]);
		}
		
		f.Close();
	}
	
	public static void Write(string fileName, NormalOptions no, CloudOptions co, WorleyOptions wo)
	{
		Write(fileName, no, co, wo, null);
	}
	
	public static void Read(string fileName, ref NormalOptions no, ref CloudOptions co, ref WorleyOptions wo, ref Texture2D tex)
	{
		FileStream f = File.OpenRead(fileName);
		
		using(BinaryReader r = new BinaryReader(f))
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
		}
		
		if(f.Position != f.Length)
		{
			byte[] png = new byte[f.Length - f.Position];
			for(int i=0;i<png.Length;++i)
				png[i] = (byte)f.ReadByte();
			tex.LoadImage(png);
		}
	}
}