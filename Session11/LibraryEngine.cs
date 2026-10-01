using System;
using System.Collections.Generic;

namespace Session11
{
    public class LibraryEngine
    {
        // (a) Parameterized with the USER-DEFINED delegate
        public static void ProcessBooks(List<Book> bList, BookFunctionDelegate fPtr)
        {
            foreach (Book B in bList)
            {
                Console.WriteLine(fPtr(B));
            }
        }

        // (b) Parameterized with the BUILT-IN delegate Func<Book, string>
        // (different name on purpose: passing a method group/lambda to two overloads
        // with equivalent delegate types would be ambiguous)
        public static void ProcessBooksWithFunc(List<Book> bList, Func<Book, string> fPtr)
        {
            foreach (Book B in bList)
            {
                Console.WriteLine(fPtr(B));
            }
        }
    }
}