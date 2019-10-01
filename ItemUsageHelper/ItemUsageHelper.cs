using System;
using System.Collections.Generic;
using System.Linq;
using Kentico.Kontent.Delivery;
using Newtonsoft.Json.Linq;
namespace ItemUsageHelper
{
    public class ItemUsageHelper
    {
        public IDeliveryClient Client { get; set; }

        public IReadOnlyList<ContentType> Types;
        public IEnumerable<ContentItem> AllItems;
        public Dictionary<string, string> UsedItems;
        public List<KeyValuePair<string, string>> UnusedItems;
        public Dictionary<string, ContentElement> ModularElements;
        public IEnumerable<string> ModularElementNames;

        public ItemUsageHelper(string projectID)
        {
            Client = DeliveryClientBuilder.WithProjectId(projectID).Build();
            ModularElements = new Dictionary<string, ContentElement>();
            UsedItems = new Dictionary<string, string>();
        }

        //public method for retreiving the names and codenames of unused items
        public virtual List<KeyValuePair<string, string>> GetUnusedItems()
        {
            FindModularElements();
            FindAllItems();
            FindUsedITems();
            FindUnusedItems();
            return UnusedItems;
        }

        //Finds modular content (linked item) elements in order to see which items are referenced 
        protected virtual void FindModularElements()
        {
            Types = Client.GetTypesAsync().Result.Types;
            foreach (ContentType type in Types)
            {
                foreach (KeyValuePair<string, ContentElement> elem in type.Elements)
                {

                    if (elem.Value.Type.Equals("modular_content"))
                    {
                        ModularElements.Add(elem.Key, elem.Value);
                    }
                }
            }
            ModularElementNames = ModularElements.Select(i => i.Key);
        }

        //retreives all items, including only system properties and modular elements
        protected virtual void FindAllItems()
        {
            var response = Client.GetItemsAsync(new ElementsParameter(ModularElementNames.ToArray()), new DepthParameter(0)).Result;
            AllItems = response.Items;
        }

        //figures out which items are referenced by any modular fields
        protected virtual void FindUsedITems()
        {
            foreach (ContentItem item in AllItems)
            {
                foreach (string elem in ModularElementNames)
                {
                    var modularElement = item.Elements[elem];
                    if (modularElement != null)
                    {
                        JArray modularItems = (JArray)modularElement["value"];
                        for (int i = 0; i < modularItems.Count; i++)
                        {
                            string codename = modularElement["value"][i].ToString();
                            if (!UsedItems.Keys.Contains(codename))
                            {
                                UsedItems.Add(codename, GetDisplayName(codename));
                            }
                        }
                    }
                }
            }
        }

        protected string GetDisplayName(string codename)
        {
            return AllItems.Where(i => i.System.Codename == codename).FirstOrDefault().System.Name;
        }

        //finds the difference between the list of all items and the list of used items
        protected virtual void FindUnusedItems()
        {
            var allDictionary = AllItems.Select(i => new KeyValuePair<string, string>(i.System.Codename, i.System.Name)).ToList();
            UnusedItems = allDictionary.Except(UsedItems).ToList();
        }
    }
}
