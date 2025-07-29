using Core.Entities;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace Utils
{
    public static class FillDbHelper
    {
        private static string? GetFullNameInApplicationTree(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            DirectoryInfo? dict = new DirectoryInfo(Directory.GetCurrentDirectory());
            FileInfo[] files = dict.GetFiles(fileName);


            while(dict != null && files.Length == 0) 
            {
                dict = dict.Parent;
                if(dict != null)
                {
                    files = dict.GetFiles(fileName);
                }
            }

            if(dict != null)
            {
                return files[0].FullName;
            }
            return null;
        }

        public static async Task<string[][]> ReadStringMatrixFromCsvAsync(string fileName, bool overreadTitleLine)
        {
            int startLine = 0; // soll die Titelzeile überlesen werden startet der Zeilenzähler bei 1
            int subtractIndex = 0; // und eine Zeile ist zu überlesen
            string? fullFileName = GetFullNameInApplicationTree(fileName); // csv-Datei liegt im Projektverzeichnis
            if (fullFileName == null)
            {
                throw new FileNotFoundException("File " + fileName + " not found in applicationpath");
            }
            string[] lines = await File.ReadAllLinesAsync(fullFileName, Encoding.UTF8);
            int lineCount = lines.Length;
            if (overreadTitleLine)
            {
                lineCount--;
                startLine = 1;
                subtractIndex = 1;
            }
            string[][] elements = new String[lineCount][];
            for (int line = startLine; line < lines.Length; line++)
            {
                elements[line - subtractIndex] = lines[line].Split(';');
            }
            return elements;
        }



  
    }
}
