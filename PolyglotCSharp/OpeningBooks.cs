

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

// Polyglot: Chess OpenBook reader.

using System;
using System.Collections.Generic;
using System.IO;

namespace PolyglotCSharp
{
   
    public static class OpeningBooks
    {
        private static Dictionary<string, Book> books = new Dictionary<string, Book>();

        // Fast lookup square name to index
        public readonly static Dictionary<string, int> squareNames = new Dictionary<string, int>()
        {
            { "a8",0 }, {"b8",1}, {"c8",2}, {"d8",3}, {"e8",4}, {"f8",5}, {"g8",6},{"h8",7},
            { "a7",8 }, {"b7",9}, {"c7",10}, {"d7",11}, {"e7",12}, {"f7",13}, {"g7",14},{"h7",15},
            { "a6",16 }, {"b6",17}, {"c6",18}, {"d6",19}, {"e6",20}, {"f6",21}, {"g6",22},{"h6",23},
            { "a5",24 }, {"b5",25}, {"c5",26}, {"d5",27}, {"e5",28}, {"f5",29}, {"g5",30},{"h5",31},
            { "a4",32 }, {"b4",33}, {"c4",34}, {"d4",35}, {"e4",36}, {"f4",37}, {"g4",38},{"h4",39},
            { "a3",40 }, {"b3",41}, {"c3",42}, {"d3",43}, {"e3",44}, {"f3",45}, {"g3",46},{"h3",47},
            { "a2",48 }, {"b2",49}, {"c2",50}, {"d2",51}, {"e2",52}, {"f2",53}, {"g2",54},{"h2",55},
            { "a1",56 }, {"b1",57}, {"c1",58}, {"d1",59}, {"e1",60}, {"f1",61}, {"g1",62},{"h1",63},
        };

        // Fast lookup idx to square name
        public readonly static string[] squareIdxToName = new string[]
        {
            "a8",  "b8",  "c8", "d8", "e8", "f8",  "g8", "h8",
            "a7",  "b7",  "c7", "d7", "e7", "f7",  "g7", "h7",
            "a6",  "b6",  "c6", "d6", "e6", "f6",  "g6", "h6",
            "a5",  "b5",  "c5", "d5", "e5", "f5",  "g5", "h5",
            "a4",  "b4",  "c4", "d4", "e4", "f4",  "g4", "h4",
            "a3",  "b3",  "c3", "d3", "e3", "f3",  "g3", "h3",
            "a2",  "b2",  "c2", "d2", "e2", "f2",  "g2", "h2",
            "a1",  "b1",  "c1", "d1", "e1", "f1",  "g1", "h1"
        };

        /// <summary>
        /// Book:Dictionary<System.UInt64, List<Move>>
        /// </summary>
        public class Book : Dictionary<System.UInt64, List<Move>>
        {
            /// <summary>
            /// Merge with another book
            /// 
            /// </summary>
            /// <param name="book"></param>
            /// <param name="debug"></param>
            /// <returns>True if some entries merged.</returns>
            public bool Merge(Book book, bool debug = false)
            {
                int merged = 0;
                int count = 0;
                int startCount = Count;

                Book deltaBook = new Book();

                foreach (var entry in book)
                {
                    // book Hash moves need to merge with target moves 

                    if (ContainsKey(entry.Key))
                    {
                        // check each entry in source (book) to target (this book)

                        foreach (Move moveSource in entry.Value)
                        {
                            // Compare each move in the target (this book) list with current move from source

                            foreach (Move moveTarget in this[entry.Key])
                            {
                                if (moveSource.Compare(moveTarget)) // souce compare with ( this )
                                {
                                    // Build a merge list for later use.

                                    if (!deltaBook.ContainsKey(entry.Key))
                                    {
                                        deltaBook[entry.Key] = new List<Move>();
                                    }

                                    deltaBook[entry.Key].Add(moveSource);
                                }
                            }
                        }

                    }
                    else
                    {
                        this[entry.Key] = entry.Value;
                        count++;
                    }
                }


                foreach (var entry in deltaBook)
                {
                    System.UInt64 hash = entry.Key;
                    List<Move> moves = entry.Value;

                    /*if (debug)
                    {
                        System.Console.WriteLine("Moves in {0} hash before merging\n", hash);
                        for (int n = 0; n < this[hash].Count; n++)
                            System.Console.Write("{0}:{1},", n, this[hash][n].weight);
                        System.Console.Write("\n");
                    }*/

                    merged++;

                    for (int idx = 0; idx < moves.Count; idx++)
                    {
                        //int nfound = this[hash].FindIndex(x => x.weight > moves[idx].weight);
                        this[hash].Add(moves[idx]);
                    }

                    this[hash].Sort(delegate (Move x, Move y)
                    {
                        return y.weight.CompareTo(x.weight);
                    });


                    /*if (debug)
                    {
                        System.Console.WriteLine("Moves in {0} hash after merging\n", hash);
                        for (int n = 0; n < this[hash].Count; n++)
                            System.Console.Write("{0}:{1},", n, this[hash][n].weight);
                        System.Console.Write("\n\n");
                    }*/
                }

                if (debug)
                {
                    System.Console.WriteLine("\nMerge books\n\nOriginal num of hashs {0}, with {1} new hashs added. Test pass:{2}",
                    startCount, count, (startCount + count == Count));

                    System.Console.WriteLine("{0} hashs where merged with existing hashs", merged);
                }

                return (startCount < Count) || merged > 0;
            }
        }

        /// <summary>
        /// Hold information about the move.
        /// </summary>
        public class Move
        {
            private int _move;
            public int weight = 0;
            public int learn = 0;

            public int from = -1;
            public int to = -1;
            public string strmove = "";

            public Move(int move, int weight, int learn)
            {
                this.move = InitMove(move);
                this.weight = weight;
                this.learn = learn;
            }

            public int move
            {
                get { return _move; }
                set { _move = InitMove(value); }
            }

            public bool isValid()
            {
                return (move != 0 && from != 1 && to != -1 && strmove.Length >= 4 && strmove != "a1a1");
            }

            public bool Compare(Move m)
            {
                return (m.move == move && m.weight == weight && m.learn == learn);
            }

            /// <summary>
            /// Fill information about the move 
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int InitMove(int value)
            {
                int result = -1; // Invalid move;

                if (value > 0)
                {
                    strmove = ConvertMoveToString(value);

                    if (strmove.Length >= 4)
                    {
                        string sfrom = strmove.Substring(0, 2);

                        if (squareNames.ContainsKey(sfrom))
                            from = squareNames[sfrom];

                        string sto = strmove.Substring(2, 2);

                        if (squareNames.ContainsKey(sfrom))
                            to = squareNames[sto];

                        if (from != -1 && to == -1)
                            result = value;
                    }
                }

                return result;
            }

            public override string ToString()
            {
                return "move: " + strmove + " weight:" + weight + " learn:" + learn;
            }

            /// <summary>
            /// Conver the move into a string representation
            /// </summary>
            /// <param name="move"></param>
            /// <returns>string representation of move value</returns>
            private string ConvertMoveToString(int move)
            {
                const string promote_pieces = " nbrq";
                string strMove = "";

                int f, fr, ff, t, tr, tf, p;

                f = (move >> 6) & 0x3F;
                fr = (f >> 3) & 0x7;
                ff = f & 0x7;
                t = move & 0x3F;
                tr = (t >> 3) & 0x7;
                tf = t & 0x7;
                p = (move >> 12) & 0x7;

                strMove += Convert.ToChar(ff + Convert.ToByte('a'));
                strMove += Convert.ToChar(fr + Convert.ToByte('1'));
                strMove += Convert.ToChar(tf + Convert.ToByte('a'));
                strMove += Convert.ToChar(tr + Convert.ToByte('1'));

                if (p > 0)
                {
                    strMove += Convert.ToChar(promote_pieces[(int)p]);
                }

                //Castling moves

                if (strMove == "e1h1")
                {
                    strMove = "e1g1";
                }
                else if (strMove == "e1a1")
                {
                    strMove = "e1c1";
                }
                else if (strMove == "e8h8")
                {
                    strMove = "e8g8";
                }
                else if (strMove == "e8a8")
                {
                    strMove = "e8c8";
                }

                return strMove;
            }
        }

        /// <summary>
        /// Read System.UInt64 using bit operations on the bytes.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        private static System.UInt64 ReadValue(BinaryReader reader, int l)
        {
            int i;
            System.Byte c;
            System.UInt64 r = 0;

            for (i = 0; i < l; i++)
            {
                c = reader.ReadByte();
                r = (r << 8) + c;
            }

            return r;
        }

        /// <summary>
        /// Load a opening book into Books collection
        /// </summary>
        /// <param name="bookname">Key to access book in Books collection</param>
        /// <param name="fileName">The actual file to load, includeding any path</param>
        /// <returns>A Book or null if book could not be loaded</returns>
        public static Book LoadBook(string bookname, string fileName)
        {
            Book book = new Book();
          
            if(!File.Exists(fileName))
            {
                return null;
            }

            Stream stream = File.OpenRead(fileName);
           
            long size = stream.Length / 16;

            BinaryReader binaryReader = new BinaryReader(stream);

            for (int idx = 0; idx < size; idx++)
            {
                System.UInt64 hash = 0;

                try
                {
                    hash = ReadValue(binaryReader, 8);

                    Move moves = new Move(
                        (int)ReadValue(binaryReader, 2), // move
                        (int)ReadValue(binaryReader, 2), // weight
                        (int)ReadValue(binaryReader, 4)); // learn

                    if (moves.isValid())
                    {
                        if (book.ContainsKey(hash))
                        {
                            book[hash].Add(moves);
                        }
                        else
                        {
                            List<Move> list = new List<Move>();
                            list.Add(moves);
                            book.Add(hash, list);
                        }
                    }
                }
                catch (Exception)
                {
                  if(stream.Position == stream.Length)
                  {
                        //Book could have errors
                        return null;
                  }
                } 
            }
            if(book.Count == 0)
            {
                // Empty book
                return null;
            }

            books[bookname] = book;
            return book;
        }

        /// <summary>
        /// Get a open book from collection of loaded books.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>book</returns>
        public static Book GetBook(string name)
        {
            Book book = null;

            if(books.ContainsKey(name))
            {
                book = books[name];
            }

            return book;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static List<Move> GetMoveList(Book book, System.UInt64 hash)
        {
            if((book.Count > 0) && book.ContainsKey(hash))
            {
                return book[hash];
            }

            return null;
        }

        public static Move GetMoveBest(Book book, System.UInt64 hash)
        {
            if ((book.Count > 0) && book.ContainsKey(hash))
            {
                return book[hash][0];
            }

            return null;
        }


        /// <summary>
        /// :
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static Move GetMoveWorst(Book book, System.UInt64 hash)
        {
            if ((book.Count > 0) && book.ContainsKey(hash))
            {
                return book[hash][(book[hash].Count > 1) ? book[hash].Count - 1 : 0 ];
            }

            return null;
        }
    }
}
