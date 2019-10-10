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

	var publicreferences = new ItemUsageHelper(projectID);
	// Lists the codename (key) and display name (value) of published items which are not referenced by other published items
	List<KeyValuePair<string, string>> unused = references.GetUnusedItems();

  
