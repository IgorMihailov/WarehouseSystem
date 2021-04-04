using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse
{
    // Пример паттерна "Итератор" - итерируем по продуктам в магазине
    class Customer
    {
        public void SeeProducts(Store store)
        {
            IProductIterator iterator = store.CreateNumerator();
            while (iterator.HasNext())
            {
                Product product = iterator.Next();
                Console.WriteLine("Наименование: " + product.Name + ", цена: " + new Random().Next(10, 500));
            }
        }
    }

    interface IProductIterator
    {
        bool HasNext();
        Product Next();
    }

    interface IProductNumerable
    {
        IProductIterator CreateNumerator();
        int Count { get; }
        Product this[int index] { get; }
    }

    class Store : IProductNumerable
    {
        private Product[] Products;
        public Store()
        {
            Products = new Product[]
            {
                new Product("Бананы", 10, 10),
                new Product ("Яблоки", 20, 20),
                new Product ("Арбузы", 30, 30)
            };
        }

        public int Count
        {
            get { return Products.Length; }
        }

        public Product this[int index]
        {
            get { return Products[index]; }
        }

        public IProductIterator CreateNumerator()
        {
            return new StoreNumerator(this);
        }
    }
    class StoreNumerator : IProductIterator
    {
        IProductNumerable aggregate;
        int index = 0;
        public StoreNumerator(IProductNumerable a)
        {
            aggregate = a;
        }

        public bool HasNext()
        {
            return index < aggregate.Count;
        }

        public Product Next()
        {
            return aggregate[index++];
        }
    }
}
