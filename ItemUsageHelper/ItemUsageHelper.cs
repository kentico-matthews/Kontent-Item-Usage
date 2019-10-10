using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kentico.Kontent.Delivery;
using Newtonsoft.Json.Linq;
namespace ItemUsage
{
    public class ItemUsageHelper
    {
        const string RICHTEXT = "rich_text";
        const string MODULAR = "modular_content";

        public IDeliveryClient Client { get; set; }

        public IReadOnlyList<ContentType> Types;
        public List<ContentItem> AllItems;
        public Dictionary<string, string> UsedItems;
        public List<KeyValuePair<string, string>> UnusedItems;
        public List<ElementDetails> ModularElements;
        public IEnumerable<string> ModularElementNames;

        /// <summary>
        /// Instantiates a new item usage helper object using the production api
        /// </summary>
        /// <param name="projectID">The ID of the project</param>
        public ItemUsageHelper(string projectID)
        {
            Client = DeliveryClientBuilder.WithProjectId(projectID).Build();
            ModularElements = new List<ElementDetails>();
            UsedItems = new Dictionary<string, string>();
            AllItems = new List<ContentItem>();
        }

        /// <summary>
        /// Instantiates a new item usage helper object using the preview api
        /// </summary>
        /// <param name="projectID">The ID of the project</param>
        /// <param name="previewKey">The Preview API key of the project</param>
        public ItemUsageHelper(string projectID, string previewKey)
        {
            Client = DeliveryClientBuilder.WithOptions(builder => builder
                .WithProjectId(projectID)
                .UsePreviewApi(previewKey)
                .Build())
            .Build();
            ModularElements = new List<ElementDetails>();
            UsedItems = new Dictionary<string, string>();
            AllItems = new List<ContentItem>();
        }

        /// <summary>
        /// Get the codenames and display names of items which are not referenced by any other items
        /// </summary>
        /// <returns>Returns a list of KeyValuePairs containing the codenames and display names of unreferenced objects</returns>
        public virtual List<KeyValuePair<string, string>> GetUnusedItems()
        {
            FindModularElements();
            FindAllItems();
            FindUsedITems();
            FindUnusedItems();
            return UnusedItems;
        }
        /// <summary>
        /// Get the codenames and display names of items which are referenced by other items
        /// </summary>
        /// <returns>Returns a list of KeyValuePairs containing the codenames and display names of referenced objects</returns>
        public virtual List<KeyValuePair<string, string>> GetUsedItems()
        {
            FindModularElements();
            FindAllItems();
            FindUsedITems();
            return UsedItems.ToList();
        }
        /// <summary>
        /// Get a list of modular content (linked item) elements that exist in the project
        /// </summary>
        /// <returns>Returns a list of ElementDetails objects, which contain the name, codename, content type name, and content type codename of linked item elements in the project</returns>
        public virtual List<ElementDetails> GetModularElements()
        {
            FindModularElements();
            return ModularElements;
        }

        /// <summary>
        /// Finds modular content (linked item) elements in order to see which items are referenced
        /// </summary>
        protected virtual void FindModularElements()
        {
            Types = Client.GetTypesAsync().Result.Types;
            foreach (ContentType type in Types)
            {
                foreach (KeyValuePair<string, ContentElement> elem in type.Elements)
                {

                    if (elem.Value.Type.Equals(MODULAR))
                    {
                        ElementDetails newElement = new ElementDetails
                        {
                            Codename = elem.Key,
                            Name = elem.Value.Name,
                            ElementType=MODULAR,
                            ContentTypeCodename = type.System.Codename,
                            ContentTypeName = type.System.Name
                        };
                        ModularElements.Add(newElement);
                    }
                    else if (elem.Value.Type.Equals(RICHTEXT))
                    {
                        ElementDetails newElement = new ElementDetails
                        {
                            Codename = elem.Key,
                            Name = elem.Value.Name,
                            ElementType=RICHTEXT,
                            ContentTypeCodename = type.System.Codename,
                            ContentTypeName = type.System.Name
                        };
                        ModularElements.Add(newElement);
                    }
                }
            }
            ModularElementNames = ModularElements.Select(i => i.Codename);
        }

        /// <summary>
        /// Gets all of the items in the project
        /// </summary>
        protected virtual void FindAllItems()
        {
            IDeliveryItemsFeed feed = Client.GetItemsFeed(new ElementsParameter(ModularElementNames.ToArray()));
            while (feed.HasMoreResults)
            {
                DeliveryItemsFeedResponse response = feed.FetchNextBatchAsync().Result;
                foreach (ContentItem item in response)
                {
                    AllItems.Add(item);
                }
            }
        }

        /// <summary>
        /// Figures out which items are referenced by any modular/richtext fields
        /// </summary>
        protected virtual void FindUsedITems()
        {
            foreach (ContentItem item in AllItems)
            {
                foreach (string elem in ModularElementNames)
                {
                    var modularElement = item.Elements[elem];
                    if (modularElement != null)
                    {
                        if (modularElement["type"].ToString().Equals(MODULAR))
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
                        else if (modularElement["type"].ToString().Equals(RICHTEXT))
                        {
                            JArray modularItems = (JArray)modularElement[MODULAR];
                            for (int i = 0; i < modularItems.Count; i++)
                            {
                                string codename = modularElement[MODULAR][i].ToString();
                                if (!UsedItems.Keys.Contains(codename))
                                {
                                    UsedItems.Add(codename, GetDisplayName(codename));
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Looks up the display name for the given codename in the AllItems list
        /// </summary>
        /// <param name="codename">The codename of the item</param>
        /// <returns>Returns the display name (System.Name) of the given item</returns>
        protected string GetDisplayName(string codename)
        {
            return AllItems.Where(i => i.System.Codename == codename).FirstOrDefault().System.Name;
        }

        /// <summary>
        /// Calculates which items are not referenced by any other items
        /// </summary>
        protected virtual void FindUnusedItems()
        {
            var allDictionary = AllItems.Select(i => new KeyValuePair<string, string>(i.System.Codename, i.System.Name)).ToList();
            UnusedItems = allDictionary.Except(UsedItems).ToList();
        }
    }

    
}
