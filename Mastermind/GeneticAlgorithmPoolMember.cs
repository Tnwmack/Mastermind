using System;

namespace Mastermind
{
	/// <summary>
	/// Generic pool member used by the generic alogrithm to hold an item of type T and an evaluation score.
	/// </summary>
	/// <typeparam name="T">Type of pool item</typeparam>
	public struct GeneticAlgorithmPoolMember<T> : IEquatable<GeneticAlgorithmPoolMember<T>>, IComparable<GeneticAlgorithmPoolMember<T>> where T : IGeneticItem, ICloneable
	{
		/// <summary>
		/// Item being evaluated.
		/// </summary>
		public T Item { get; }

		/// <summary>
		/// Item fitness score.
		/// </summary>
		public int Score { get; }

		/// <summary>
		/// Constructor for GeneticAlgorithmPoolMember.
		/// </summary>
		/// <param name="Item"><see cref="Item"/></param>
		/// <param name="Score"><see cref="Score"/></param>
		public GeneticAlgorithmPoolMember(T Item, int Score)
		{
			this.Item = (T)Item.Clone();
			this.Score = Score;
		}

		/// <see cref="IComparable.CompareTo(object)"/>
		public int CompareTo(GeneticAlgorithmPoolMember<T> other)
		{
			return other.Score - Score;
		}

		/// <see cref="Object.Equals(object)"/>
		public override bool Equals(object obj)
		{
			if(obj is GeneticAlgorithmPoolMember<T> j)
			{
				return j.Score == Score;
			}

			return false;
		}

		/// <see cref="Object.GetHashCode"/>
		public override int GetHashCode() => Score;

		/// <see cref="IEquatable{T}.Equals(T)"/>
		public bool Equals(GeneticAlgorithmPoolMember<T> obj) => Equals((object)obj);

		/// <see cref="Object"/>
		public static bool operator ==(GeneticAlgorithmPoolMember<T> a, GeneticAlgorithmPoolMember<T> b) => a.Equals(b);
		/// <see cref="Object"/>
		public static bool operator !=(GeneticAlgorithmPoolMember<T> a, GeneticAlgorithmPoolMember<T> b) => !a.Equals(b);
		/// <see cref="Object"/>
		public static bool operator <(GeneticAlgorithmPoolMember<T> a, GeneticAlgorithmPoolMember<T> b) => a.Score < b.Score;
		/// <see cref="Object"/>
		public static bool operator >(GeneticAlgorithmPoolMember<T> a, GeneticAlgorithmPoolMember<T> b) => a.Score > b.Score;
		/// <see cref="Object"/>
		public static bool operator <=(GeneticAlgorithmPoolMember<T> a, GeneticAlgorithmPoolMember<T> b) => a.Score <= b.Score;
		/// <see cref="Object"/>
		public static bool operator >=(GeneticAlgorithmPoolMember<T> a, GeneticAlgorithmPoolMember<T> b) => a.Score >= b.Score;
	}
}
