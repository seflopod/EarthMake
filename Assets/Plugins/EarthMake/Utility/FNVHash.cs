//
// FNVHash.cs
//  
// Author:
//       Peter Bartosch <bartoschp@gmail.com>
// 
// This code is provided for the public domain.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

public class FNVHash
{
	/// <summary>
	/// Constant used in FNV hash function.
	/// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
	/// </summary>
	private readonly uint OFFSET_BASIS;
	/// <summary>
	/// Constant used in FNV hash function
	/// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
	/// </summary>
	private readonly uint FNV_PRIME;
	
	public FNVHash()
	{
		//32-bit hash
		OFFSET_BASIS = 2166136261;
		FNV_PRIME = 16777619;
	}
		
	/// <summary>
	/// Hashes three integers into a single integer using FNV hash.
	/// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
	/// </summary>
	/// <returns>hash value</returns>
	/// <param name='i'>
	/// I.
	/// </param>
	/// <param name='j'>
	/// J.
	/// </param>
	/// <param name='k'>
	/// K.
	/// </param>
	public uint Hash(uint i, uint j, uint k)
	{
		return (uint)((((((OFFSET_BASIS ^ (uint)i) * FNV_PRIME) ^ (uint)j) * FNV_PRIME) ^ (uint)k) * FNV_PRIME);
	}
}