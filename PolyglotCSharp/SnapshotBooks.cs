using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

// TEST ONLY
// Convert preprocessed book.bin, into a binary serilized C# data.
// So I can just load the data directly into bakv into C#, without having
// to do any processing.

// Results of testing
// Con:
// 1)   When saving uncompress the resulting file is about 3x 
//      larger than the original data source file.
// 
// 2)   When loading/decompressing the compress file it is taking very long to load.
//      Notacable longer, so much so its not really usable. 
//
// Pos:
//      Load the uncompress serialize binary seems a little slower than
//      reading and procesing the original data source file.
//      However, on a slower device this will problem not be the case, and
//      load the uncompress serialize binary should be fast than processing the source data.
//
//

namespace PolyglotCSharp
{
    public static class SnapshotBooks
    {
        public static void Save(string filename, bool compress = false)
        {
            if (OpeningBooks.books.Count > 0)
            {
                GZipStream gZipStream = null;
                
                System.IO.FileStream fileStream = new FileStream(filename, FileMode.Create);

                if (compress)
                {
                    gZipStream = new GZipStream(fileStream, CompressionMode.Compress);
                }
                BinaryFormatter binaryFormatter = new BinaryFormatter();
               
                try
                {
                    if (compress == true)
                    {
                        binaryFormatter.Serialize(gZipStream, OpeningBooks.books);
                    }
                    else
                    {
                        binaryFormatter.Serialize(fileStream, OpeningBooks.books);
                    }
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    fileStream.Close();
                }
            }
        }

        public static void Load(string filename, bool compressedfile = false)
        {
            GZipStream gZipStream = null;

            if (!File.Exists(filename))
                return;

            System.IO.FileStream fileStream = new FileStream(filename, FileMode.Open);

            if (compressedfile)
            {
                gZipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            try
            {

                if (compressedfile == true)
                {
                    OpeningBooks.books = (Dictionary<string, OpeningBooks.Book>)binaryFormatter.Deserialize(gZipStream);
                }
                else
                {
                    OpeningBooks.books = (Dictionary<string, OpeningBooks.Book>) binaryFormatter.Deserialize(fileStream);
                }

            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fileStream.Close();
            }

        }
    }
}

/*
var formatter = new BinaryFormatter();
using (var outputFile = new FileStream("OutputFile", FileMode.CreateNew))
using (var compressionStream = new GZipStream(
                         outputFile, CompressionMode.Compress))
{
    formatter.Serialize(compressionStream, objToSerialize);
    compressionStream.Flush();
}
*/
