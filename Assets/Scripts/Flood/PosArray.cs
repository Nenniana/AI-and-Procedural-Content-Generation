using System;
using System.Collections;
namespace PosArrays
{
	public struct pos
	{ //a generic position struct
		public int row;
		public int col;
		public pos(int r, int c)
		{
			row = r;
			col = c;
		}
	}
	public class PosArray<T>
	{ //a 2D array with a position indexer and 1D indexer in addition to the usual 2D indexer
		public T[,] objs;
		public T this[int row, int col]
		{
			get
			{
				return objs[row, col];
			}
			set
			{
				objs[row, col] = value;
			}
		}
		public T this[pos p]
		{
			get
			{
				return objs[p.row, p.col];
			}
			set
			{
				objs[p.row, p.col] = value;
			}
		}
		public T this[int idx]
		{
			get
			{
				return objs[idx / objs.GetLength(1), idx % objs.GetLength(1)];
			}
			set
			{
				objs[idx / objs.GetLength(1), idx % objs.GetLength(1)] = value;
			}
		}
		public IEnumerator GetEnumerator()
		{
			return objs.GetEnumerator();
		}
		public PosArray(int rows, int cols)
		{
			objs = new T[rows, cols];
		}
	}
}