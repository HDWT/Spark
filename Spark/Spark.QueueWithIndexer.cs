using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Spark
{
	private class QueueWithIndexer<T>
	{
		private const int ArraySize = 32;

		private List<T[]> m_list = null;

		private T[] m_firstArray = null;
		private T[] m_lastArray = null;

		private int m_firstIndexInList = 0;
		private int m_firstIndexInArray = 0;

		private int m_lastIndexInList = 0;
		private int m_lastIndexInArray = 0;

		private int m_count = 0;
		private int m_capacity = 0;

		public QueueWithIndexer()
		{
			m_list = new List<T[]>(4);

			m_firstArray = new T[ArraySize];
			m_lastArray = m_firstArray;

			m_list.Add(m_firstArray);
		}

		public int Count
		{
			get { return m_count; }
		}

		public int Capacity
		{
			get { return m_capacity; }
		}

		public T this[int index]
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

		public void Enqueue(T value)
		{
			m_lastArray[m_lastIndexInArray++] = value;

			if (m_lastIndexInArray == ArraySize)
			{
				m_lastArray = new T[ArraySize];
				m_list.Add(m_lastArray);

				m_lastIndexInList++;
				m_lastIndexInArray = 0;
			}

			m_count++;
			m_capacity++;
		}

		public T Dequeue()
		{
			T value = m_firstArray[m_firstIndexInArray++];

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

	private class ObjectPool<T> where T : class, new()
	{
		private QueueWithIndexer<T> m_objects = new QueueWithIndexer<T>();

		public T Get()
		{
			lock (m_objects)
			{
				if (m_objects.Count == 0)
					m_objects.Enqueue(new T());

				return m_objects.Dequeue();
			}
		}

		public void Return(T obj)
		{
			lock (m_objects)
			{
				if (obj != null)
					m_objects.Enqueue(obj);
			}
		}
	}
}