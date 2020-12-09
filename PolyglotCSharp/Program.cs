
/*************************************************************************/
/* Copyright (c) 2020 Chay Palton                                        */
/*                                                                       */
/* Permission is hereby granted, free of charge, to any person obtaining */
/* a copy of this software and associated documentation files (the       */
/* "Software"), to deal in the Software without restriction, including   */
/* without limitation the rights to use, copy, modify, merge, publish,   */
/* distribute, sublicense, and/or sell copies of the Software, and to    */
/* permit persons to whom the Software is furnished to do so, subject to */
/* the following conditions:                                             */
/*                                                                       */
/* The above copyright notice and this permission notice shall be        */
/* included in all copies or substantial portions of the Software.       */
/*                                                                       */
/* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,       */
/* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF    */
/* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.*/
/* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY  */
/* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,  */
/* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE     */
/* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                */
/*************************************************************************/

using System;
using System.Collections.Generic;


///////////////////////////////////////////////////
// Polyglot: Chess OpenBook reader and checker.   //
// Can also mergw books: TODO Display board     //
//////////////////////////////////////////////////
namespace Chess.Utils
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Chay's chess opening book, and data checker.\n");

            string filename = "..\\..\\..\\..\\..\\book\\komodo.bin";
            string bookname = "komodo";

            System.Console.WriteLine("Try to load opening book {0}", filename);
            OpeningBooks.Book bookCodeKomodo = OpeningBooks.LoadBook(bookname, filename);

            if (bookCodeKomodo != null)
            {
                TestCaseBookInfo(filename, bookname);
            } else
            {
                System.Console.WriteLine("Could not load openbook {0}", filename);
            }

            System.Console.WriteLine("Try to load opening book {0}", filename);
            filename = "..\\..\\..\\..\\..\\book\\codekiddy.bin";
            bookname = "codekiddy";

            OpeningBooks.Book bookCodeKiddy = OpeningBooks.LoadBook(bookname, filename);

            if (bookCodeKomodo != null)
            {
                TestCaseBookInfo(filename, bookname);
            }
            else
            {
                System.Console.WriteLine("Could not load opening book {0}", filename);
            }

            if (bookCodeKomodo != null && bookCodeKomodo != null)
            {
                bookCodeKiddy.Merge(bookCodeKomodo, true);

            }
            else
            {
                System.Console.WriteLine("Unable to merge books, as required book(s) not loaded.");
            }
        }

        static void TestCaseBookInfo(string filename, string bookname)
        {
            
            OpeningBooks.Book book = OpeningBooks.GetBook(bookname);

            if (book != null)
            {
                System.Console.WriteLine("Opening book checking {0}", filename);

                System.Console.WriteLine("\nBook has {0} entries.", (book.Count));

                TestCaseShowRangeOfMoves(book);

                string fenStartPos = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                System.UInt64 hashStartPos = 0x463b96181691fc9c;
                List<OpeningBooks.Move> oplist = OpeningBooks.GetMoveList(book, hashStartPos);

                if (oplist.Count > 0)
                {
                    System.Console.WriteLine("\n\nBook has starting position move(s),\nfor FEN {0}", fenStartPos);
                }

                TestCaseHash(book, hashStartPos, "e2e4");

                List<OpeningBooks.Move> mlstA = OpeningBooks.GetMoveList(book, 0x463b96181691fc9c);
                List<OpeningBooks.Move> mlstB = OpeningBooks.GetMoveList(book, 0x5c3f9b829b279560);
                List<OpeningBooks.Move> mlstC = OpeningBooks.GetMoveList(book, 0x3c8123ea7b067637);

            }
            else
            {
                System.Console.WriteLine("Book not found {0}\n", bookname);

            }
        }

        static void TestCaseShowRangeOfMoves(Dictionary<System.UInt64, List<OpeningBooks.Move>> book)
        {
            // count each keys that have multiply move to select from.
            int[] multiMoveKeys = new int[10000]; // Never this big

            int highest = 0;
            if (book != null)
            {
                foreach (KeyValuePair<System.UInt64, List<OpeningBooks.Move>> lst in book)
                {
                    if (lst.Value.Count < multiMoveKeys.Length)
                    {
                        if (highest < lst.Value.Count)
                            highest = lst.Value.Count;

                        multiMoveKeys[lst.Value.Count]++;
                    }
                }

                System.Console.WriteLine("\nHashs with move(s) avalible.");
                for (int idx = 0; idx < highest - 1; idx++)
                {
                    System.Console.WriteLine("\t{0} Hash have move(s) = {1}", multiMoveKeys[idx], idx);
                }
            }
        }


        static bool TestCaseHash(Dictionary<System.UInt64, List<OpeningBooks.Move>> book, System.UInt64 hash, string strmove)
        {
            bool result = false;
            int found = -1;

            System.Console.WriteLine("Test: Book has move \"{0}\" in hash {1}", strmove, hash);
            
            if (book != null && book.ContainsKey(hash))
            {
                List<OpeningBooks.Move> oplst = book[hash];
    
                for (int idx = 0; idx < oplst.Count; idx++)
                {
                    if (oplst[idx].strmove == strmove)
                    {
                        found = idx;
                    }
                }

                if (found != -1)
                {
                    result = true;
                    System.Console.WriteLine("Passed found..{0}", oplst[found]);
                }
            }

            return result;
        }
    }
}
