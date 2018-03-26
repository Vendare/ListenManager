using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenManager.src.Managers.Messages
{
    class ReloadMemberMessage
    {
        public bool ReloadAllMembers { get; set; }
        public bool ReloadBirthdays { get; set; }
    }
}
