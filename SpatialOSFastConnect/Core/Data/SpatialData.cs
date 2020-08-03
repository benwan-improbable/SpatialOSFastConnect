using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpatialOSFastConnect
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SpatialData
    {
        public string cli_version {set; get;}
        [JsonProperty]
        public SpatialAuthData json_data { set; get; }
        public string level { set; get; }
        public string msg { set; get; }
        public string time { set; get; }
    }

    public class SpatialAuthData 
    {
        public SpatialDevelopmentAuthData development_authentication_token { set; get; }

        public string token_secret { set; get; }
    }

    public class SpatialDevelopmentAuthData
    {
        public string id { set; get; }
        public string project_name { set; get; }
        public string description { set; get; }
        public string creation_time { set; get; }
        public string expiration_time { set; get; }
    }

    public class SpatialPlayerIdentityData
    {
        public string playerIdentityToken { set; get; }
    }

    public class SpatialLoginData
    { 
        public List<SpatialLoginToken> loginTokenDetails { set; get; }
    }

    public class SpatialLoginToken
    { 
        public string deploymentId { set; get; }

        public string deploymentName { set; get; }

        public List<string> tags { set; get; }

        public string loginToken { set; get; }
    }

    public class SpatialEnvironment
    {
        public string locatorHost { set; get; }
        public string infraServicesUrl { set; get; }
    }

    public class SpatialEnvironmentConfig
    {
        public static Dictionary<string, SpatialEnvironment> data ;

        static SpatialEnvironmentConfig()
        {
            data = new Dictionary<string, SpatialEnvironment>();

            data.Add("cn-testing", new SpatialEnvironment {
                locatorHost = "locator-cn-testing.spatialoschina.com",
                infraServicesUrl = "api-cn-testing.spatial.spatialoschina.com"
            });
            data.Add("cn-staging", new SpatialEnvironment
            {
                locatorHost = "locator-cn-staging.spatialoschina.com",
                infraServicesUrl = "api-cn-staging.spatial.spatialoschina.com"
            });
            data.Add("cn-production", new SpatialEnvironment
            {
                locatorHost = "locatorHost=locator.spatialoschina.com",
                infraServicesUrl = "api.spatial.spatialoschina.com"
            });
        }
    }
}
