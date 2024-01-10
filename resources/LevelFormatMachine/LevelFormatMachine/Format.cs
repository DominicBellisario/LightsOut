using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Robinson Ruddock
 * 2/22/23
 * Format Class
 * 
 * Handles text files and formats them properly for the game
 * 
 * No Known Issues
 */
namespace LevelFormatMachine
{
    internal class Format
    {
        private string[] data;

        //loads data from text file
        public void Load(string fileName)
        {
            StreamReader input = null;
            try
            {
                input = new StreamReader(fileName);

                string currentLine;

                //determine how far in the text file to read
                int height = getHeight(fileName);

                data = new string[height];

                //skip over some lines
                input.ReadLine();
                input.ReadLine();
                input.ReadLine();
                input.ReadLine();

                //load lines from text file into an array
                for (int i=0; (currentLine = input.ReadLine()) != null && i < height; i++)
                {
                    data[i] = currentLine;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //if input was opened close it
            if (input != null)
            {
                input.Close();
            }
        }

        //saves formatted data to text file
        public void Save(string fileName)
        {
            StreamWriter output = null;
            try
            {
                output = new StreamWriter(fileName);

                for(int i=0; i < data.Length; i++)
                {
                    output.WriteLine(data[i]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //if output was opened close it
            if (output != null)
            {
                output.Close();
            }
        }


        //formats strings in data
        public void Editor()
        {
            for(int i=0; i < data.Length; i++)
            {
                data[i] = ShaveFirstTwoColumns(data[i]);
                data[i] = ReplaceLetter(data[i], (char)32, "E");
                data[i] = ReplaceLetter(data[i], (char)9, ",");
            }
        }


        //finds the bounds of the text file being formatted
        public static int getHeight(string fileName)
        {
            //declare variables
            StreamReader input = null;
            bool isRead = true;
            string currentLine;
            int height = 0;

            try
            {
                input = new StreamReader(fileName);

                //skip through lines
                input.ReadLine();
                input.ReadLine();
                input.ReadLine();
                input.ReadLine();
                
                //determines height by finding the bounds of the level
                while ((currentLine = input.ReadLine()) != null && isRead)
                {
                    //ending bounds
                    if (CheckForLetter(currentLine, "?"))
                    {
                        isRead = false;
                    }
                    height++;
                }
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //close if input was opened
            if (input != null)
            {
                input.Close();
            }

            return height - 1;
        }


        //check if a string has a specific letter in it
        public static bool CheckForLetter(string line, string letter)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line.Substring(i, 1) == letter)
                {
                    return true;
                }
            }
            return false;
        }


        //replaces a specified character with a different letter
        public static string ReplaceLetter(string line, char letter, string formatLetter)
        {
            string formattedLine = "";

            //creates a new string with proper formatting
            foreach(char word in line)
            {
                if(word == letter)
                {
                    formattedLine = formattedLine + formatLetter;
                }
                else
                {
                    formattedLine = formattedLine + word;
                }
            }
            
            return formattedLine;
        }

        //removes first two columns to increase efficiency of game
        public static string ShaveFirstTwoColumns(string line)
        {
            string formattedLine = "";
            int i = 0;
            foreach(char word in line)
            {
                if(i > 1)
                {
                    formattedLine = formattedLine + word;
                }
                i++;
            }

            return formattedLine;
        }
    }
}
