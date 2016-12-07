using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace wesof
{
    class FileManager
    {
        public static bool WriteToFile(string content, string filename, CommandLineInterface _CLI)
        {
            bool success = false;

            try
            {
                StreamWriter sw = new StreamWriter(@filename);
                sw.Write(content);
                sw.Close();
                success = true;

            } catch (Exception e)
            {
                _CLI.Out("Error while saving the file:" + e.Message, true, true);
            }
            return success;
        }


    }
}
