using System;
using System.IO;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.AspNetCore.ApiHelp.Core.Test {
    public interface IPagedList<T> {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        IList<T> Items { get; }
    }

    public interface NestedGeneric<TOut, TInner> {
        int TotalCount { get; }
        int TotalPages { get; }
        IList<TOut> Items { get; }
        IPagedList<TInner> PagedList { get; set; }
    }

    public class User {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class Agenda {
        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>The template identifier.</value>
        public int TemplateId { get; set; }
        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public IList<int> Users { get; set; }
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public IList<TimeSpan> Items { get; set; }
    }

    public enum Gender {
        Unknown,
        Male,
        Female,
    }

    public class TypeJsonExtensionsTest {
        [Fact]
        public void GenericType_Scaffold_Test() {
            var type = typeof(IPagedList<User>);
            var json = type.Scaffold();

            var expected = File.ReadAllText(".\\Scaffold.json");

            Assert.Equal(expected, json.ToString());
        }

        [Fact]
        public void GenericType_Schema_Test() {
            var type = typeof(IPagedList<User>);
            var json = type.Schema();

            var expected = File.ReadAllText(".\\Schema.json");

            Assert.Equal(expected, json.ToString());
        }

        [Fact]
        public void Nested_GenericType_Scaffold_Test() {
            var type = typeof(NestedGeneric<User, User>);
            var json = type.Scaffold();

            var expected = File.ReadAllText(".\\NestedScaffold.json");

            Assert.Equal(expected, json.ToString());
        }

        [Fact]
        public void Nested_GenericType_Schema_Test() {
            var type = typeof(NestedGeneric<User, User>);
            var json = type.Schema();

            var expected = File.ReadAllText(".\\NestedSchema.json");

            Assert.Equal(expected, json.ToString());
        }

        [Fact]
        public void Class_TimeSpan_Prop_Schema_Default_Value_Test() {
            var type = typeof(Agenda);
            var json = type.Schema();

            var expected = File.ReadAllText(".\\TimeSpan.json");

            Assert.Equal(expected, json.ToString());
        }

        [Fact]
        public void Enum_Schema_Default_Value_Test() {
            var type = typeof(Gender);
            var json = type.Schema();

            Assert.Equal(0, json);
        }
    }
}
