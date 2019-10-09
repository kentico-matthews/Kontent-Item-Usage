using System;
using System.Collections.Generic;
using ItemUsage;
namespace ItemUsageConsoleApp
{
    class Program
    {
        protected ItemUsageHelper Helper;
        protected string ProjectId;
        protected string PreviewKey;
        
        static void Main(string[] args)
        {
            Program window = new Program();
            bool continueEx = true;

            Welcome();

            while(continueEx)
            {
                window.ProcessInput(Console.ReadLine());
                Console.WriteLine("\r\n\r\nEnter a command");
            }
            
        }

        protected static void Welcome()
        {
            Console.WriteLine("Item Usage Helper\r\n" +
                "Enter a command\r\n" +
                "To set the project ID, enter 'setID Your_Project_ID'\r\n" +
                "To set the preview key, enter 'setKey Your_Preview_API_Key\r\n" +
                "To list items which are not referenced by any other item, enter 'unused'\r\n" +
                "To list items which are referenced, enter 'used'\r\n" +
                "To list linked item (a.k.a. modular content) elements which exist in your project, enter 'elements'");
        }

        protected void ProcessInput(string input)
        {
            string[] command = input.Split(' ');
            if (command.Length > 2)
            {
                Console.WriteLine("Please enter a single command, using a space between the command and its parameter if applicable");
            }
            else
            {
                switch (command[0])
                {
                    case "setID":
                        SetProjectID(command[1]);
                        break;
                    case "setKey":
                        SetPreviewKey(command[1]);
                        break;
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

        protected void SetProjectID(string id)
        {
            ProjectId = id;
        }
        protected void SetPreviewKey(string key)
        {
            PreviewKey = key;
        }
        protected void GetUnusedItems()
        {
            Helper = GetHelper();
            if (Helper != null)
            {
                foreach (KeyValuePair<string, string> item in Helper.GetUnusedItems())
                {
                    Console.WriteLine(item.Value);
                    Console.WriteLine(item.Key);
                    Console.WriteLine();
                }
            }
        }
        protected void GetUsedItems()
        {
            Helper = GetHelper();
            if (Helper != null)
            {
                foreach (KeyValuePair<string, string> item in Helper.GetUsedItems())
                {
                    Console.WriteLine(item.Value);
                    Console.WriteLine(item.Key);
                }
            }
        }
        protected void GetElements()
        {
            Helper = GetHelper();
            if (Helper != null)
            {
                foreach (ElementDetails details in Helper.GetModularElements())
                {
                    Console.WriteLine(details.Name);
                    Console.WriteLine(details.Codename);
                    Console.WriteLine("(From content type)");
                    Console.WriteLine(details.ContentTypeName);
                    Console.WriteLine(details.ContentTypeCodename);
                }
            }
        }
        protected bool ProjectIdIsBlank()
        {
           return (string.IsNullOrEmpty(ProjectId) || ProjectId.ToLower().Equals("null"));
        }
        protected bool PreviewKeyIsBlank()
        {
            return (string.IsNullOrEmpty(PreviewKey) || PreviewKey.ToLower().Equals("null"));
        }
        protected ItemUsageHelper GetHelper()
        {
            if (ProjectIdIsBlank())
            {
                Console.WriteLine("Please set the project ID first.");
            }
            else if (PreviewKeyIsBlank())
            {
                Console.WriteLine("(No preview key; published content only)");
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
                    Console.WriteLine("Error creating delivery client\r\n" + e.Message);
                }
            }
            return null;
        }
    }
}
