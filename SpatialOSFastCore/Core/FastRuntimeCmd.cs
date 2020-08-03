using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SpatialOSFastConnect
{
    public class FastRuntimeCmd
    {
        const string SpatialCmd = @"spatial";
        const string OperateCmd = @".\operate.exe";
        const string CmdPath = @"C:\Windows\System32\cmd.exe";

        //Generate dev auth token with the command
        public static string GenerateAuthToken(string chinf, string environment, string description = "default description")
        {
            string[] arguments = new string[3];
            arguments[0] = string.Format(@"project auth dev-auth-token create --description=""{0}""", description);
            arguments[1] = string.Format(@"--project_name {0} --lifetime=""24h10m20s""", chinf);
            arguments[2] = string.Format(@"--environment={0} --json_output", environment);

            var outJson = RunExternalExe(SpatialCmd, string.Join(" ", arguments));
            var jsonObject = JsonConvert.DeserializeObject<SpatialData>(outJson);
            return jsonObject.json_data.token_secret;
        }


        // Generate player identify token with the command
        public static string GeneratePlayerIdentifyToken(string tokenSecret, string playerIdentifier, string environment) {
            string[] arguments = new string[3];
            arguments[0] = string.Format(@"xavier");
            arguments[1] = string.Format(@"create-development-pit {0} {1}", tokenSecret, playerIdentifier);
            arguments[2] = string.Format(@"--environment={0}", environment);

            var outJson = RunExternalExe(OperateCmd, string.Join(" ", arguments));
            var start = outJson.IndexOf("{");
            var end = outJson.IndexOf("}");
            var msg = outJson.Substring(start, end + 1 - start).Replace("\\", "");
            var jsonObject = JsonConvert.DeserializeObject<SpatialPlayerIdentityData>(msg);
            return jsonObject.playerIdentityToken;
        }

        // Generate login token command
        public static Dictionary<string, string> GenerateLoginToken(string playerIdentityToken, string environment) 
        {
            string[] arguments = new string[3];
            arguments[0] = string.Format(@"xavier");
            arguments[1] = string.Format(@"create-development-login-tokens {0}", playerIdentityToken);
            arguments[2] = string.Format(@"--worker_type UnityClient --environment={0}", environment);

            var outJson = RunExternalExe(OperateCmd, string.Join(" ", arguments));
            var start = outJson.IndexOf("{");
            var end = outJson.IndexOf("]}");
            var msg = outJson.Substring(start, end + 2 - start).Replace("\\", "");
            var jsonObject = JsonConvert.DeserializeObject<SpatialLoginData>(msg);
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("loginToken", jsonObject.loginTokenDetails[0].loginToken);
            data.Add("deploymentName", jsonObject.loginTokenDetails[0].deploymentName);
            return data;
        }

        public static string StartClient(string ClientPath, string chinf, string myassembly, string loginToken , string playerIdentityToken, string environment) 
        {
            string[] arguments = new string[7];
            arguments[0] = string.Format(@"+projectName {0}", chinf);
            arguments[1] = string.Format(@"+deploymentName {0}", myassembly);
            arguments[2] = string.Format(@"+loginToken {0}", loginToken);
            arguments[3] = string.Format(@"+playerIdentityToken {0}", playerIdentityToken);
            arguments[4] = string.Format(@"+environment {0}", environment);
            SpatialEnvironmentConfig.data.TryGetValue(environment, out SpatialEnvironment env);
            arguments[5] = string.Format(@"+locatorHost {0}", env.locatorHost);
            arguments[6] = string.Format(@"+infraServicesUrl {0}", env.infraServicesUrl);
            var outJson = RunExternalExe(ClientPath + "UnityClient@Windows.exe", string.Join(" ", arguments));
            return outJson;
        }

        public static string RunExternalExe(string filename, string arguments = null)
        {
            var process = new Process();

            process.StartInfo.FileName = filename;
            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }
#if Debug
            Console.WriteLine("cmd:" + filename + " " +arguments);
#endif
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            var stdOutput = new StringBuilder();
            process.OutputDataReceived += (sender, args) => stdOutput.AppendLine(args.Data); // Use AppendLine rather than Append since args.Data is one line of output, not including the newline character.

            string stdError = null;
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                stdError = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception("OS error while executing " + Format(filename, arguments) + ": " + e.Message, e);
            }

            if (process.ExitCode == 0)
            {
#if Debug
                Console.WriteLine("stdOutput: " + stdOutput);
#endif
                return stdOutput.ToString();
            }
            else
            {
                var message = new StringBuilder();

                if (!string.IsNullOrEmpty(stdError))
                {
                    message.AppendLine(stdError);
                }

                if (stdOutput.Length != 0)
                {
                    message.AppendLine("Std output:");
                    message.AppendLine(stdOutput.ToString());
                }

                throw new Exception(Format(filename, arguments) + " finished with exit code = " + process.ExitCode + ": " + message);
            }
        }

        private static string Format(string filename, string arguments)
        {
            return "'" + filename +
                ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
                "'";
        }
    }
}
