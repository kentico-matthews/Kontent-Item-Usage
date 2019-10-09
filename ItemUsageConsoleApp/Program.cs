using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ItemUsage;
namespace ItemUsageConsoleApp
{
    class Program
    {
        //properties
        protected ItemUsageHelper Helper;
        protected string ProjectId;
        protected string PreviewKey;

        /// <summary>
        /// Main method for console app
        /// </summary>
        static void Main(string[] args)
        {
            Program window = new Program();
            bool continueEx = true;

            Welcome();

            while(continueEx)
            {
                Console.WriteLine();
                window.ProcessInput(Console.ReadLine());
                Console.WriteLine();
                Console.WriteLine("Enter a command");
            }
            
        }

        /// <summary>
        /// Writes initial welcome method
        /// </summary>
        protected static void Welcome()
        {
            Console.WriteLine("Item Usage Helper");
            Console.WriteLine("Enter a command");
            Console.WriteLine();

            Console.WriteLine("-To set the project ID, enter:");
            Console.WriteLine("setID YOUR_PROJECT_ID");
            Console.WriteLine();

            Console.WriteLine("-To set the preview key, enter:");
            Console.WriteLine("setKey YOUR_PREVIEW_API_KEY");
            Console.WriteLine();

            Console.WriteLine("-To list items which are not referenced by any other item, enter:");
            Console.WriteLine("unused OPTIONAL_CSV_OUTPUT_FILE_PATH");
            Console.WriteLine();

            Console.WriteLine("-To list items which are referenced, enter:");
            Console.WriteLine("used OPTIONAL_CSV_OUTPUT_FILE_PATH");
            Console.WriteLine();
            
            Console.WriteLine("-To list modular content elements which exist in your project, enter:");
            Console.WriteLine("elements OPTIONAL_CSV_OUTPUT_FILE_PATH");
        }

        /// <summary>
        /// Processes the command the user enters into the console
        /// </summary>
        /// <param name="input">The command they have entered</param>
        protected void ProcessInput(string input)
        {
            string[] command = input.Split(' ');
            if (command.Length > 2)
            {
                Console.WriteLine("Please enter a single command, using a space between the command and its parameter if applicable");
            }
            else
            {
                if (command.Length == 2)
                {
                    switch (command[0])
                    {
                        case "setID":
                            ProjectId = command[1];
                            break;
                        case "setKey":
                            PreviewKey = command[1];
                            break;
                        case "unused":
                            GetUnusedItems(command[1]);
                            break;
                        case "used":
                            GetUsedItems(command[1]);
                            break;
                        case "elements":
                            GetElements(command[1]);
                            break;

                        default:
                            Console.WriteLine("Command Not Recognized");
                            break;
                    }
                }
                else
                {
                    switch (command[0])
                    {
                        case "unused":
                            GetUnusedItems();
                            break;
                        case "used":
                            GetUsedItems();
                            break;
                        case "elements":
                            GetElements();
                            break;

                        default:
                            Console.WriteLine("Command Not Recognized");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Lists unused content items in the console
        /// </summary>
        protected void GetUnusedItems()
        {
            Helper = GetHelper();
            if (Helper != null)
            {
                var items = Helper.GetUnusedItems();
                Console.WriteLine("Unused Items (Codename, Name):");
                WriteItems(items);
            }
        }

        /// <summary>
        /// Lists unused content items in the console and writes them to a csv file
        /// </summary>
        /// <param name="path">Path of the csv file to create</param>
        protected void GetUnusedItems(string path)
        {
            Helper = GetHelper();
            if (Helper != null)
            {
                var items = Helper.GetUnusedItems();
                WriteCSV(items, path);
                Console.WriteLine($"File {path} created.");
                Console.WriteLine("Unused Items (Codename, Name):");
                WriteItems(items);
            }
        }

        /// <summary>
        /// Lists used content items in the console
        /// </summary>
        protected void GetUsedItems()
        {
            Helper = GetHelper();
            if (Helper != null)
            {
                var items = Helper.GetUsedItems();
                Console.WriteLine("Used Items (Codename, Name):");
                WriteItems(items);
            }
        }

        /// <summary>
        /// Lists used content items in the console and writes them to a csv file
        /// </summary>
        /// <param name="path">Path of the csv file to create</param>
        protected void GetUsedItems(string path)
        {
            Helper = GetHelper();
            if (Helper != null)
            {
                var items = Helper.GetUsedItems();
                WriteCSV(items, path);
                Console.WriteLine($"File {path} created.");
                Console.WriteLine("Used Items (Codename, Name):");
                WriteItems(items);
            }
        }

        /// <summary>
        /// Lists linked item elements in console
        /// </summary>
        protected void GetElements()
        {
            Helper = GetHelper();
            if (Helper != null)
            {
                var items = Helper.GetModularElements();
                Console.WriteLine("Modular Elements (Element Codename, Element Name, Content Type Codename, Content Type Name):");
                WriteItems(items);
            }
        }

        /// <summary>
        /// lists linked item elements in console and writes them to csv file
        /// </summary>
        /// <param name="path">Path of the csv file to create</param>
        protected void GetElements(string path)
        {
            Helper = GetHelper();
            if (Helper != null)
            {
                var items = Helper.GetModularElements();
                WriteCSV(items, path);
                Console.WriteLine($"File {path} created.");
                Console.WriteLine("Modular Elements (Element Codename, Element Name, Content Type Codename, Content Type Name):");
                WriteItems(items);
            }
        }
        
        /// <summary>
        /// Checks whether or not the project ID is null, empty, or set to "null" by the command
        /// </summary>
        protected bool ProjectIdIsBlank()
        {
           return (string.IsNullOrEmpty(ProjectId) || ProjectId.ToLower().Equals("null"));
        }

        /// <summary>
        /// Checks whether or not the Preview key is null, empty, or set to "null" by the command
        /// </summary>
        protected bool PreviewKeyIsBlank()
        {
            return (string.IsNullOrEmpty(PreviewKey) || PreviewKey.ToLower().Equals("null"));
        }

        /// <summary>
        /// Constructs an ItemUsageHelper based on the project id and preview api key (if supplied
        /// </summary>
        /// <returns>ItemUsageHelper</returns>
        protected ItemUsageHelper GetHelper()
        {
            if (ProjectIdIsBlank())
            {
                Console.WriteLine("Please set the project ID first.");
            }
            else if (PreviewKeyIsBlank())
            {
                Console.WriteLine("(No preview key; checking content only)");
                Console.WriteLine();
                try
                {
                    var helper = new ItemUsageHelper(ProjectId);
                    return helper;
                    
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error creating delivery client\r\n" + e.Message);
                }
            }
            else
            {
                try
                {
                    var helper = new ItemUsageHelper(ProjectId,PreviewKey);
                    return helper;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error creating delivery client\r\n");
                    Console.WriteLine(e.Message);
                }
            }
            return null;
        }

        /// <summary>
        /// Printing logic for content items
        /// </summary>
        protected void WriteItems(List<KeyValuePair<string, string>> list)
        {
                foreach (KeyValuePair<string, string> item in list)
                {
                    Console.WriteLine(item.Key);
                    Console.WriteLine(item.Value);
                    Console.WriteLine();
                }
        }

        /// <summary>
        /// Printing logic for content elements
        /// </summary>
        protected void WriteItems(List<ElementDetails> list)
        {
                foreach (ElementDetails item in list)
                {
                    Console.WriteLine(item.Codename);
                    Console.WriteLine(item.Name);
                    Console.WriteLine(item.ContentTypeCodename);
                    Console.WriteLine(item.ContentTypeName);
                    Console.WriteLine();
                }
        }

        /// <summary>
        /// CSV writing for content items
        /// </summary>
        protected void WriteCSV(List<KeyValuePair<string, string>> list, string path)
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("Item Codename,Item Name");
            foreach (KeyValuePair<string, string> item in list)
            {
                csv.AppendLine($"{item.Key},{item.Value}");
            }
            try
            {
                File.WriteAllText(path, csv.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating csv file");
                Console.WriteLine(e.Message);
            }

        }

        /// <summary>
        /// CSV writing for content elements
        /// </summary>
        protected void WriteCSV(List<ElementDetails> list, string path)
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("Element Codename,Element Name,Content Type Codename,Content Type Name");
            foreach (ElementDetails item in list)
            {
                csv.AppendLine($"{item.Codename},{item.Name},{item.ContentTypeCodename},{item.ContentTypeName}");
            }
            try
            {
                File.WriteAllText(path, csv.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating csv file");
                Console.WriteLine(e.Message);
            }
        }
    }
}
