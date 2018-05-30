﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FclEx.Helpers;
using Xunit;

namespace FclEx.Test.Helpers
{
    public class ComparerHelperTests
    {
        public class Tester
        {
            private static int _id;
            public int Id { get; }

            public Tester()
            {
                Id = Interlocked.Increment(ref _id);
            }
        }

        private Tester[] Generate()
        {
            var random = new Random(0);
            var testers = Enumerable.Repeat((Func<Tester>)(() => new Tester()), 100)
                .Select(m => m()).OrderBy(m => random.Next()).ToArray();
            return testers;
        }

        [Fact]
        public void KeyComparer_Test()
        {
            var testers = Generate();
            var testersOrdered = testers.OrderBy(m => m.Id).ToArray();
            var comparer = ComparerHelper.Create<Tester, int>(m => m.Id);
            var sortList = new SortedSet<Tester>(testers, comparer);
            Assert.True(testersOrdered.SequenceEqual(sortList));
        }

        [Fact]
        public void CommonComparer_Test()
        {
            var testers = Generate();
            var testersOrdered = testers.OrderBy(m => m.Id).ToArray();
            var comparer = ComparerHelper.Create<Tester>((x, y) => Comparer<int>.Default.Compare(x.Id, y.Id));
            var sortList = new SortedSet<Tester>(testers, comparer);
            Assert.True(testersOrdered.SequenceEqual(sortList));
        }
    }
}
