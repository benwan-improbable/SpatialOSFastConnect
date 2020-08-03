using System;
using System.Threading.Tasks;

namespace SpatialOSFastConnect
{
    class Program
    {
        private static string environment = "cn-production";
        private static string chinf = "china_product_test";
        private static string playerIdentifier = "benwan";
        const string ClientPath = @"C:\Users\WanXi\Documents\Unity\tank-demo-ini\workers\unity\build\worker\UnityClient@Windows\";
        static void Main(string[] args)
        {
            ConnectSpatialAsync();
            Console.ReadLine();
        }

        public static async void ConnectSpatialAsync()
        {
            Console.WriteLine("Await Taskfunction Start");
            await Task.Run(() => {
                var tokenSecret = FastRuntimeCmd.GenerateAuthToken(chinf, environment);
                var playerIdentityToken = FastRuntimeCmd.GeneratePlayerIdentifyToken(tokenSecret, playerIdentifier, environment);
                var data = FastRuntimeCmd.GenerateLoginToken(playerIdentityToken, environment);
                data.TryGetValue("loginToken", out string loginToken);
                data.TryGetValue("deploymentName", out string deploymentName);
                FastRuntimeCmd.StartClient(ClientPath, chinf, deploymentName, loginToken, playerIdentityToken, environment);
            });
        }
    }
}
