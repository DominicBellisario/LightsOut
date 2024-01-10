/*
 * Robinson Ruddock
 * 2/22/23
 * Text File Formatter
 * 
 * Loads a text file that was converted from an excel sheet,
 * then properly formats it into a new text file for the
 * level loader in the main game
 * 
 * No Known Issues
 */
namespace LevelFormatMachine
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            Format format = new Format();

            //load text file
            Console.WriteLine("What is the name of the file you want to format to?");
            string fileName = Console.ReadLine();
            format.Load("..\\..\\..\\" + fileName);

            //edit text file
            format.Editor();

            //create a new text file
            Console.WriteLine("What is the name of the new file you want to create?");
            fileName = Console.ReadLine();
            format.Save("..\\..\\..\\" + fileName);
        }

        
    }
}