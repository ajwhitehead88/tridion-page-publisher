using System;
using System.Net;
using System.ServiceModel;
using log4net;
using log4net.Config;
using Tridion.ContentManager.CoreService.Client;

namespace PagePublisher
{
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                XmlConfigurator.Configure();

                // Parse the options
                var options = new Options();
                if (!CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    options.GetUsage();
                    return;
                }

                Log.Info("Starting page publisher ");
                var client = GetClient(options);

                client.Publish(options.Items,
                               new PublishInstructionData
                                   {
                                       ResolveInstruction = new ResolveInstructionData
                                               {
                                                   IncludeChildPublications = options.InChildren
                                               },
                                       RenderInstruction = new RenderInstructionData()
                                   },
                               options.Targets,
                               PublishPriority.Low,
                               new ReadOptions());
            }
            catch (Exception ex)
            {
                Log.Error("Error publishing pages: " + ex);
            }

            Log.Info("Finished");
        }

        /// <summary>
        /// Create a core service client using the options passed in
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private static ICoreService GetClient(Options options)
        {
            var hostname = options.Hostname;
            Log.Info("Creating client to " + hostname);
            hostname = string.Format("{0}{1}{2}",
                                     hostname.StartsWith("http") ? "" : "http://", hostname,
                                     hostname.EndsWith("/") ? "" : "/");

            var endpoint = new EndpointAddress(hostname + "webservices/CoreService2011.svc/basicHttp");

            var factory = new ChannelFactory<ICoreService>(new BasicHttpBinding
                {
                    MaxBufferSize = 104857600, // 100 MB
                    MaxBufferPoolSize = 104857600,
                    MaxReceivedMessageSize = 104857600,
                    SendTimeout = new TimeSpan(0, 0, 20, 0),
                    ReaderQuotas =
                        new System.Xml.XmlDictionaryReaderQuotas
                            {
                                MaxStringContentLength = 104857600, // 100 MB
                                MaxArrayLength = 104857600,
                            },
                    Security = new BasicHttpSecurity
                        {
                            Mode = BasicHttpSecurityMode.TransportCredentialOnly,
                            Transport = new HttpTransportSecurity
                                {
                                    ClientCredentialType = HttpClientCredentialType.Windows,
                                }
                        }
                }, endpoint);

            if (factory.Credentials == null)
            {
                throw new NullReferenceException("Core service factory has no credentials set!");
            }

            if (!string.IsNullOrEmpty(options.Username))
            {
                Log.Info("Using user " + options.Username);
                factory.Credentials.Windows.ClientCredential.UserName = options.Username;
                factory.Credentials.Windows.ClientCredential.Domain = options.Domain;
            }
            else
            {
                Log.Info("Using current user");
                factory.Credentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;
            }

            return factory.CreateChannel();
        }
    }
}
