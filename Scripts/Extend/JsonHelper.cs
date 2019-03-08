/*****************
author：myth
*****************/

namespace Myth.Extend
{
    using Newtonsoft.Json;

    public static class JsonHelper
    {
        public static T Deserialize<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        public static string Serialize<T>(this T t)
        {
            return JsonConvert.SerializeObject(t);
        }
    }
}
