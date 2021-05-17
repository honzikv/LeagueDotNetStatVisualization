using System;
using System.Collections.Generic;
using System.Linq;

namespace dotNetMVCLeagueApp.Utils {
    
    /// <summary>
    /// Jednoduchy wrapper okolo Queue objektu, pro ukladani poslednich N objektu
    /// do seznamu. Implementovano pomoci Queue
    /// </summary>
    /// <typeparam name="T">Typ objektu</typeparam>
    public class FixedSizeQueue<T> {
        
        /// <summary>
        /// Maximalni pocet prvku
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Queue pro ukladani prvku
        /// </summary>
        private Queue<T> queue;

        /// <summary>
        /// Konstruktor - ulozi si max pocet prvku a vytvori interni queue
        /// </summary>
        /// <param name="size">maximalni pocet prvku</param>
        public FixedSizeQueue(int size) {
            if (size <= 0) {
                throw new ArgumentException("Size must be greater than zero");
            }
            
            Size = size;
            queue = new Queue<T>();
        }

        /// <summary>
        /// Prida prvek na konec
        /// </summary>
        /// <param name="item">reference na prvek</param>
        public void Enqueue(T item) {
            queue.Enqueue(item);
            if (queue.Count > Size) {
                queue.Dequeue();
            }
        }

        /// <summary>
        /// Pro snazsi manipulaci muzeme prevest na seznam
        /// </summary>
        /// <returns>Novou instanci List pro interni queue</returns>
        public List<T> ToList() => queue.ToList();
    }
}