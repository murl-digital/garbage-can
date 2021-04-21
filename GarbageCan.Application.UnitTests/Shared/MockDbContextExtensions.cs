using GarbageCan.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GarbageCan.Application.UnitTests.Shared
{
    public static class MockDbContextExtensions
    {
        /// <summary>
        /// Will Configure the db context db set with a mocked Db Set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setFunc"></param>
        /// <param name="context"></param>
        /// <returns>The Mocked Db Set</returns>
        public static DbSet<T> ConfigureAsMock<T>(this IApplicationDbContext context,
            Func<IApplicationDbContext, DbSet<T>> setFunc) where T : class
        {
            var mockDbSet = Array.Empty<T>().AsQueryable().BuildMockDbSet();
            setFunc(context).Returns(x => mockDbSet);
            return mockDbSet;
        }

        /// <summary>
        /// Will Configure the db context db set with a mocked Db Set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setFunc"></param>
        /// <param name="context"></param>
        /// <param name="populatedDbSetItems">Items to be put into the mock DB set</param>
        /// <returns>The Mocked Db Set</returns>
        public static DbSet<T> ConfigureAsMock<T>(this IApplicationDbContext context,
            Func<IApplicationDbContext, DbSet<T>> setFunc,
            IEnumerable<T> populatedDbSetItems) where T : class
        {
            var mockDbSet = populatedDbSetItems.AsQueryable().BuildMockDbSet();
            setFunc(context).Returns(x => mockDbSet);
            return mockDbSet;
        }

        /// <summary>
        /// Will Configure the db context db set with a mocked Db Set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setFunc"></param>
        /// <param name="item">Item to be put into the mock DB set</param>
        /// <param name="context"></param>
        /// <returns>The Mocked Db Set</returns>
        public static DbSet<T> ConfigureAsMock<T>(this IApplicationDbContext context,
            Func<IApplicationDbContext, DbSet<T>> setFunc,
            T item) where T : class
        {
            var mockDbSet = new[] { item }.AsQueryable().BuildMockDbSet();
            setFunc(context).Returns(x => mockDbSet);
            return mockDbSet;
        }
    }
}