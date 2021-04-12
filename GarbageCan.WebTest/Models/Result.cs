using System.Collections.Generic;
using System.Linq;

namespace GarbageCan.WebTest.Models
{
    public class Result<T>
    {
        public string[] Errors { get; private set; }
        public bool Succeeded { get; private set; }
        public T Value { get; private set; }

        public static Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T> { Succeeded = false, Errors = errors?.ToArray() ?? new string[0] };
        }

        public static Result<T> Success(T value)
        {
            return new Result<T> { Value = value, Succeeded = true, Errors = new string[0] };
        }
    }
}