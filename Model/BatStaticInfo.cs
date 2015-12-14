﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Delta.PECS.WebCSC.Model
{
    /// <summary>
    /// Bat Static Information Class
    /// </summary>
    [Serializable]
    public class BatStaticInfo
    {
        /// <summary>
        /// LscID
        /// </summary>
        public int LscID { get; set; }

        /// <summary>
        /// LscName
        /// </summary>
        public string LscName { get; set; }

        /// <summary>
        /// Area1ID
        /// </summary>
        public int Area1ID { get; set; }

        /// <summary>
        /// Area1Name
        /// </summary>
        public string Area1Name { get; set; }

        /// <summary>
        /// Area2ID
        /// </summary>
        public int Area2ID { get; set; }

        /// <summary>
        /// Area2Name
        /// </summary>
        public string Area2Name { get; set; }

        /// <summary>
        /// Area3ID
        /// </summary>
        public int Area3ID { get; set; }

        /// <summary>
        /// Area3Name
        /// </summary>
        public string Area3Name { get; set; }

        /// <summary>
        /// StaID
        /// </summary>
        public int StaID { get; set; }

        /// <summary>
        /// StaName
        /// </summary>
        public string StaName { get; set; }

        /// <summary>
        /// DevID
        /// </summary>
        public int DevID { get; set; }

        /// <summary>
        /// DevName
        /// </summary>
        public string DevName { get; set; }

        /// <summary>
        /// DevIndex
        /// </summary>
        public int DevIndex { get; set; }

        /// <summary>
        /// StartTime
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// PunchTime
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// LastTime
        /// </summary>
        public double LastTime { get; set; }
    }
}
