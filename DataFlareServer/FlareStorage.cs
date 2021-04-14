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
        static Random rand;
        const uint SHORT_CODE_LENGTH = 4;
        static FlareStorage()
        {
            FlareCollection = new Dictionary<string, List<Flare>>();
            rand = new Random();
            if(SHORT_CODE_LENGTH == 0)
            {
                throw new Exception("SHORT_CODE_LENGTH must be greater than zero.");
            }
        }

        public static bool Add(Flare flare)
        {
            if(string.IsNullOrWhiteSpace(flare.Tag))
            {
                Debug.WriteLine("Trying to add flare with missing tag!");
                return false;
            }

            flare.Guid = Guid.NewGuid();
            flare.Created = DateTime.Now;

            if (!FlareCollection.ContainsKey(flare.Tag))
            {
                FlareCollection[flare.Tag] = new List<Flare>();
            }
            int attempts = 0;
            while (flare.ShortCode <= 0 && attempts < 10)
            {
                //For 4 digit shortcode, produces 1000-9999
                var check = rand.Next((int)Math.Pow(10, SHORT_CODE_LENGTH - 1), (int)Math.Pow(10, SHORT_CODE_LENGTH));
                if(FlareCollection[flare.Tag].Any(f => f.ShortCode == check))
                {
                    attempts++;
                }
                else
                {
                    flare.ShortCode = check;
                }
            }

            if(flare.ShortCode <= 0)
            {
                return false;
            }

            FlareCollection[flare.Tag].Add(flare);
            return true;
        }

        public static Flare GetShortCode(int shortCode)
        {
            foreach(var list in FlareCollection.Values)
            {
                var found = list.FirstOrDefault(f => f.ShortCode == shortCode);
                if(found != null)
                {
                    return found;
                }
            }

            return null;
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
            var purgeMinutes = 24 * 60;

            foreach(var list in FlareCollection.Values)
            {
                list.RemoveAll(f => DateTime.Now - f.Created > TimeSpan.FromMinutes(purgeMinutes));
            }
        }
    }
}
