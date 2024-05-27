using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NexCode.TinyMCE.Blazor.Code
{
    internal static class CommonHelpers
    {


        public static bool IsNotNullOrWhiteSpace([NotNullWhen(true)] this string? str) => !str.IsNullOrWhiteSpace();

        public static bool IsNullOrWhiteSpace([NotNullWhen(false)]this string? str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static void AddIf<T>(this ICollection<T> items, T? item, Func<T?, bool> predicate)
        {
            if(predicate(item))
                items.Add(item);
        }


        public static async ValueTask WhenAll(this IEnumerable<ValueTask> tasks)
        {
            var newTasks = tasks.Select(i => i.AsTask());
            await Task.WhenAll(newTasks);
        }


        public static void ReleaseAll(this SemaphoreSlim semaphore)
        {
            try {
                while (true)
                    semaphore.Release();
            }
            catch(SemaphoreFullException){}
            catch(ObjectDisposedException){}

        }

        public static string Random(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[RandomNumberGenerator.GetInt32(0, chars.Length)]).ToArray());
        }

    }

}
