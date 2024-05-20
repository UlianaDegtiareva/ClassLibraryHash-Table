using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryLaba10;
using static System.Diagnostics.Activity;

namespace ClassLibraryHash_Table
{
    public class MyCollection_HashTable<T> : MyHashTable<T>, ICollection<T>, IEnumerable<T> where T : IInit, IComparable, ICloneable, new()
    {
        public bool IsReadOnly => throw new NotImplementedException();

        public MyCollection_HashTable() : base() { }
        public MyCollection_HashTable(int size, double fillRatio) : base(size, fillRatio) { }
        public MyCollection_HashTable(MyCollection_HashTable<T> c) : base(c) { }
        public MyCollection_HashTable(params T[] collection) : base(collection) { }


        public new void Print()
        {
            base.Print();
        }

        public new bool Contains(T data)
        {
            return base.Contains(data);
        }

        public MyCollection_HashTable<T> CopyTable()
        {
            MyCollection_HashTable<T> copiedTable = new MyCollection_HashTable<T>(Capacity, fillRatio);

            // Копируем элементы из текущей таблицы в новую таблицу
            for (int i = 0; i < Capacity; i++)
            {
                copiedTable.table[i] = table[i];
            }

            return copiedTable;
        }

        public MyCollection_HashTable<T> DeepCopy()
        {
            // Создаем массив для копирования элементов
            T[] array = new T[Count];

            // Используем CopyTo для копирования элементов
            CopyTo(array, 0);

            // Создаем новую таблицу из скопированных элементов
            MyCollection_HashTable<T> copiedTable = new MyCollection_HashTable<T>(array);

            return copiedTable;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new MyEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Add(T item)
        {
            AddItem(item);
        }

        public void FillCollection(int numberOfItems)
        {
            FillTable(numberOfItems);
        }

        public void Clear()
        {
            // Очищаем все элементы в массиве, присваивая им значение по умолчанию для типа T
            Array.Clear(table, 0, table.Length);

            // Сбрасываем счетчик элементов на 0
            count = 0;

            // Переинициализируем таблицу с начальной емкостью
            table = new T[Capacity];
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            // Проверка, что массив не является null
            if (array == null)
            {
                throw new Exception("Массив пуст");
            }

            // Проверка, что в массиве достаточно места для копирования элементов
            if (array.Length - arrayIndex < Count)
            {
                throw new Exception("Для копирования элементов в массиве недостаточно места");
            }

            // Проверка, что индекс arrayIndex находится в пределах допустимого диапазона
            if (arrayIndex < 0 || arrayIndex >= array.Length)
            {
                throw new Exception($"Индекс, с которого необходимо начать копирование, выходит за пределы массива: [0, {array.Length}]");
            }

            // Начальный индекс для копирования элементов из коллекции в массив
            int ind = arrayIndex;

            // Установка значений до arrayIndex в массиве в значение по умолчанию для типа T
            for (int i = 0; i < arrayIndex; i++)
            {
                array[i] = default(T);
            }

            // Копирование элементов из коллекции в массив, начиная с arrayIndex
            foreach (T item in this)
            {
                array[ind++] = (T)item.Clone();
            }

            // Установка значений после скопированных элементов в значение по умолчанию для типа T
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = default(T);
            }
        }

        public bool Remove(T item)
        {
            return RemoveData(item);
        }
    }
    public class MyEnumerator<T> : IEnumerator<T> where T : IInit, IComparable, ICloneable, new()
    {
        MyCollection_HashTable<T> collection;
        int position = -1; // Текущая позиция перечислителя

        public MyEnumerator(MyCollection_HashTable<T> collection)
        {
            this.collection = collection;
            position = -1; // Устанавливаем начальную позицию на 0
        }
        public T Current => collection.table[position];

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            // Метод Dispose пустой, так как в данном случае нет необходимости освобождать ресурсы
        }

        // Перемещаемся к следующему элементу в коллекции
        public bool MoveNext()
        {
            position++;
            while (position < collection.Capacity && collection.table[position] == null)
            {
                position++;
            }
            if (position >= collection.Capacity)
            {
                Reset(); // Сбросить перечислитель к началу, если достигнут конец коллекции
                return false;
            }
            return true;
        }

        // Сбрасываем перечислитель к началу
        public void Reset()
        {
            position = -1;
        }
    }
}
