using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmashBcatDetector.Json
{
    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        private static List<string> IgnoredProperties = new List<string>
        {
            "TitleKey",
            "ContentKey",
            "Image"
        };


        public static ShouldSerializeContractResolver Instance { get; } = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);        
            if (IgnoredProperties.Contains(member.Name))
            {
                property.Ignored = true;
            }
            return property;
        }
}
}