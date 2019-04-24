﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmLib.IoC.Tests
{
    public interface IItem
    {
 
    }

    public class Item : IItem
    {
        public string myString = "my default value";

        public string DoSomething()
        {
            return "Ok";
        }

    }

    public class ItemWithParameter : IItem
    {
        public Item item;

        public ItemWithParameter(Item item)
        {
            this.item = item;
        }
    }

    public interface IItem2
    {

    }

    public class Item2 : IItem2
    {
        public string myString;

        public Item2()
        {
            this.myString = "my item 2 value";
        }

        public string DoSomething()
        {
            return "Ok item 2";
        }

    }

    public class ItemWithString : IItem
    {
        public string myString;

        public ItemWithString(string myString)
        {
            this.myString = myString;
        }
    }

    public class ItemWithParameters : IItem
    {
        public Item item;
        public string[] myArray;
        public string myString;
        public int myInt;
        public bool myBool;

        public ItemWithParameters(Item item, string[] myArray, string myString, int myInt, bool myBool)
        {
            this.item = item;
            this.myArray = myArray;
            this.myString = myString;
            this.myInt = myInt;
            this.myBool = myBool;
        }
    }


    public class ItemWithInt
    {
        public int MyInt { get; }

        public ItemWithInt(int myInt)
        {
            MyInt = myInt;
        }
    }


    public class ItemWithPrivateCtor
    {
        internal ItemWithPrivateCtor()
        {

        }
    }

    public class ItemWithMultiCtor
    {
        public bool EmptyCtor { get; set; }

        public ItemWithMultiCtor(string i)
        {

        }

        public ItemWithMultiCtor(int i)
        {

        }

        public ItemWithMultiCtor()
        {
            EmptyCtor = true;
        }
    }

    public class ItemWithPreferredCtor
    {
        public bool EmptyCtor { get; set; }

        public ItemWithPreferredCtor(string i)
        {

        }

        [PreferredConstructor]
        public ItemWithPreferredCtor(int i)
        {

        }

        public ItemWithPreferredCtor()
        {
            EmptyCtor = true;
        }
    }

    public class ItemWithDependency
    {
        [Dependency]
        public string MyString { get; set; }

        [Dependency(Name = "k1")]
        public Item MyItem { get; set; }

        public int MyInt { get; set; }
    }

}
