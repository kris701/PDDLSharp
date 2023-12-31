﻿using PDDLSharp.Tools;

namespace PDDLSharp.Models.SAS
{
    public class Fact
    {
        public int ID { get; set; } = -1;
        public string Name { get; }
        public string[] Arguments { get; }

        public Fact(string name, params string[] arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        // The order is important!
        // Based on: https://stackoverflow.com/a/30758270
        private int _hashCache = -1;
        public override int GetHashCode()
        {
            if (_hashCache != -1)
                return _hashCache;
            const int seed = 487;
            const int modifier = 31;
            unchecked
            {
                _hashCache = 50 * Name.GetHashCode() + Arguments.Length * Arguments.Aggregate(seed, (current, item) =>
                    (current * modifier) * item.GetHashCode());
                return _hashCache;
            }
        }

        /// <summary>
        /// Equals is just based on the ID of the fact, since the translator only outputs unique IDs
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj is Fact f)
            {
                if (ID != f.ID)
                    return false;
                return true;
            }
            return false;
        }

        public bool ContentEquals(object? obj)
        {
            if (obj is Fact f)
            {
                if (Name != f.Name) return false;
                if (!EqualityHelper.AreListsEqual(Arguments, f.Arguments)) return false;
                return true;
            }
            return false;
        }

        public override string? ToString()
        {
            var retStr = Name;
            foreach (var arg in Arguments)
                retStr += $" {arg}";
            return retStr;
        }

        public Fact Copy()
        {
            var arguments = new string[Arguments.Length];
            for (int i = 0; i < Arguments.Length; i++)
                arguments[i] = Arguments[i];
            var newFact = new Fact(Name, arguments);
            newFact.ID = ID;
            return newFact;
        }
    }
}
