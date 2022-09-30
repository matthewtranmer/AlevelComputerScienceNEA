using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA_GUI
{
    class ReversedLinkedList<T>
    {
        public class StorageObject
        {
            public T storage;
            public StorageObject item_before;

            public StorageObject(T storage, StorageObject item_before)
            {
                this.storage = storage;
                this.item_before = item_before;
            }
        }

        public int length { get { return _length; } }

        private int _length = 0;
        StorageObject last_item;

        public void add(T data)
        {
            StorageObject new_item = new StorageObject(data, last_item);
            last_item = new_item;
            _length++;
        }

        public T removeLast()
        {
            T old_data = last_item.storage;
            last_item = last_item.item_before;
            return old_data;
        }

        public StorageObject retrieve_object(int index)
        {
            int iterations = length - 1 - index;
            if (iterations < 0 || iterations > length - 1) throw new IndexOutOfRangeException();

            StorageObject retrieved_object = last_item;
            for (int i = 0; i < iterations; i++)
            {
                retrieved_object = retrieved_object.item_before;
            }
            return retrieved_object;
        }

        public T this[int index]
        {
            get { return retrieve_object(index).storage; }
            set { retrieve_object(index).storage = value; }
        }
    }

    class Stack<T>
    {
        public int length { get { return storage.length; } }
        ReversedLinkedList<T> storage = new ReversedLinkedList<T>();

        public void push(T item)
        {
            storage.add(item);
        }

        public T pop()
        {
            T data = storage[storage.length - 1];
            storage.removeLast();
            return data;
        }

        public T peak()
        {
            return storage[storage.length - 1];
        }
    }
}
