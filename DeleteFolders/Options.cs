using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeleteFolders
{
    public interface IDeleteFolderOptions
    {
        [Option("renewalday",
            HelpText = "The target day for renewal. If not specified the current day is used.")]
        int renewalday { get; set; }

        [Option("renewalmonth",
            HelpText = "The target month for renewal. If not specified the current month is used.")]
        int renewalmonth { get; set; }

        [Option("includeerrors", Default = false,
            HelpText = "Normally domains that are in an error state are skipped. This option will include those domains and retry them.")]
        bool includeerrors { get; set; }
    }

    [Verb("renew", HelpText = "Renew expiring certificates. If renewalday and renewalmonth are not specified it will default to the current month and day. (For more details try: --help renew)")]
    public class DeleteFolderOptions : IDeleteFolderOptions
    {
        private int _renewalday;
        private int _renewalmonth;
        private bool _includeerrors;

        public int renewalday
        {
            get { return _renewalday; }
            set { _renewalday = value; }
        }

        public int renewalmonth
        {
            get { return _renewalmonth; }
            set { _renewalmonth = value; }
        }

        public bool includeerrors
        {
            get { return _includeerrors; }
            set { _includeerrors = value; }
        }
    }

    [Verb("purge", HelpText = "Purge domains with expired certificates.")]
    public class PurgeOptions
    {
    }


}
