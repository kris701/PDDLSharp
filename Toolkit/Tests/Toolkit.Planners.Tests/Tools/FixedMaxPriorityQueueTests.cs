using PDDLSharp.Toolkit.Planners.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Tests.Tools
{
    [TestClass]
    public class FixedMaxPriorityQueueTests
    {
        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void Can_SetSize(int size)
        {
            // ARRANGE
            // ACT
            var queue = new FixedMaxPriorityQueue<int>(size);

            // ASSERT
            Assert.AreEqual(size, queue.Size);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Size must be larger than 0!")]
        [DataRow(0)]
        [DataRow(-10)]
        public void Cant_SetSize_IfBelow1(int size)
        {
            // ARRANGE
            // ACT
            var queue = new FixedMaxPriorityQueue<int>(size);
        }

        [TestMethod]
        // Size 5
        [DataRow(5, 1, 2, 3, 4)]
        [DataRow(5, -10, 50, 2, 16)]
        [DataRow(5, -10, 50, 2, 16, 10, 4, 100)]
        // Size 1
        [DataRow(1, 1, 2, 3, 4)]
        [DataRow(1, -10, 50, 2, 16)]
        [DataRow(1, -10, 50, 2, 16, 10, 4, 100)]
        // Size 50
        [DataRow(50, 1, 2, 3, 4)]
        [DataRow(50, -10, 50, 2, 16)]
        [DataRow(50, -10, 50, 2, 16, 10, 4, 100)]
        public void Can_EnqueueWithPriority(int size, params int[] elements)
        {
            // ARRANGE
            var queue = new FixedMaxPriorityQueue<int>(size);

            // ACT
            for (int i = 0; i < elements.Length; i++)
                queue.Enqueue(elements[i], elements[i]);

            // ASSERT
            int prev = int.MaxValue;
            while (queue.Count > 0)
            {
                var element = queue.Dequeue();
                Assert.IsTrue(element < prev);
                prev = element;
            }
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, 1)]
        [DataRow(10, 5)]
        public void Can_EnqueueWithinSize(int size, int addElements)
        {
            // ARRANGE
            var queue = new FixedMaxPriorityQueue<int>(size);

            // ACT
            for (int i = 0; i < addElements; i++)
                queue.Enqueue(i, 0);

            // ASSERT
            Assert.AreEqual(addElements, queue.Count);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(-50)]
        [DataRow(500000)]
        public void Can_Dequeue(int value)
        {
            // ARRANGE
            var queue = new FixedMaxPriorityQueue<int>(1);
            queue.Enqueue(value, 0);

            // ACT
            var dequed = queue.Dequeue();

            // ASSERT
            Assert.AreEqual(value, dequed);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The queue is already empty!")]
        public void Cant_Dequeue_IfQueueEmpty()
        {
            // ARRANGE
            var queue = new FixedMaxPriorityQueue<int>(1);

            // ACT
            queue.Dequeue();
        }
    }
}
