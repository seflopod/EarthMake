//
// MeshGenerator.cs
//  
// Author:
//       Peter Bartosch <bartoschp@gmail.com>
// 
// This software is provided for the public domain.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using UnityEngine;
using System.Collections.Generic;

public static class MeshGenerator
{
	/// <summary>
	/// Creates the new mesh plane with normals, uvs, and tangents.
	/// </summary>
	/// <returns>
	/// The new unoptimized mesh plane.
	/// </returns>
	/// <param name='height'>
	/// Plane height.
	/// </param>
	/// <param name='width'>
	/// Plane width.
	/// </param>
	/// <param name='showSeams'>
	/// Whether or not the new mesh should show its seams.  That is, should the
	/// vertex normals be calculated as the average of all face normals
	/// (<c>false</c>), or should each vertex normal be the normal for a single
	/// face (<c>true</c>).
	/// </param>
	public static Mesh CreateNewMeshPlane(uint height, uint width, bool showSeams)
	{
		//Center mesh on origin
		float xMax = width / 2.0f;
		float zMax = height / 2.0f;
		float xMin = -xMax;
		float zMin = -zMax;
		
		//Create lists for adding to mesh
		List<Vector3> allVerts = new List<Vector3>();
		List<Vector3> vertLst = new List<Vector3>();
		List<Vector3> nrmlLst = new List<Vector3>();
		List<Vector2> uvLst = new List<Vector2>();
		List<int> triLst = new List<int>();

		//create the verts for the mesh.
		for(float z=zMax; z>=zMin; --z)
		{
			for(float x=xMax; x>=xMin; --x)
			{
				allVerts.Add(new Vector3(x, 0.0f, z));
			}
		}
		
		if(showSeams)
		{
			buildListsRough(height, width, ref allVerts, ref vertLst, ref nrmlLst, ref uvLst, ref triLst);
		}
		else
		{
			buildListsSmooth(height, width, ref allVerts, ref vertLst, ref nrmlLst, ref uvLst, ref triLst);
		}
		
		Mesh ret = new Mesh();
		ret.vertices = vertLst.ToArray();
		ret.triangles = triLst.ToArray();
		ret.normals = nrmlLst.ToArray();
		ret.uv = uvLst.ToArray();
		ret.tangents = calculateMeshTangents(ret);
		return ret;
	}
	
	public static Mesh CreateNewMeshPlane(uint height, uint width)
	{
		return CreateNewMeshPlane(height, width, false);
	}

	/// <summary>
	/// Builds the vert, normal, uv, and tri lists for a mesh that is "smooth".
	/// </summary>
	/// <param name="height">Height.</param>
	/// <param name="width">Width.</param>
	/// <param name="allVerts">All verts.</param>
	/// <param name="vertLst">Vert lst.</param>
	/// <param name="nrmlLst">Nrml lst.</param>
	/// <param name="uvLst">Uv lst.</param>
	/// <param name="triLst">Tri lst.</param>
	private static void buildListsSmooth(uint height, uint width,
													ref List<Vector3> allVerts,
													ref List<Vector3> vertLst,
													ref List<Vector3> nrmlLst,
													ref List<Vector2> uvLst,
													ref List<int> triLst)
	{
		vertLst = allVerts;
		for(int r=0; r<height; ++r)
		{
			for(int c=0; c<width; ++c)
			{
				int tl = r * (int)(height + 1) + c;
				int tr = tl + 1;
				int bl = tl + (int)height + 1;
				int br = bl + 1;
				
				if(((r + c) & 1) == 0) //r+c is even
				{
					/*
					 * Do Bottom-Left Triangle
					 */
					triLst.Add(tl);
					triLst.Add(bl);
					triLst.Add(br);
					
					/*
					 * Do Upper-Right Triangle
					 */
					triLst.Add(br);
					triLst.Add(tr);
					triLst.Add(tl);
				}
				else //r+c is odd
				{
					/*
					 * Do Upper-Left Triangle
					 */
					triLst.Add(tl);
					triLst.Add(bl);
					triLst.Add(tr);
					
					/*
					 * Do Bottom-Right Triangle
					 */
					triLst.Add(tr);
					triLst.Add(bl);
					triLst.Add(br);
				}
			}
		}
		
		foreach(Vector3 vert in vertLst)
		{
			//normals are calculated later, for now just use up.
			nrmlLst.Add(Vector3.up);

			//all uvs are based on the vert
			uvLst.Add(new Vector2(vert.x, vert.z));
		}
	}

	/// <summary>
	/// Builds the vert, normal, uv, and tri lists for a mesh that is "rough".
	/// </summary>
	/// <param name="height">Height.</param>
	/// <param name="width">Width.</param>
	/// <param name="allVerts">All verts.</param>
	/// <param name="vertLst">Vert lst.</param>
	/// <param name="nrmlLst">Nrml lst.</param>
	/// <param name="uvLst">Uv lst.</param>
	/// <param name="triLst">Tri lst.</param>
	/// <description>
	/// Since the mesh needs to show its seams, we will create duplicate verts
	/// for every triangle.  This will matter when calculating the normals,
	/// as the normal is based on all faces that use a given vert.
	/// </description>
	private static void buildListsRough(uint height, uint width,
													ref List<Vector3> allVerts,
													ref List<Vector3> vertLst,
													ref List<Vector3> nrmlLst,
													ref List<Vector2> uvLst,
													ref List<int> triLst)
	{
		int nTris = 0;
		for(int r=0; r<height; ++r)
		{
			for(int c=0; c<width; ++c)
			{
				int tl = r * (int)(height + 1) + c;
				int tr = tl + 1;
				int bl = tl + (int)height + 1;
				int br = bl + 1;
				
				if(((r + c) & 1) == 0) //r+c is even
				{
					/*
					 * Do Bottom-Left Triangle
					 */
					vertLst.Add(allVerts[tl]);
					vertLst.Add(allVerts[bl]);
					vertLst.Add(allVerts[br]);
					
					//add uvs
					uvLst.Add(new Vector2(allVerts[tl].x, allVerts[tl].z));
					uvLst.Add(new Vector2(allVerts[bl].x, allVerts[bl].z));
					uvLst.Add(new Vector2(allVerts[br].x, allVerts[br].z));
					
					/*
					 * Do Upper-Right Triangle
					 */
					vertLst.Add(allVerts[br]);
					vertLst.Add(allVerts[tr]);
					vertLst.Add(allVerts[tl]);
					
					//add uvs
					uvLst.Add(new Vector2(allVerts[br].x, allVerts[br].z));
					uvLst.Add(new Vector2(allVerts[tr].x, allVerts[tr].z));
					uvLst.Add(new Vector2(allVerts[tl].x, allVerts[tl].z));
				}
				else //r+c is odd
				{
					/*
					 * Do Upper-Left Triangle
					 */
					vertLst.Add(allVerts[tl]);
					vertLst.Add(allVerts[bl]);
					vertLst.Add(allVerts[tr]);
					
					//add uvs
					uvLst.Add(new Vector2(allVerts[tl].x, allVerts[tl].z));
					uvLst.Add(new Vector2(allVerts[bl].x, allVerts[bl].z));
					uvLst.Add(new Vector2(allVerts[tr].x, allVerts[tr].z));
					
					/*
					 * Do Bottom-Right Triangle
					 */
					vertLst.Add(allVerts[tr]);
					vertLst.Add(allVerts[bl]);
					vertLst.Add(allVerts[br]);
					
					//add uvs
					uvLst.Add(new Vector2(allVerts[tr].x, allVerts[tr].z));
					uvLst.Add(new Vector2(allVerts[bl].x, allVerts[bl].z));
					uvLst.Add(new Vector2(allVerts[br].x, allVerts[br].z));
				}

				//normal and tri index for all six new vertices
				for(int i=0; i<6; ++i)
				{
					//we'll adjust the normals later, for now just use Up
					nrmlLst.Add(Vector3.up);

					//since we just added the verts for the next two tris, we
					//only need to use the last six indices.
					triLst.Add(nTris++);
				}
			}
		}
	}
	
	private static void mapFaceToVert(ref Dictionary<int, HashSet<int>> vertsToFaces, int vertIdx, int faceIdx)
	{
		HashSet<int> faceSet;
		vertsToFaces.TryGetValue(vertIdx, out faceSet);
		if(faceSet == null)
		{ //did not find vertex
			faceSet = new HashSet<int>();
		}
		
		//no matter what, add the faceIdx to the face set and create new
		//Dictionary entry
		faceSet.Add(faceIdx);
		vertsToFaces.Add(vertIdx, faceSet);
	}
	
	public static Vector4[] calculateMeshTangents(Mesh mesh)
	{
		//speed up math by copying the mesh arrays
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;
	 
		//variable definitions
		int triangleCount = triangles.Length;
		int vertexCount = vertices.Length;
	 
		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
	 
		Vector4[] tangents = new Vector4[vertexCount];
	 
		for(long a = 0; a < triangleCount; a += 3)
		{
			long i1 = triangles[a + 0];
			long i2 = triangles[a + 1];
			long i3 = triangles[a + 2];
	 
			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];
	 
			Vector2 w1 = uv[i1];
			Vector2 w2 = uv[i2];
			Vector2 w3 = uv[i3];
	 
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
	 
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
	 
			float div = (s1 * t2 - s2 * t1);
			float r = (div == 0.0f) ? 0.0f : (1.0f / div);
	 
			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
	 
			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;
	 
			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}
	 
	 
		for(long a = 0; a < vertexCount; ++a)
		{
			Vector3 n = normals[a];
			Vector3 t = tan1[a];
			Vector3.OrthoNormalize(ref n, ref t);
			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;
	 
			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}
	 
		return tangents;
	}
	
}

