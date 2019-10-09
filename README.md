**Item usage helper**  
This is a utility class to help find items which are/aren't referenced by any other items in Kontent.  
It can be used as sample code, or added to your project as a class library.  
  
**Examples**  
  
    string projectID = "[Your Project ID]";
	string previewKey = "[Your Preview Key]";
	
	var references = new ItemUsageHelper(projectID,previewKey);
	
	// Lists the codename (key) and display name (value) of items which are not referenced by other items
	List<KeyValuePair<string, string>> unused = references.GetUnusedItems();

	// Lists the codename (key) and display name (value) of items which are referenced by other items 
	List<KeyValuePair<string, string>> used = references.GetUsedItems();  
  
The compiled version can be downloaded at https://kenticosoftware-my.sharepoint.com/:u:/g/personal/matthews_kentico_com/EenMJldqT4xCnxSSbDnbZBoBaiUiMuH756gLL9R20Y8zCg?e=HhmTD8.  
To run it, extract the folder, then either double click the exe file or run it from the command line.