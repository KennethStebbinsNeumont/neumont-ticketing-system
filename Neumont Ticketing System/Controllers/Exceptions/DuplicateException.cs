using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Controllers.Exceptions
{
    public class DuplicateException : Exception
    {
        public DuplicateException() : base()
        {
        }

        public DuplicateException(string? message) : base(message)
        {
        }

        public DuplicateException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public DuplicateException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }

    public class DuplicateException<T> : DuplicateException
    {
        public T Duplicate { get; private set; }
        public List<T> Duplicates { get; private set; }
        public int DuplicateCount { get
            {
                return Duplicates.Count;
            } 
        }

        public DuplicateException(T duplicate) : base()
        {
            InitSingle(duplicate);
        }

        public DuplicateException(List<T> duplicates): base()
        {
            InitList(duplicates);
        }

        public DuplicateException(T duplicate, string? message) : base(message)
        {
            InitSingle(duplicate);
        }

        public DuplicateException(List<T> duplicates, string? message) : base(message)
        {
            InitList(duplicates);
        }

        public DuplicateException(T duplicate, string? message, Exception? innerException)
            : base(message, innerException)
        {
            InitSingle(duplicate);
        }

        public DuplicateException(List<T> duplicates, string? message, Exception? innerException)
            : base(message, innerException)
        {
            InitList(duplicates);
        }

        public DuplicateException(T duplicate, System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            InitSingle(duplicate);
        }

        public DuplicateException(List<T> duplicates, System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            InitList(duplicates);
        }

        private void InitSingle(T duplicate)
        {
            Duplicate = duplicate;
            Duplicates = new List<T>(1);
            Duplicates.Add(duplicate);
        }

        private void InitList(List<T> duplicates)
        {
            if (duplicates.Count == 0)
                throw new ArgumentException("Given duplicates list cannot be empty");

            Duplicate = duplicates.First();
            Duplicates = duplicates;
        }
    }
}
