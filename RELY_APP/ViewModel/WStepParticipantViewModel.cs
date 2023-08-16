using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RELY_APP.ViewModel
{
    public class WStepParticipantViewModel
    {
        public int Id { get; set; }
        public int WStepId { get; set; }
        public string Type { get; set; }
        public int ParticipantId { get; set; }
        public bool IsDefault { get; set; }
        public string Description { get; set; }

        public string ParticipantName { get; set; }

    }
}