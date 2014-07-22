using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Spark
{
	private class QueueWithIndexer
	{
		private const int ArraySize = 32;

		private List<int[]> m_list = null;

		private int[] m_firstArray = null;
		private int[] m_lastArray = null;

		private int m_firstIndexInList = 0;
		private int m_firstIndexInArray = 0;

		private int m_lastIndexInList = 0;
		private int m_lastIndexInArray = 0;

		private int m_count = 0;

		public QueueWithIndexer()
		{
			m_list = new List<int[]>(4);

			m_firstArray = new int[ArraySize];
			m_lastArray = m_firstArray;

			m_list.Add(m_firstArray);
		}

		public int Count
		{
			get { return m_count; }
		}

		public int this[int index]
		{
			get
			{
				int listIndex = index / ArraySize + m_firstIndexInList;
				int arrayIndex = index % ArraySize + m_firstIndexInArray;

				return m_list[listIndex][arrayIndex];
			}
			set
			{
				int listIndex = index / ArraySize + m_firstIndexInList;
				int arrayIndex = index % ArraySize + m_firstIndexInArray;

				m_list[listIndex][arrayIndex] = value;
			}
		}

		public void Enqueue(int value)
		{
			m_lastArray[m_lastIndexInArray++] = value;

			if (m_lastIndexInArray == ArraySize)
			{
				m_lastArray = new int[ArraySize];
				m_list.Add(m_lastArray);

				m_lastIndexInList++;
				m_lastIndexInArray = 0;
			}

			m_count++;
		}

		public int Dequeue()
		{
			int value = m_firstArray[m_firstIndexInArray++];

			if (m_firstIndexInArray == ArraySize)
			{
				m_firstIndexInList++;
				m_firstIndexInArray = 0;

				m_firstArray = m_list[m_firstIndexInList];
			}

			m_count--;

			return value;
		}
	}
}