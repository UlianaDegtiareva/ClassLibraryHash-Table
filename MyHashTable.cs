using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryLaba10;

namespace ClassLibraryHash_Table
{
    public class MyHashTable<T> where T : IInit, IComparable, ICloneable, new()
    {
        public T[] table;
        public int count = 0;
        public double fillRatio; //коэф. заполняемости таблицы

        public int Capacity => table.Length; //емкость, кол-во выделенной памяти
        public int Count => count;

        public MyHashTable(int size, double fillRatio)
        {
            table = new T[size];
            this.fillRatio = fillRatio;
        }

        public MyHashTable() { }

        public MyHashTable(MyHashTable<T> c)
        {
            if (c == null)
                throw new Exception("empty collection: null");
            if (c.Count == 0) throw new Exception("empty collection");
            // Устанавливаем коэффициент заполняемости по умолчанию
            int initialCapacity = c.Count;
            // Устанавливаем коэффициент заполняемости по умолчанию
            double defaultFillRatio = 0.72;

            // Инициализируем таблицу начальной емкостью
            table = new T[initialCapacity];
            fillRatio = defaultFillRatio;

            // Добавляем каждый элемент из коллекции в хэш-таблицу
            for (int i = 0; i < initialCapacity; i++)
            {
                if (c[i] != null)
                {
                    AddItem((T)c[i].Clone());
                }
            }
        }

        public MyHashTable(params T[] collection)
        {
            if (collection == null)
                throw new Exception("empty collection: null");
            if (collection.Length == 0) throw new Exception("empty collection");
            // Устанавливаем коэффициент заполняемости по умолчанию
            int initialCapacity = collection.Length;
            // Устанавливаем коэффициент заполняемости по умолчанию
            double defaultFillRatio = 0.72;

            // Инициализируем таблицу начальной емкостью
            table = new T[initialCapacity];
            fillRatio = defaultFillRatio;

            // Добавляем каждый элемент из коллекции в хэш-таблицу
            for (int i = 0; i < initialCapacity; i++)
            {
                if (collection[i] != null)
                {
                    AddItem((T)collection[i].Clone());
                }
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= table.Length)
                {
                    throw new IndexOutOfRangeException("Index is out of range");
                }
                return table[index];
            }
            set
            {
                if (index < 0 || index >= table.Length)
                {
                    throw new IndexOutOfRangeException("Index is out of range");
                }
                table[index] = value;
            }
        }

        public void FillTable(int numberOfItems)
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                T newItem = new T();
                newItem.RandomInit();
                AddItem(newItem);
            }
        }

        /// <summary>
        /// Удаление элементов из таблицы
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool RemoveData(T data)
        {
            if (count == 0)
            {
                throw new Exception("Невозможно удалить элемент из пустой таблицы.");
            }

            bool removed = false; // Флаг указывающий, был ли удалён элемент
            int index = FindItem(data);

            while (index >= 0)
            {
                count--;
                removed = true;
                table[index] = default;
                index = FindItem(data); // Ищем следующий элемент с похожим содержанием
            }
            return removed;
        }

        public bool Contains(T data)
        {
            return FindItem(data) >= 0;
        }

        /// <summary>
        /// Печать таблицы
        /// </summary>
        public void Print()
        {
            try
            {
                if (table.All(item => item == null))
                {
                    throw new Exception("Таблица пуста, нет элементов для вывода.");
                }
                int i = 0;
                foreach (T item in table)
                {
                    if (item != null)
                    {
                        Console.WriteLine($"{i} : {item}, {GetIndex(item)} ");
                        i++;
                    }
                    else
                    {
                        Console.WriteLine($"{i} : {item}");
                        i++;
                    }

                }
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

        /// <summary>
        /// Добавление элемента в таблицу
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(T item)
        {
            if ((double)Count / Capacity > fillRatio)
            {
                //увеличиваем таблицу в 2 раза и переписываем всю информацию
                T[] temp = (T[])table.Clone();
                table = new T[temp.Length * 2];
                count = 0;
                for (int i = 0; i < temp.Length; i++)
                    AddData(temp[i]);
            }
            AddData(item);
        }

        /// <summary>
        /// Вычисление индекса
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int GetIndex(T data)
        {
            return Math.Abs(data.GetHashCode()) % Capacity;
        }

        //добавление элемента в таблицу
        public void AddData(T data)
        {
            if (data == null)
            {
                // Обработка случая, когда data равен null
                return;
            }
            else
            {
                int index = GetIndex(data);
                int current = index;

                if (Contains(data))
                {
                    return;
                }

                if (table[index] != null)
                {
                    while (current < table.Length && table[current] != null)
                        current++;
                    if (current == table.Length)
                    {
                        current = 0;
                        while (current < index && table[current] != null)
                            current++;
                        if (current == index)
                        {
                            throw new Exception("Нет места в таблице");
                        }
                    }
                }
                //место найдено
                table[current] = data;
                count++;
            }
        }

        public int FindItem(T data)
        {
            int index = GetIndex(data);

            if (table[index] != null && table[index].Equals(data))
            {
                return index; // Искомый элемент найден по вычисленному индексу
            }

            for (int i = 1; i < table.Length; i++)
            {
                int currentIndex = (index + i) % Capacity; // Вычисляем текущий индекс с учетом смещения

                if (table[currentIndex] == null)
                {
                    continue; // Пропускаем нулевые ячейки
                }

                if (table[currentIndex].Equals(data))
                {
                    return currentIndex; // Искомый элемент найден по смещенному индексу
                }
            }

            return -1; // Элемент не найден в таблице
        }
    }
}
