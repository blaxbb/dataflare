using DataFlareClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DataFlareServer
{
    public static class FlareStorage
    {
        static Dictionary<string, List<Flare>> FlareCollection;
        static FlareStorage()
        {
            FlareCollection = new Dictionary<string, List<Flare>>();
        }

        public static void Add(Flare flare)
        {
            if(string.IsNullOrWhiteSpace(flare.Tag))
            {
                Debug.WriteLine("Trying to add flare with missing tag!");
                return;
            }

            flare.Guid = Guid.NewGuid();
            flare.Created = DateTime.Now;

            if (FlareCollection.ContainsKey(flare.Tag))
            {
                FlareCollection[flare.Tag].Add(flare);
            }
            else
            {
                FlareCollection[flare.Tag] = new List<Flare>()
                {
                    flare
                };
            }
        }

        public static List<Flare> GetTag(string tag)
        {
            return FlareCollection.ContainsKey(tag) ? FlareCollection[tag] : new List<Flare>();
        }

        public static Flare? Get(Guid guid)
        {
            foreach(var list in FlareCollection)
            {
                var found = list.Value.FirstOrDefault(f => f.Guid == guid);
                if(found != null)
                {
                    return found;
                }
            }

            return null;
        }

        public static void Cleanup()
        {
            var purgeMinutes = 30;

            foreach(var list in FlareCollection.Values)
            {
                list.RemoveAll(f => DateTime.Now - f.Created > TimeSpan.FromMinutes(30));
            }
        }
    }
}
