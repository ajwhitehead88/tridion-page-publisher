using CommandLine;
using CommandLine.Text;

namespace PagePublisher
{
    /// <summary>
    /// Parsed model of the command line options
    /// </summary>
    public class Options
    {
        [Option('h', "hostname", Required = false, DefaultValue = "localhost", HelpText = "Hostname for the Tridion CME")]
        public string Hostname { get; set; }

        [Option('u', "username", Required = false, HelpText = "Username for login to the Tridion CME")]
        public string Username { get; set; }

        [Option('p', "password", Required = false, HelpText = "Password for login to the Tridion CME")]
        public string Password { get; set; }

        [Option('d', "domain", Required = false, HelpText = "Password for login to the Tridion CME")]
        public string Domain { get; set; }

        [OptionArray('i', "items", Required = true, HelpText = "List of TCMIDs of the items to publish")]
        public string[] Items{ get; set; }

        [OptionArray('t', "target", Required = true, HelpText = "TCMID of the publication target")]
        public string[] Targets { get; set; }

        [Option('c', "children", Required = false, DefaultValue = false, HelpText = "Publish in child publications")]
        public bool InChildren { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }
    }
}
